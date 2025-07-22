namespace Args.Net
{
    public class Arguments
    {
        //public List<string> UnknownOptions { get; } = new List<string>();
        public bool Valid { get; private set; } = true;


        public void Invalidate() => Valid = false;

        public virtual string GetHelp() => throw new NotImplementedException();
    }
}
