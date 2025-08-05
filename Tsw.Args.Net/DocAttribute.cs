namespace Tsw.Args.Net
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DocAttribute(string text) : Attribute
    {
        public string Text { get; } = text;
    }
}
