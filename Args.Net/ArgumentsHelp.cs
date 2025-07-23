using System.Reflection;
using System.Text;

namespace Args.Net
{
    public class ArgumentsHelp : ArgumentsDefinition
    {
        public string GetText(bool showSyntaxHelp = true, string? errorMessage = null)
        {
            StringBuilder text = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                text.AppendLine($"Error: {errorMessage}");
            }
            if (showSyntaxHelp)
            {
                text.AppendLine($"{GetExecutableName()}, (C) Tomasz Wiezik");
                text.AppendLine();

                text.AppendLine("SYNTAX:");
                text.AppendLine(string.Empty);

                var syntaxDoc = BuildSyntaxDoc();
                var formatter = new TextFormatter();

                var argumentNameColumnWidth = GetMaxArgumentLength(syntaxDoc);
                var optionShortcutNameColumnWidth = GetMaxOptionShortcutNameLength(syntaxDoc);
                var optionNameColumnWidth = GetMaxOptionNameLength(syntaxDoc);
                if (argumentNameColumnWidth > optionShortcutNameColumnWidth + formatter.Spacing + optionNameColumnWidth)
                {
                    optionNameColumnWidth = argumentNameColumnWidth - formatter.Spacing - optionNameColumnWidth;
                }
                if (argumentNameColumnWidth < optionShortcutNameColumnWidth + formatter.Spacing + optionNameColumnWidth)
                {
                    argumentNameColumnWidth = optionShortcutNameColumnWidth + formatter.Spacing + optionNameColumnWidth;
                }

                foreach (var doc in syntaxDoc)
                {
                    text.AppendLine($"{GetExecutableName()} {doc.FullSyntax}");
                    text.AppendLine();
                    text.AppendLine(formatter.ToColumns([0], [doc.Text]));
                    text.AppendLine();

                    foreach (var argument in doc.Arguments.FindAll(x => !x.FixedValue))
                    {
                        text.AppendLine(formatter.ToColumns([argumentNameColumnWidth, 0], [argument.Name, argument.Text]));
                        text.AppendLine();
                    }

                    foreach (var option in doc.Options)
                    {
                        text.AppendLine(formatter.ToColumns([optionShortcutNameColumnWidth, optionNameColumnWidth, 0], [option.ShortcutName, option.Name, option.Text]));
                        text.AppendLine();
                    }
                }
            }

            return text.ToString().Trim();
        }


        private List<SyntaxVariantDoc> BuildSyntaxDoc()
        {
            var syntaxDoc = new List<SyntaxVariantDoc>();

            foreach (var variant in InstantiateSyntaxVariants())
            {
                if (variant != null)
                {
                    var doc = GetClassAttribute<DocAttribute>(variant);
                    if (doc == null) throw new ApplicationException($"Syntax variant {((Arguments)variant).SyntaxVariantName} has no [Doc] attribute");

                    var arguments = BuildArgumentsDoc(GetPropertiesWithAttribute<ArgumentAttribute>(variant));
                    var options = BuildOptionsDoc(GetPropertiesWithAttribute<OptionAttribute>(variant));

                    syntaxDoc.Add(new SyntaxVariantDoc(
                        text: doc.Text,
                        syntaxVariantName: ((Arguments)variant).SyntaxVariantName,
                        fullSyntax: CreateFullSyntax(arguments, options),
                        arguments: arguments,
                        options: options));
                }
            }
            syntaxDoc.Sort((x, y) => x.FullSyntax.CompareTo(y.FullSyntax));

            return syntaxDoc;
        }


        private List<ArgumentDoc> BuildArgumentsDoc(IEnumerable<PropertyInfo> properties)
        {
            var argumentsDoc = new List<ArgumentDoc>();

            foreach (var property in properties)
            {
                var argument = GetPropertyAttribute<ArgumentAttribute>(property);
                if (argument == null) throw new ApplicationException($"Property {property.Name} has no [Argument] attribute");

                var doc = GetPropertyAttribute<DocAttribute>(property);
                if (doc == null) throw new ApplicationException($"Property {property.Name} has no [Doc] attribute");
                if (string.IsNullOrWhiteSpace(argument.Name) && string.IsNullOrWhiteSpace(argument.RequiredValue)) throw new ApplicationException($"Porperty {property.Name} has neither Name, nor RequiredValue specified in [Argument] attribute");

                argumentsDoc.Add(new ArgumentDoc(
                    name: (string.IsNullOrWhiteSpace(argument!.RequiredValue) ? argument.Name : argument.RequiredValue)!,
                    position: argument.Position,
                    required: argument.Required,
                    text: doc.Text,
                    fixedValue: !string.IsNullOrWhiteSpace(argument.RequiredValue)));
            }
            argumentsDoc.Sort((x, y) => x.Position - y.Position);

            return argumentsDoc;
        }


        private List<OptionDoc> BuildOptionsDoc(IEnumerable<PropertyInfo> properties)
        {
            var optionsDoc = new List<OptionDoc>();

            foreach (var property in properties)
            {
                var option = GetPropertyAttribute<OptionAttribute>(property);
                if (option == null) throw new ApplicationException($"Property {property.Name} has no [Option] attribute");
                if (string.IsNullOrWhiteSpace(option.Name)) throw new ApplicationException($"Porperty {property.Name} has no Name specified in [Option] attribute");

                var doc = GetPropertyAttribute<DocAttribute>(property);
                if (doc == null) throw new ApplicationException($"Property {property.Name} has no [Doc] attribute");

                var propertyType = GetPropertyType(property);

                optionsDoc.Add(new OptionDoc(
                    name: propertyType.FullName == "System.Boolean" ? option.Name : $"{option.Name}=<{propertyType.Name.ToLower()}>",
                    shortcutName: string.IsNullOrWhiteSpace(option.ShortcutName) ? string.Empty : option.ShortcutName,
                    required: option.Required,
                    text: doc.Text));
            }
            optionsDoc.Sort((x, y) => x.Name.CompareTo(y.Name));

            return optionsDoc;
        }


        private string CreateFullSyntax(List<ArgumentDoc> arguments, List<OptionDoc> options)
        {
            var fullSyntax = string.Empty;

            foreach (var argument in arguments)
            {
                fullSyntax += argument.Required ? $" {argument.Name}" : $" [{argument.Name}]";
            }
            foreach (var option in options)
            {
                fullSyntax += option.Required ? $" {option.Name}" : $" [{option.Name}]";
            }

            return fullSyntax.Trim();
        }


        private int GetMaxArgumentLength(List<SyntaxVariantDoc> syntaxDoc)
        {
            int maxLength = -1;
            foreach (var syntax in syntaxDoc)
            {
                foreach(var argument in syntax.Arguments)
                {
                    if (argument.Name.Length > maxLength) maxLength = argument.Name.Length;
                }
            }
            return maxLength;
        }

        private int GetMaxOptionShortcutNameLength(List<SyntaxVariantDoc> syntaxDoc)
        {
            int maxLength = -1;
            foreach (var syntax in syntaxDoc)
            {
                foreach (var option in syntax.Options)
                {
                    if (option.ShortcutName.Length > maxLength) maxLength = option.ShortcutName.Length;
                }
            }
            return maxLength;
        }

        private int GetMaxOptionNameLength(List<SyntaxVariantDoc> syntaxDoc)
        {
            int maxLength = -1;
            foreach (var syntax in syntaxDoc)
            {
                foreach (var option in syntax.Options)
                {
                    if (option.Name.Length > maxLength) maxLength = option.Name.Length;
                }
            }
            return maxLength;
        }

    }
}
