namespace TW.Args.Net.Sample.InputArguments
{
    [Arguments]
    [Doc("LoadDataArguments descriptive documentation for help purposes. It can be long and span across multiple lines in a console application.")]
    internal class LoadDataArguments : Arguments
    {
        [Argument(Required = true, RequiredValue = "load", Position = 0)]
        [Doc("Load command")]
        public string? LoadCommand { get; set; }

        [Argument(Required = true, RequiredValue = "data", Position = 1)]
        [Doc("Data command")]
        public string? DataCommand { get; set; }

        [Argument(Name = "<file>", Required = true, Position = 2)]
        [Doc("File name containing data to load.")]
        public string? FileName { get; set; }

        [Option(Name = "option1")]
        [Doc("Option 1. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")]
        public bool? Option1 { get; set; } = false;

        [Option(Name = "option2", ShortcutName = "o2", Required = true)]
        [Doc("Option 2. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")]
        public int? Option2 { get; set; }
    }
}
