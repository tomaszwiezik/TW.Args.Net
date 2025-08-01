namespace TW.Args.Net.Tests
{
    [Arguments]
    [Doc("Sample positional arguments")]
    internal class SamplePositionalArguments
    {
        [Argument(Required = true, RequiredValue = "command", Position = 0)]
        [Doc("Command")]
        public string? Command { get; set; }

        [Argument(Name = "<file>", Required = true, Position = 1)]
        [Doc("File name")]
        public string? File { get; set; }

        [Argument(Name = "<output_file>", Required = false, Position = 2)]
        [Doc("Outoput file name")]
        public string? OutputFile { get; set; }

    }
}
