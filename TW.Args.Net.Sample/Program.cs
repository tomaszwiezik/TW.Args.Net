using TW.Args.Net.Sample.InputArguments;

namespace TW.Args.Net.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            args = ["--help"];
            ArgumentsParser.Parse(args, (variants) =>
            {
                Console.WriteLine(variants.SyntaxVariantName);
                var Arguments = variants.GetSyntaxVariant<LoadDataArguments>();
            });


            try
            {
                args = ["--help"];
                var variants = new ArgumentsParser().Parse(args);

                Console.WriteLine(variants.SyntaxVariantName);
                var arguments = variants.GetSyntaxVariant<LoadDataArguments>();
            }
            catch (HelpRequestedException)
            {
                Console.WriteLine(new ArgumentsHelp().GetText());
            }
            catch (SyntaxException ex)
            {
                Console.WriteLine($"Syntax error: {ex.Message}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error: {ex.ToString()}");
#else
                Console.WriteLine($"Error: {ex.Message}");
#endif
            }


            args = ["load", "data", "555", "--option1", "-o2=6"];
            ArgumentsParser.Parse<LoadDataArguments>(args, (arguments) =>
            {
            });


            try
            {
                args = ["load", "data", "555", "--option1", "-o2=6"];
                var arguments1 = new ArgumentsParser().Parse<LoadDataArguments>(args);
            }
            catch (HelpRequestedException)
            {
                Console.WriteLine(new ArgumentsHelp().GetText());
            }
            catch (SyntaxException ex)
            {
                Console.WriteLine($"Syntax error: {ex.Message}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error: {ex.ToString()}");
#else
                Console.WriteLine($"Error: {ex.Message}");
#endif
            }
        }
    }
}
