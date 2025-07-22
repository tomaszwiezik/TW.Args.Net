namespace Args.Net.Sample.InputArguments
{
    [Arguments]
    public class ExportDataArguments : Arguments
    {
        [Argument(Required = true, RequiredValue = "export", Position = 0)]
        public string? ExportCommand { get; set; }

        [Argument(Required = true, RequiredValue = "data", Position = 1)]
        public string? DataCommand { get; set; }


        public override string GetHelp() => "export data";
    }
}
