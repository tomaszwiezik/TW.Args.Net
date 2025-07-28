namespace TW.Args.Net
{
    internal class SyntaxVariantDoc
    {
        public SyntaxVariantDoc(string text, string syntaxVariantName, string fullSyntax, List<ArgumentDoc> arguments, List<OptionDoc> options)
        {
            Text = text;
            SyntaxVariantName = syntaxVariantName;
            FullSyntax = fullSyntax;
            Arguments = arguments;
            Options = options;
        }

        public string Text { get; }

        public string SyntaxVariantName { get; }

        public string FullSyntax { get; }

        public List<ArgumentDoc> Arguments { get; }

        public List<OptionDoc> Options { get; }
    }
}
