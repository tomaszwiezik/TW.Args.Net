using System.Reflection;

namespace Args.Net
{
    public class ArgumentParser
    {
        public ParsedArguments Parse(string[] args, Assembly? assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }

            var arguments = args.ToList().FindAll(x => !x.StartsWith('-'));
            var options = args.ToList().FindAll(x => x.StartsWith('-'));

            var argumentVariants = assembly!.GetTypes()
                .Where(x => x.IsClass && x.GetCustomAttribute<ArgumentsAttribute>() != null)
                .Select(x => Activator.CreateInstance(x))
                .ToList();

            for (int position = 0; position < arguments.Count; position++)
            {
                var argument = arguments[position];

                for (int i = 0; i < argumentVariants.Count; i++)
                {
                    var argumentVariant = argumentVariants[i];
                    var argumentProperties = argumentVariant!.GetType().GetProperties()
                        .Where(x => Attribute.IsDefined(x, typeof(ArgumentAttribute)));

                    if (argumentProperties.Count() > arguments.Count) continue;   // There are more arguments than the variant can accept

                    foreach (var property in argumentProperties)
                    {
                        var propertyAttribute = property.GetCustomAttribute<ArgumentAttribute>();
                        if (propertyAttribute!.Position == position)
                        {
                            if (propertyAttribute.RequiredValue == argument || propertyAttribute.RequiredValue == null)
                            {
                                property.SetValue(argumentVariant, argument);
                            }
                        }
                    }
                }
            }

            foreach (var option in options)
            {
                var optionNameValue = option.Split('=', 2);
                var optionName = optionNameValue[0];
                var optionValue = optionNameValue.Length == 2 ? optionNameValue[1] : null;

                for (int i = 0; i < argumentVariants.Count; i++)
                {
                    var argumentVariant = argumentVariants[i];
                    var optionProperties = argumentVariant!.GetType().GetProperties()
                        .Where(x => Attribute.IsDefined(x, typeof(OptionAttribute)));

                    foreach (var property in optionProperties)
                    {
                        var propertyAttribute = property.GetCustomAttribute<OptionAttribute>();
                        if (propertyAttribute!.Name == optionName || propertyAttribute!.ShortcutName == optionName)
                        {
                            if (property.PropertyType.FullName != "System.Boolean" && optionValue == null)
                            {
                                throw new InvalidOperationException($"Option {option} is invalid");
                            }

                            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) == null ?
                                property.PropertyType.FullName :
                                Nullable.GetUnderlyingType(property.PropertyType)!.FullName;

                            switch (propertyType)
                            {
                                case "System.Boolean": property.SetValue(argumentVariant, true); break;
                                case "System.Int32": property.SetValue(argumentVariant, Convert.ToInt32(optionValue)); break;
                                case "System.String": property.SetValue(argumentVariant, optionValue); break;
                                default: throw new InvalidOperationException($"Option {option} is invalid");
                            }
                        }
                    }
                }
            }

            foreach (var argumentVariant in argumentVariants)
            {
                var variantAccepted = true;

                var argumentProperties = argumentVariant!.GetType().GetProperties()
                    .Where(x => Attribute.IsDefined(x, typeof(ArgumentAttribute)));

                foreach (var property in argumentProperties)
                {
                    var propertyAttribute = property.GetCustomAttribute<ArgumentAttribute>();
                    var propertyValue = property.GetValue(argumentVariant);

                    if (propertyAttribute!.Required && propertyValue == null) variantAccepted = false;
                }

                var optionProperties = argumentVariant!.GetType().GetProperties()
                    .Where(x => Attribute.IsDefined(x, typeof(OptionAttribute)));

                foreach (var property in optionProperties)
                {
                    var propertyAttribute = property.GetCustomAttribute<OptionAttribute>();
                    var propertyValue = property.GetValue(argumentVariant);

                    if (propertyAttribute!.Required && propertyValue == null) variantAccepted = false;
                }

                if (variantAccepted)
                {
                    return new ParsedArguments(argumentVariant);
                }
            }

            throw new ArgumentException();
        }

    }
}
