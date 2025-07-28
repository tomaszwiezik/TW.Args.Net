using System.Reflection;

namespace TW.Args.Net
{
    public class ArgumentsDefinition
    {
        public ArgumentsDefinition(Assembly? assembly = null, string? executableName = null)
        {
            _assembly = assembly == null ? Assembly.GetEntryAssembly()! : assembly;
            if (_assembly == null) throw new ApplicationException("It is not possible to determine the assembly where arguments are defined");

            _executableName = executableName == null ? Path.GetFileNameWithoutExtension(_assembly.Location) : executableName;
            if (_executableName == null) throw new ApplicationException("It is not possible to determine the executable name");
        }

        private readonly Assembly _assembly;
        private readonly string _executableName;


        protected List<object?> InstantiateSyntaxVariants() => _assembly.GetTypes()
                .Where(x => x.IsClass && x.GetCustomAttribute<ArgumentsAttribute>() != null)
                .Select(x => Activator.CreateInstance(x))
                .ToList();


        protected string GetExecutableName() => _executableName;


        protected Type GetPropertyType(PropertyInfo property) => Nullable.GetUnderlyingType(property.PropertyType) != null ?
            Nullable.GetUnderlyingType(property.PropertyType)! :
            throw new ApplicationException($"{property.Name}: properties decorated with [Argument] or [Option] attributes must be nullable");


        protected IEnumerable<PropertyInfo> GetPropertiesWithAttribute<TAttr>(object instance) => instance
            .GetType()
            .GetProperties()
            .Where(x => Attribute.IsDefined(x, typeof(TAttr)));


        protected TAttr? GetClassAttribute<TAttr>(object instance) where TAttr : Attribute =>
            instance.GetType().GetCustomAttribute<TAttr>();


        protected TAttr? GetPropertyAttribute<TAttr>(PropertyInfo property) where TAttr: Attribute =>
            property.GetCustomAttribute<TAttr>();

    }
}
