namespace TW.Args.Net
{
    internal class SyntaxDoc
    {
        public SyntaxDoc(List<SyntaxVariantDoc> documentation/*, int maxArgumentNameLength, int maxOptionNameLength, int maxOptionShortcutNameLength*/)
        {
            Documentation = documentation;
            //MaxArgumentNameLength = maxArgumentNameLength;
            //MaxOptionNameLength = maxOptionNameLength;
            //MaxOptionShortcutNameLength = maxOptionShortcutNameLength;
        }

        public List<SyntaxVariantDoc> Documentation { get; }

        //public int MaxArgumentNameLength { get; }

        //public int MaxOptionNameLength { get; }

        //public int MaxOptionShortcutNameLength { get; }

    }
}
