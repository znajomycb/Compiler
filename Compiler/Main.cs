using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Compiler
    {
        public static int errors = 0;
        public static List<string> source;

        // arg[0] określa plik źródłowy
        // pozostałe argumenty są ignorowane
        public static int Main(string[] args)
        {
            string file;
            FileStream fileStream;
            Console.WriteLine("Hello!");
            if (args.Length >= 1)
                file = args[0];
            else
            {
                Console.Write("\nsource file:  ");
                file = Console.ReadLine();
            }
            try
            {
                var sr = new StreamReader(file);
                string str = sr.ReadToEnd();
                sr.Close();
                source = new System.Collections.Generic.List<string>(str.Split(new string[] { "\r\n" }, System.StringSplitOptions.None));
                fileStream = new FileStream(file, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                return 1;
            }
            Scanner scanner = new Scanner(fileStream);
            Parser parser = new Parser(scanner);
            Console.WriteLine();
            parser.Parse();
            fileStream.Close();
            if (errors == 0)
            {
                
            }
            else
                Console.WriteLine($"\n  {errors} errors detected\n");
            return errors == 0 ? 0 : 2;
        }
    }
}