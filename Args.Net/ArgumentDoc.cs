namespace Args.Net
{
    public class ArgumentDoc
    {
        public ArgumentDoc(string name, int position, bool required, string text, bool fixedValue)
        { 
            Name = name;
            Position = position;
            Required = required;
            Text = text;
            FixedValue = fixedValue;
        }


        public string Name { get; }

        public int Position { get; }

        public bool Required { get; }

        public string Text { get; }

        public bool FixedValue { get; }
    }
}
