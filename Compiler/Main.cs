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

        private static StreamWriter sw;
        private static int nr;

        public static List<SyntaxTree> code = new List<SyntaxTree>();

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
            Console.WriteLine();
            if (errors == 0)
            {
                sw = new StreamWriter(file + ".ll");
                GenCode();
                sw.Close();
                Console.Write("Compilation success!");
            }
            else
                Console.Write($"\n  {errors} errors detected\n");
            return errors == 0 ? 0 : 2;
        }
        public static void EmitCode(string instr = null)
        {
            sw.WriteLine(instr);
        }
        public static void EmitCode(string instr, params object[] args)
        {
            sw.WriteLine(instr, args);
        }
        public static string NewTemp()
        {
            return string.Format($"%t{++nr}");
        }

        private static void GenCode()
        {
            EmitCode("; prolog");
            EmitCode("@readInt = constant [3 x i8] c\"%d\\00\"");
            EmitCode("@writeInt = constant [4 x i8] c\"%d\\0A\\00\"");
            EmitCode("@readDouble = constant [4 x i8] c\"%lf\\00\"");
            EmitCode("@writeDouble = constant [5 x i8] c\"%lf\\0A\\00\"");
            EmitCode("declare i32 @printf(i8*, ...)");
            EmitCode("declare i32 @scanf(i8*, ...)");
            EmitCode("define i32 @main()");
            EmitCode("{");
            for (int i = 0; i < code.Count; ++i)
            {
                code[i].GenCode();
            }
            EmitCode("ret i32 0");
            EmitCode("}");
        }
    }

    public abstract class SyntaxTree
    {
        public string type;
        public abstract int Count();
        public abstract void Print();
        public abstract string GenCode();
    }

    class ErrorException : ApplicationException
    {
        public readonly bool Recovery;
        public ErrorException(bool rec = true) { ++Compiler.errors; Recovery = rec; }
        public ErrorException(string msg, bool rec = true) : base(msg) { ++Compiler.errors; Recovery = rec; }
        public ErrorException(string msg, Exception ex, bool rec = true) : base(msg, ex) { ++Compiler.errors; Recovery = rec; }
    }
}