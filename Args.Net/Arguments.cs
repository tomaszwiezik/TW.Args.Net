namespace Args.Net
{
    public class Arguments
    {
        public List<string> UnknownOptions { get; } = new List<string>();

        public virtual string GetHelp() => throw new NotImplementedException();
    }
}
