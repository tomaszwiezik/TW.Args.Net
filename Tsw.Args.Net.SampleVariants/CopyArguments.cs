namespace Tsw.Args.Net.SampleVariants
{
    [Arguments]
    [Doc("Copies a source file to the destination.")]
    internal class CopyArguments
    {
        [Argument(Name = "copy", Required = true, RequiredValue = "copy", Position = 0)]
        [Doc("Copy command.")]
        public string? Action { get; set; }

        [Argument(Name = "<source_file>", Required = true, Position = 1)]
        [Doc("Source file name.")]
        public string? SourceFile { get; set; }

        [Argument(Name = "<destination_file>", Required = true, Position = 2)]
        [Doc("Destination file name.")]
        public string? DestinationFile { get; set; }


        [Option(Name = "quiet", Required = false, ShortcutName = "q")]
        [Doc("Don't ask for confirmation.")]
        public bool? Quiet { get; set; } = false;

        [Option(Name = "retry", Required = false)]
        [Doc("Number of retries.")]
        public int? Retry { get; set; } = 0;
    }
}
