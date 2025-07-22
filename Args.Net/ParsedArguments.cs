namespace Args.Net
{
    public class ParsedArguments
    {
        public ParsedArguments(object variant)
        {
            _selectdVariant = variant;
        }


        private object _selectdVariant;

        public string SyntaxVariantName => _selectdVariant.GetType().Name;

        public T GetSyntaxVariant<T>() where T : class => (T)_selectdVariant;
    }
}
