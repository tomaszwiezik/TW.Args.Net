namespace TW.Args.Net
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ArgumentAttribute : Attribute
    {
        public string? Name { get; set; }

        public bool Required { get; set; }

        public string? RequiredValue { get; set; }

        public int Position { get; set; }
    }
}
