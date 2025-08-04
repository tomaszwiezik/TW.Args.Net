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
        public ArgumentsParser(Assembly? assembly = null, ParserOptions? options = null) 
            : base(assembly, options?.ApplicationName)
        {
            Options.Merge(options);
        }


        public ParserOptions Options { get; private set; } = new ParserOptions()
        {
            OptionPrefix = "--",
            OptionShortcutPrefix = "-"
        };


        /// <summary>
        /// Parses arguments, select the matching one from defined syntax variants and pass them to the handler, or throw SyntaxException if none is matched.
        /// </summary>
        /// <param name="args">Arguments to parse.</param>
        /// <param name="handler">A function which receives arguments class selected from syntax variant.</param>
        /// <param name="onHelpRequested">An optional handler executed when a request for help is detected (--help or -h option is ued).</param>
        /// <param name="onSyntaxError">An optional handler executed when syntax problems are detected. The input parameter is the syntax error message.</param>
        /// <param name="onError">An optional handler executed when an exception is thrown in the application. The input parameter is the exception.</param>
        /// <returns>Returns 0 if arguments were correctly parsed and processed, 1 when syntax problems are detected, and 2 in all other cases.</returns>
        public int Run(string[] args, Func<object, int> handler, Func<int>? onHelpRequested = null, Func<string, int>? onSyntaxError = null, Func<Exception, int>? onError = null) =>
            Run(() => handler(Parse(args)), onHelpRequested, onSyntaxError, onError);


        /// <summary>
        /// Parse arguments and match them with arguments class of type T, then invoke the handler. Throw SyntaxException if arguments cannot be properly matched.
        /// </summary>
        /// <typeparam name="T">Argument definition class.</typeparam>
        /// <param name="args">Arguments to parse.</param>
        /// <param name="handler">A function which receives arguments class.</param>
        /// <param name="onHelpRequested">An optional handler executed when a request for help is detected (--help or -h option is ued).</param>
        /// <param name="onSyntaxError">An optional handler executed when syntax problems are detected. The input parameter is the syntax error message.</param>
        /// <param name="onError">An optional handler executed when an exception is thrown in the application. The input parameter is the exception.</param>
        /// <returns></returns>
        public int Run<T>(string[] args, Func<T, int> handler, Func<int>? onHelpRequested = null, Func<string, int>? onSyntaxError = null, Func<Exception, int>? onError = null) where T : class =>
            Run(() => handler(Parse<T>(args)), onHelpRequested, onSyntaxError, onError);


        private int Run(Func<int> handler, Func<int>? onHelpRequested = null, Func<string, int>? onSyntaxError = null, Func<Exception, int>? onError = null)
        {
            try
            {
                return handler();
            }
            catch (HelpRequestedException)
            {
                if (onHelpRequested != null)
                {
                    return onHelpRequested();
                }
                else
                {
                    Console.WriteLine(new ArgumentsHelp(assembly: GetArgumentDefinitionAssembly(), options: Options).GetText());
                    return 0;
                }
            }
            catch (SyntaxException ex)
            {
                if (onSyntaxError != null)
                {
                    return onSyntaxError(ex.Message);
                }
                else
                {
                    Console.WriteLine($"Syntax error: {ex.Message}");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                if (onError != null)
                {
                    return onError(ex);
                }
                else
                {
                    Console.WriteLine($"Error: {ex}");
                    return 2;
                }
            }
        }


        private object Parse(string[] args)
        {
            if (args.Length == 1 && (args[0] == $"{Options.OptionPrefix}help" || args[0] == $"{Options.OptionShortcutPrefix}h")) throw new HelpRequestedException();

            var syntaxVariants = InstantiateSyntaxVariants();
            var arguments = ExtractArguments(args);
            var options = ExtractOptions(args);
            var parsedSyntaxVariants = new List<object>();

            for (int i = 0; i < syntaxVariants.Count; i++)
            {
                var syntaxVariant = ParseOptions(options, ParseArguments(arguments, syntaxVariants[i]));
                if (syntaxVariant != null)
                {
                    parsedSyntaxVariants.Add(syntaxVariant);
                }
            }

            var selectedSyntaxVariants = SelectValidSyntaxVariants(parsedSyntaxVariants);

            return selectedSyntaxVariants.Count switch
            {
                1 => selectedSyntaxVariants[0],
                0 => throw new SyntaxException($"Missing or incorrect arguments; use {Options.OptionPrefix}help or {Options.OptionShortcutPrefix}h option to display help"),
                _ => throw new SyntaxException("Ambiguous syntax definition, more than one syntax variants match provided arguments")
            };
        }


        private T Parse<T>(string[] args) where T : class => (T)Parse(args);


        private List<string> ExtractArguments(string[] args) => args
            .ToList()
            .FindAll(x => !x.StartsWith(Options.OptionPrefix!) && !x.StartsWith(Options.OptionShortcutPrefix!));


        private List<Option> ExtractOptions(string[] args) => args
            .ToList()
            .FindAll(x => x.StartsWith(Options.OptionPrefix!) || x.StartsWith(Options.OptionShortcutPrefix!))
            .Select(x => new Option(x))
            .ToList();


        private object? ParseArguments(List<string> arguments, object? syntaxVariant)
        {
            if (syntaxVariant == null) return null;

            var properties = GetPropertiesWithAttribute<ArgumentAttribute>(syntaxVariant);
            if (arguments.Count > properties.Count()) return null;   // There are more arguments than the variant can accept

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
                if (!optionFound) return null;
            }

            return syntaxVariant;
        }


        private object? ParseOptions(List<Option> options, object? syntaxVariant)
        {
            if (syntaxVariant == null) return null;

            var properties = GetPropertiesWithAttribute<OptionAttribute>(syntaxVariant);

            foreach (var option in options)
            {
                var optionFound = false;
                foreach (var property in properties)
                {
                    var attribute = GetPropertyAttribute<OptionAttribute>(property);
                    if ($"{Options.OptionPrefix}{attribute!.Name}" == option.Name || $"{Options.OptionShortcutPrefix}{attribute!.ShortcutName}" == option.Name)
                    {
                        if (GetPropertyType(property).FullName != "System.Boolean" && !option.HasValue)
                        {
                            throw new SyntaxException($"Option {Options.OptionPrefix}{option.Name} is invalid, no value has been provided");
                        }

                        switch (GetPropertyType(property).FullName)
                        {
                            case "System.Boolean": property.SetValue(syntaxVariant, true); break;
                            case "System.Int16": property.SetValue(syntaxVariant, Convert.ToInt16(option.Value)); break;
                            case "System.Int32": property.SetValue(syntaxVariant, Convert.ToInt32(option.Value)); break;
                            case "System.Int64": property.SetValue(syntaxVariant, Convert.ToInt64(option.Value)); break;
                            case "System.String": property.SetValue(syntaxVariant, option.Value); break;
                            default: throw new SyntaxException($"Option {Options.OptionPrefix}{option.Name} of type {GetPropertyType(property).FullName} is not supported");
                        }

                        optionFound = true;
                    }
                }
                if (!optionFound) return null;
            }

            return syntaxVariant;
        }


        private List<object> SelectValidSyntaxVariants(List<object> syntaxVariants)
        {
            var selectedSyntaxVariants = new List<object>();

            foreach (var syntaxVariant in syntaxVariants)
            {
                var variantAccepted = true;

                foreach (var property in GetPropertiesWithAttribute<ArgumentAttribute>(syntaxVariant))
                {
                    var attribute = GetPropertyAttribute<ArgumentAttribute>(property);
                    if (attribute!.Required && property.GetValue(syntaxVariant) == null) variantAccepted = false;
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
