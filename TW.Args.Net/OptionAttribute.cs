namespace TW.Args.Net
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        public string? Name { get; set; }

        public string? ShortcutName { get; set; }

        public bool Required { get; set; } = false;
    }
}
