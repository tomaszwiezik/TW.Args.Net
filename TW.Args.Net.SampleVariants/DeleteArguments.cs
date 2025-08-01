namespace TW.Args.Net.SampleVariants
{
    [Arguments]
    [Doc("Deletes a file.")]
    internal class DeleteArguments
    {
        [Argument(Name = "delete", Required = true, RequiredValue = "delete", Position = 0)]
        [Doc("Delete command.")]
        public string? Action { get; set; }

        [Argument(Name = "<file>", Required = true, Position = 1)]
        [Doc("File name.")]
        public string? File { get; set; }


        [Option(Name = "force", Required = false, ShortcutName = "f")]
        [Doc("Force file deletion.")]
        public bool? Force { get; set; } = false;
    }
}
