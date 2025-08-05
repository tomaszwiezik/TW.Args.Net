using System.Reflection;
using System.Text;

namespace Tsw.Args.Net
{
    public class ArgumentsHelp : ArgumentsDefinition
    {
        public ArgumentsHelp(Assembly? assembly = null, ParserOptions? options = null)
            : base(assembly, options?.ApplicationName)
        {
            Options.Merge(options);
        }


        public ParserOptions Options { get; private set; } = new ParserOptions()
        {
            OptionPrefix = "--",
            OptionShortcutPrefix = "-"
        };


        public string GetText()
        {
            var text = new StringBuilder();

            text.AppendLine($"{GetExecutableName()}, (C) Tomasz Wiezik");
            text.AppendLine();

            text.AppendLine("SYNTAX:");
            text.AppendLine(string.Empty);

            var syntaxDoc = new SyntaxDocBuilder(Options).Build();
            var formatter = new TextFormatter();

            GetColumsWidth(syntaxDoc, formatter,
                out int argumentNameColumnWidth,
                out int optionNameColumnWidth,
                out int optionShortcutNameColumnWidth);

            foreach (var doc in syntaxDoc.Documentation)
            {
                text.AppendLine($"{GetExecutableName()} {doc.FullSyntax}");
                text.AppendLine();
                text.AppendLine(formatter.ToColumns([0], [doc.Text]));
                text.AppendLine();

                foreach (var argument in doc.Arguments.FindAll(x => !x.FixedValue))
                {
                    text.AppendLine(formatter.ToColumns([argumentNameColumnWidth, 0], [argument.Name, argument.Text]));
                    text.AppendLine();
                }

                foreach (var option in doc.Options)
                {
                    text.AppendLine(formatter.ToColumns([optionShortcutNameColumnWidth, optionNameColumnWidth, 0], [option.ShortcutName, option.Name, option.Text]));
                    text.AppendLine();
                }
            }

            return text.ToString().Trim();
        }


        private void GetColumsWidth(SyntaxDoc syntaxDoc, TextFormatter formatter,
            out int argumentNameColumnWidth,
            out int optionNameColumnWidth,
            out int optionShortcutNameColumnWidth)
        {
            argumentNameColumnWidth = GetMaxArgumentLength(syntaxDoc.Documentation);
            optionNameColumnWidth = GetMaxOptionNameLength(syntaxDoc.Documentation);
            optionShortcutNameColumnWidth = GetMaxOptionShortcutNameLength(syntaxDoc.Documentation);
            if (argumentNameColumnWidth > optionShortcutNameColumnWidth + formatter.Spacing + optionNameColumnWidth)
            {
                optionNameColumnWidth = argumentNameColumnWidth - formatter.Spacing - optionShortcutNameColumnWidth;
            }
            if (argumentNameColumnWidth < optionShortcutNameColumnWidth + formatter.Spacing + optionNameColumnWidth)
            {
                argumentNameColumnWidth = optionShortcutNameColumnWidth + formatter.Spacing + optionNameColumnWidth;
            }
        }


        private int GetMaxArgumentLength(List<SyntaxVariantDoc> syntaxDoc)
        {
            int maxLength = -1;
            foreach (var syntax in syntaxDoc)
            {
                foreach (var argument in syntax.Arguments)
                {
                    if (argument.Name.Length > maxLength) maxLength = argument.Name.Length;
                }
            }
            return maxLength;
        }

        private int GetMaxOptionShortcutNameLength(List<SyntaxVariantDoc> syntaxDoc)
        {
            int maxLength = -1;
            foreach (var syntax in syntaxDoc)
            {
                foreach (var option in syntax.Options)
                {
                    if (option.ShortcutName.Length > maxLength) maxLength = option.ShortcutName.Length;
                }
            }
            return maxLength;
        }

        private int GetMaxOptionNameLength(List<SyntaxVariantDoc> syntaxDoc)
        {
            int maxLength = -1;
            foreach (var syntax in syntaxDoc)
            {
                foreach (var option in syntax.Options)
                {
                    if (option.Name.Length > maxLength) maxLength = option.Name.Length;
                }
            }
            return maxLength;
        }

    }
}
