using Args.Net.Sample.InputArguments;

namespace Args.Net.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            args = "load data 555 --option1 -o2=6".Split(' ');
            var arguments = new ArgumentParser().Parse(args);

            Console.WriteLine(arguments.SyntaxVariantName);

            var variantArguments = arguments.GetSyntaxVariant<LoadDataArguments>();
        }
    }
}
