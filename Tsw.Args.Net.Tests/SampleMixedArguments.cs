namespace Tsw.Args.Net.Tests
{
    [Arguments]
    [Doc("Sample mixed arguments")]
    internal class SampleMixedArguments
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



        [Option(Name = "boolRequired", ShortcutName = "br", Required = true)]
        [Doc("boolRequired")]
        public bool? BoolRequired { get; set; }

        [Option(Name = "stringRequired", ShortcutName = "sr", Required = true)]
        [Doc("stringRequired")]
        public string? StringRequired { get; set; }

        [Option(Name = "intRequired", ShortcutName = "ir", Required = true)]
        [Doc("intRequired")]
        public int? IntRequired { get; set; }


        [Option(Name = "boolOptional", ShortcutName = "bo")]
        [Doc("boolOptional")]
        public bool? BoolOptional { get; set; } = false;

        [Option(Name = "stringOptional", ShortcutName = "so")]
        [Doc("stringOptional")]
        public string? StringOptional { get; set; } = string.Empty;

        [Option(Name = "intOptional", ShortcutName = "io")]
        [Doc("intOptional")]
        public int? IntOptional { get; set; } = 100;
    }
}
