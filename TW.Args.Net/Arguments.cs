namespace TW.Args.Net
{
    public class Arguments
    {
        public string SyntaxVariantName => GetType().FullName != null ?
            GetType().FullName! :
            throw new ApplicationException("Arguments definition class cannot be a generic type");


        public bool Valid { get; private set; } = true;


        public void Invalidate() => Valid = false;
    }
}
