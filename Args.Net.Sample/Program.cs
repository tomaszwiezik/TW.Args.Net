using Args.Net.Sample.InputArguments;

namespace Args.Net.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new ArgumentParser();
            try
            {
                args = "load data 555 --option1 -o2=6".Split(' ');
                args = "--help".Split(' ');
                var arguments = parser.Parse(args);

                Console.WriteLine(arguments.SyntaxVariantName);

                var variantArguments = arguments.GetSyntaxVariant<LoadDataArguments>();
            }
            catch (ArgumentException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine(parser.GetHelp().ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
