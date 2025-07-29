using System.Reflection;

namespace TW.Args.Net
{
    /// <summary>
    /// <para>
    /// Command-line arguments parser.
    /// </para>
    /// <para>
    /// Terminology:
    /// <list type="table">
    /// <item>Argument - a positional argument.</item>
    /// <item>Option (switch) - options, prepeded with -- or -.</item>
    /// </list>
    /// </para>
    /// </summary>
    public class ArgumentsParser : ArgumentsDefinition
    {
        public ArgumentsParser(Assembly? assembly = null, string? executableName = null) 
            : base(assembly, executableName)
        { }


        public string OptionPrefix { get; set; } = "--";
        public string OptionShortcutPrefix { get; set; } = "-";


        public static void Parse(string[] args, Action<ParsedArguments> handler)
        {
            try
            {
                handler(new ArgumentsParser().Parse(args));
            }
            catch (HelpRequestedException)
            {
                Console.WriteLine(new ArgumentsHelp().GetText());
            }
            catch (SyntaxException ex)
            {
                Console.WriteLine($"Syntax error: {ex.Message}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error: {ex.ToString()}");
#else
                Console.WriteLine($"Error: {ex.Message}");
#endif
            }
        }

        public static void Parse<T>(string[] args, Action<T> handler) where T : class
        {
            try
            {
                handler(new ArgumentsParser().Parse<T>(args));
            }
            catch (HelpRequestedException)
            {
                Console.WriteLine(new ArgumentsHelp().GetText());
            }
            catch (SyntaxException ex)
            {
                Console.WriteLine($"Syntax error: {ex.Message}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error: {ex.ToString()}");
#else
                Console.WriteLine($"Error: {ex.Message}");
#endif
            }
        }


        public ParsedArguments Parse(string[] args)
        {
            if (args.Length == 1 && (args[0] == $"{OptionPrefix}help" || args[0] == $"{OptionShortcutPrefix}h")) throw new HelpRequestedException();

            var syntaxVariants = InstantiateSyntaxVariants();
            var arguments = ExtractArguments(args);
            var options = ExtractOptions(args);

            for (int i = 0; i < syntaxVariants.Count; i++)
            {
                var syntaxVariant = syntaxVariants[i];
                ParseArguments(arguments, ref syntaxVariant);
                ParseOptions(options, ref syntaxVariant);
            }

            var selectedSyntaxVariants = SelectValidSyntaxVariants(syntaxVariants);

            return selectedSyntaxVariants.Count switch
            {
                1 => new ParsedArguments(selectedSyntaxVariants[0]),
                0 => throw new SyntaxException("Provided arguments are incorrect, use --help or -h option to display help"),
                _ => throw new SyntaxException("Ambiguous syntax definition, more than one syntax variants match provided arguments")
            };
        }


        public T Parse<T>(string[] args) where T : class => Parse(args).GetSyntaxVariant<T>();


        private List<string> ExtractArguments(string[] args) => args
            .ToList()
            .FindAll(x => !x.StartsWith(OptionPrefix) && !x.StartsWith(OptionShortcutPrefix));


        private List<Option> ExtractOptions(string[] args) => args
            .ToList()
            .FindAll(x => x.StartsWith(OptionPrefix) || x.StartsWith(OptionShortcutPrefix))
            .Select(x => new Option(x))
            .ToList();


