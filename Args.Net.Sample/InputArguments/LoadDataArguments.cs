namespace Args.Net.Sample.InputArguments
{
    /// <summary>
    /// app load data <file> --option1 [--option2=5]
    /// </summary>
    [Arguments]
    internal class LoadDataArguments : Arguments
    {
        [Argument(Required = true, RequiredValue = "load", Position = 0)]
        public string? LoadCommand { get; set; }

        [Argument(Required = true, RequiredValue = "data", Position = 1)]
        public string? DataCommand { get; set; }

        [Argument(Required = true, Position = 2)]
        public string? FileName { get; set; }

        [Option(Name = "--option1")]
        public bool Option1 { get; set; } = false;

        [Option(Name = "--option2", ShortcutName = "-o2", Required = true)]
        public int? Option2 { get; set; }


        public override string GetHelp() => "load data <file> --option1 [--option2=5]";
    }
}
