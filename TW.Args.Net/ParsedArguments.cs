namespace TW.Args.Net
{
    public class ParsedArguments
    {
        public ParsedArguments(object syntaxVariant)
        {
            _selectedSyntaxVariant = syntaxVariant;
        }

        private object _selectedSyntaxVariant;


        public string SyntaxVariantName => ((Arguments)_selectedSyntaxVariant).SyntaxVariantName;

        public T GetSyntaxVariant<T>() where T : class => (T)_selectedSyntaxVariant;
    }
}
