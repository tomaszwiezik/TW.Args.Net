namespace Tsw.Args.Net
{
    internal class Option
    {
        public Option(string option)
        {
            var optionNameValue = option.Split('=', 2);
            Name = optionNameValue[0];
            Value = optionNameValue.Length == 2 ? optionNameValue[1] : null;
        }

        public string Name { get; }

        public string? Value { get; }

        public bool HasValue => Value != null;
    }
}
