namespace TW.Args.Net
{
    public class HelpRequestedException : SyntaxException
    {
        public HelpRequestedException() : base("Help is requested")
        { }
    }
}
