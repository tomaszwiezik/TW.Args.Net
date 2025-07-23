namespace Args.Net
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
        public ParsedArguments Parse(string[] args)
        {
            if (args.Length == 1 && (args[0] == "--help" || args[0] == "-h")) throw new ArgumentException(string.Empty);

            var syntaxVariants = InstantiateSyntaxVariants();

            ParseArguments(ExtractArguments(args), ref syntaxVariants);
            ParseOptions(ExtractOptions(args), ref syntaxVariants);

            var selectedSyntaxVariants = SelectValidSyntaxVariants(syntaxVariants);

            if (selectedSyntaxVariants.Count == 1)
            {
                return new ParsedArguments(selectedSyntaxVariants[0]);
            }
            else
            {
                return selectedSyntaxVariants.Count switch
                {
                    0 => throw new ArgumentException("Provided arguments are incorrect, use --help or -h option to display help"),
                    _ => throw new ArgumentException("Ambigious syntax definition, more than one syntax variants match provided arguments")
                };
            }
        }


        //public string GetHelp()
        //{
        //    var helpText = new List<string>();
        //    var executableName = "";   // Path.GetFileNameWithoutExtension(_assembly.Location);
        //    helpText.Add($"{executableName}, (C) Tomasz Wiezik");
        //    helpText.Add(string.Empty);
        //    helpText.Add("SYNTAX:");
        //    helpText.Add(string.Empty);

        //    var syntaxVariants = InstantiateSyntaxVariants();
        //    foreach (var syntaxVariant in syntaxVariants)
        //    {
        //        helpText.Add($"{executableName} {((Arguments)syntaxVariant!).GetHelp().Trim()}");
        //        helpText.Add(string.Empty);
        //    }

        //    return string.Join(Environment.NewLine, helpText);
        //}


        private List<string> ExtractArguments(string[] args) => args.ToList().FindAll(x => !x.StartsWith('-'));


        private List<string> ExtractOptions(string[] args) => args.ToList().FindAll(x => x.StartsWith('-'));


        private void ParseArguments(List<string> arguments, ref List<object?> syntaxVariants)
        {
            for (int position = 0; position < arguments.Count; position++)
            {
                var argument = arguments[position];

                for (int i = 0; i < syntaxVariants.Count; i++)
                {
                    var syntaxVariant = syntaxVariants[i];
                    var properties = GetPropertiesWithAttribute<ArgumentAttribute>(syntaxVariant!);

                    if (properties.Count() > arguments.Count) continue;   // There are more arguments than the variant can accept

                    foreach (var property in properties)
                    {
                        var attribute = GetPropertyAttribute<ArgumentAttribute>(property);
                        if (attribute!.Position == position)
                        {
                            if (attribute.RequiredValue == argument || attribute.RequiredValue == null)
                            {
                                property.SetValue(syntaxVariant, argument);
                            }
                        }
                    }
                }
            }
        }


        private void ParseOptions(List<string> options, ref List<object?> syntaxVariants)
        {
            foreach (var option in options)
            {
                var parsedOption = new Option(option);

                for (int i = 0; i < syntaxVariants.Count; i++)
                {
                    var syntaxVariant = syntaxVariants[i];

                    var optionFound = false;
                    foreach (var property in GetPropertiesWithAttribute<OptionAttribute>(syntaxVariant!))
                    {
                        var attribute = GetPropertyAttribute<OptionAttribute>(property);
                        if (attribute!.Name == parsedOption.Name || attribute!.ShortcutName == parsedOption.Name)
                        {
                            if (GetPropertyType(property).FullName != "System.Boolean" && !parsedOption.HasValue)
                            {
                                throw new ArgumentException($"Option {option} is invalid, no value has been provided");
                            }

                            switch (GetPropertyType(property).FullName)
                            {
                                case "System.Boolean": property.SetValue(syntaxVariant, true); break;
                                case "System.Int32": property.SetValue(syntaxVariant, Convert.ToInt32(parsedOption.Value)); break;
                                case "System.String": property.SetValue(syntaxVariant, parsedOption.Value); break;
                                default: throw new ArgumentException($"Option {option} of type {GetPropertyType(property).FullName} is not supported");
                            }

                            optionFound = true;
                        }
                    }
                    if (!optionFound)
                    {
                        ((Arguments)syntaxVariant!).Invalidate();
                    }
                }
            }
        }


        private List<object> SelectValidSyntaxVariants(List<object?> syntaxVariants)
        {
            var selectedSyntaxVariants = new List<object>();

            foreach (var syntaxVariant in syntaxVariants)
            {
                var variantAccepted = true;

                if (variantAccepted)
                {
                    var typedSyntaxVariant = (Arguments)syntaxVariant!;
                    if (!typedSyntaxVariant.Valid) variantAccepted = false;
                }

                if (variantAccepted)
                {
                    foreach (var property in GetPropertiesWithAttribute<ArgumentAttribute>(syntaxVariant!))
                    {
                        var attribute = GetPropertyAttribute<ArgumentAttribute>(property);
                        if (attribute!.Required && property.GetValue(syntaxVariant) == null) variantAccepted = false;
                    }
                }

                if (variantAccepted)
                {
                    foreach (var property in GetPropertiesWithAttribute<OptionAttribute>(syntaxVariant!))
                    {
                        var attribute = GetPropertyAttribute<OptionAttribute>(property);
                        if (attribute!.Required && property.GetValue(syntaxVariant) == null) variantAccepted = false;
                    }
                }

                if (variantAccepted)
                {
                    selectedSyntaxVariants.Add(syntaxVariant!);
                }
            }

            return selectedSyntaxVariants;
        }

    }
}
