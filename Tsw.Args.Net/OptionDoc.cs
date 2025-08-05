namespace Tsw.Args.Net
{
    internal record OptionDoc(
        string Name, 
        string ShortcutName, 
        bool Required, 
        string Text);
}
