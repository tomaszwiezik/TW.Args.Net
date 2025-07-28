using TW.Args.Net.Sample.InputArguments;

namespace TW.Args.Net.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //args = "load data 555 --option1 -o2=6".Split(' ');
                args = "--help".Split(' ');
                //args = "-h".Split(' ');
                //args = "".Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var arguments = new ArgumentsParser().Parse(args);
                var arguments2 = new ArgumentsParser().Parse<LoadDataArguments>(args);

                Console.WriteLine(arguments.SyntaxVariantName);

                var variantArguments = arguments.GetSyntaxVariant<LoadDataArguments>();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(new ArgumentsHelp().GetText(showSyntaxHelp: string.IsNullOrWhiteSpace(ex.Message), errorMessage: ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