        private void ParseArguments(List<string> arguments, ref object syntaxVariant)
        {
            var properties = GetPropertiesWithAttribute<ArgumentAttribute>(syntaxVariant);
            if (arguments.Count > properties.Count())   // There are more arguments than the variant can accept
            {
                ((Arguments)syntaxVariant).Invalidate();
                return;
            }

            for (int position = 0; position < arguments.Count; position++)
            {
                var argument = arguments[position];

                var optionFound = false;
                foreach (var property in properties)
                {
                    var attribute = GetPropertyAttribute<ArgumentAttribute>(property);
                    if (attribute!.Position == position)
                    {
                        if (attribute.RequiredValue == argument || attribute.RequiredValue == null)
                        {
                            switch (GetPropertyType(property).FullName)
                            {
                                case "System.Int16": property.SetValue(syntaxVariant, Convert.ToInt16(argument)); break;
                                case "System.Int32": property.SetValue(syntaxVariant, Convert.ToInt32(argument)); break;
                                case "System.Int64": property.SetValue(syntaxVariant, Convert.ToInt64(argument)); break;
                                case "System.String": property.SetValue(syntaxVariant, argument); break;
                                default: throw new SyntaxException($"Argument {argument} of type {GetPropertyType(property).FullName} is not supported");
                            }

                            optionFound = true;
                        }
                    }
                }
                if (!optionFound)
                {
                    ((Arguments)syntaxVariant).Invalidate();
                }
            }
        }


        private void ParseOptions(List<Option> options, ref object syntaxVariant)
        {
            var properties = GetPropertiesWithAttribute<OptionAttribute>(syntaxVariant);

            foreach (var option in options)
            {
                var optionFound = false;
                foreach (var property in properties)
                {
                    var attribute = GetPropertyAttribute<OptionAttribute>(property);
                    if ($"{OptionPrefix}{attribute!.Name}" == option.Name || $"{OptionShortcutPrefix}{attribute!.ShortcutName}" == option.Name)
                    {
                        if (GetPropertyType(property).FullName != "System.Boolean" && !option.HasValue)
                        {
                            throw new SyntaxException($"Option {OptionPrefix}{option.Name} is invalid, no value has been provided");
                        }

                        switch (GetPropertyType(property).FullName)
                        {
                            case "System.Boolean": property.SetValue(syntaxVariant, true); break;
                            case "System.Int16": property.SetValue(syntaxVariant, Convert.ToInt16(option.Value)); break;
                            case "System.Int32": property.SetValue(syntaxVariant, Convert.ToInt32(option.Value)); break;
                            case "System.Int64": property.SetValue(syntaxVariant, Convert.ToInt64(option.Value)); break;
                            case "System.String": property.SetValue(syntaxVariant, option.Value); break;
                            default: throw new SyntaxException($"Option {OptionPrefix}{option.Name} of type {GetPropertyType(property).FullName} is not supported");
                        }

                        optionFound = true;
                    }
                }
                if (!optionFound)
                {
                    ((Arguments)syntaxVariant).Invalidate();
                }
            }
        }


        private List<object> SelectValidSyntaxVariants(List<object> syntaxVariants)
        {
            var selectedSyntaxVariants = new List<object>();

            foreach (var syntaxVariant in syntaxVariants)
            {
                if (syntaxVariant is not Arguments) throw new InvalidCastException($"Arguments definition class {syntaxVariant.GetType().FullName} must inherit from Arguments class");

                var variantAccepted = true;

                if (!((Arguments)syntaxVariant).Valid) variantAccepted = false;

                if (variantAccepted)
                {
                    foreach (var property in GetPropertiesWithAttribute<ArgumentAttribute>(syntaxVariant))
                    {
                        var attribute = GetPropertyAttribute<ArgumentAttribute>(property);
                        if (attribute!.Required && property.GetValue(syntaxVariant) == null) variantAccepted = false;
                    }
                }

                if (variantAccepted)
                {
                    foreach (var property in GetPropertiesWithAttribute<OptionAttribute>(syntaxVariant))
                    {
                        var attribute = GetPropertyAttribute<OptionAttribute>(property);
                        if (attribute!.Required && property.GetValue(syntaxVariant) == null) variantAccepted = false;
                    }
                }

                if (variantAccepted)
                {
                    selectedSyntaxVariants.Add(syntaxVariant);
                }
            }

            return selectedSyntaxVariants;
        }

    }
}
