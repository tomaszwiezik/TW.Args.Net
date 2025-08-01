namespace TW.Args.Net.Tests
{
    [Arguments]
    [Doc("Sample option arguments")]
    internal class SampleOptionArguments
    {
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
