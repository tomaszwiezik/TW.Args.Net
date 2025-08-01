namespace TW.Args.Net
{
    internal class TextFormatter
    {
        public TextFormatter(int maxWidth = 79, int intendation = 4, int spacing = 2)
        {
            MaxWidth = maxWidth;
            Intendation = intendation;
            Spacing = spacing;
        }

        public int MaxWidth { get; }
        public int Intendation { get; }
        public int Spacing { get; }


        public string ToColumns(List<int> width, List<string> content)
        {
            var layout = new List<(int index, int width)>();
            var index = Intendation;

            for (int i = 0; i < width.Count; i++)
            {
                layout.Add((index, width[i] == 0 ? MaxWidth - index : width[i]));
                index += width[i] + Spacing;
            }

            var text = new List<string>();
            for (int column = 0; column < width.Count; column++)
            {
                var columnText = BreakText(content[column], layout[column].width);
                for (int line = 0; line < columnText.Count; line++)
                {
                    if (line >= text.Count) text.Add(string.Empty);
                    text[line] = text[line].PadRight(layout[column].index, ' ') + columnText[line];
                }
            }

            return string.Join(Environment.NewLine, text);
        }

        private List<string> BreakText(string text, int width)
        {
            var result = new List<string>();
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var line = string.Empty;
            var separator = string.Empty;

            foreach (var word in words)
            {
                if (line.Length + word.Length + separator.Length <= width)
                {
                    line += separator + word;
                    separator = line.Length > 0 ? " " : string.Empty;
                }
                else
                {
                    result.Add(line);
                    line = word;
                    separator = line.Length > 0 ? " " : string.Empty;
                }
            }
            if (line.Length > 0)
            {
                result.Add(line);
            }
            return result;
        }

    }
}
