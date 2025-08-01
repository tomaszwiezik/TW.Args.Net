namespace TW.Args.Net
{
    public record OptionDoc(
        string Name, 
        string ShortcutName, 
        bool Required, 
        string Text);
}
