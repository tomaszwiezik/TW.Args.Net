
namespace Tsw.Args.Net.Sample
{
    internal class Program
    {
        static int Main(string[] args)
        {
            return new ArgumentsParser().Run<Arguments>(args, (arguments) =>
            {
                Console.WriteLine($"SourceFile = {arguments.SourceFile}, DestinationFile = {arguments.DestinationFile}, Quiet = {arguments.Quiet}, Retry = {arguments.Retry}");
                return 0;
            });
        }
    }
}
