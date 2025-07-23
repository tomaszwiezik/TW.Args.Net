namespace Args.Net.Sample.InputArguments
{
    [Arguments]
    [Doc("ExportDataArguments descriptive documentation for help purposes. It can be long and span across multiple lines in a console application.")]
    public class ExportDataArguments : Arguments
    {
        [Argument(Required = true, RequiredValue = "export", Position = 0)]
        [Doc("Export command")]
        public string? ExportCommand { get; set; }

        [Argument(Required = true, RequiredValue = "data", Position = 1)]
        [Doc("Data command")]
        public string? DataCommand { get; set; }
    }
}
