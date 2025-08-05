namespace Tsw.Args.Net
{
    public class ParserOptions
    {
        /// <summary>
        /// Application name to display in help text.
        /// If not provided, then the name of assembly in which argument definition classes are defined is used.
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// A prefix used to recognize option in argument list.
        /// </summary>
        public string? OptionPrefix { get; set; }

        /// <summary>
        /// A prefix used to recognize option shortcut in argument list.
        /// </summary>
        public string? OptionShortcutPrefix { get; set; }


        public void Merge(ParserOptions? options)
        {
            if (options?.ApplicationName != null) ApplicationName = options.ApplicationName;
            if (options?.OptionPrefix != null) OptionPrefix = options.OptionPrefix;
            if (options?.OptionShortcutPrefix != null) OptionShortcutPrefix = options.OptionShortcutPrefix;
        }
    }
}
