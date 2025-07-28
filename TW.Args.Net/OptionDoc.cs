namespace TW.Args.Net
{
    public class OptionDoc
    {
        public OptionDoc(string name, string shortcutName, bool required, string text)
        {
            Name = name;
            ShortcutName = shortcutName;
            Required = required;
            Text = text;
        }


        public string Name { get; }

        public string ShortcutName { get; }

        public bool Required { get; }

        public string Text { get; }
    }
}
