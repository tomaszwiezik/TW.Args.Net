namespace TW.Args.Net
{
    public record ArgumentDoc(
        string Name, 
        int Position, 
        bool Required, 
        string Text, 
        bool FixedValue);
}
