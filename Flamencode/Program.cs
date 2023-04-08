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
            string filePath = args[0];

            if (File.Exists(filePath))
            {

                string code = File.ReadAllText(filePath);
                using Stream inputStream = new ConsoleInputReader();
                using Stream outputStream = Console.OpenStandardOutput();
                using Stream errorStream = Console.OpenStandardError();

                Interpreter interpreter = new Interpreter(code, inputStream, outputStream, errorStream);
                interpreter.Run();
            } 
            else
            {
                Console.Error.WriteLine($"File {filePath} does not exist");
            }
        }
    }
}