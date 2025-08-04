namespace TW.Args.Net
{
    internal record ArgumentDoc(
        string Name, 
        int Position, 
        bool Required, 
        string Text, 
        bool FixedValue);
}
