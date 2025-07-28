namespace TW.Args.Net
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DocAttribute : Attribute
    {
        public DocAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
