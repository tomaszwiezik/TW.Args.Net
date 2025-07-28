using System.Reflection;

namespace TW.Args.Net
{
    internal class SyntaxDocBuilder : ArgumentsDefinition
    {
        public SyntaxDoc Build()
        {
            return new SyntaxDoc(BuildSyntaxDoc());
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

    }
}
