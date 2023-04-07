using System.Globalization;

namespace Flamencode;

internal class Program
{
    static void Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        if (args.Length == 0)
        {
            Console.WriteLine("You must add a path");
            //Console.WriteLine("Write -h to get help about the program");
        }
        else
        {
            /*string code = File.ReadAllText(args[0]);
            var p = new Prueba(code);
            p.Run();
            */
            
            using Stream inputStream = new ConsoleInputReader();
            using Stream outputStream = File.Create("out.txt");//Console.OpenStandardOutput();
            using Stream errorStream = Console.OpenStandardError();

            string code = File.ReadAllText(args[0]);
            Interpreter interpreter = new Interpreter(code, inputStream, outputStream, errorStream);
            interpreter.Run();
        }
    }
}