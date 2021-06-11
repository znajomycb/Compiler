using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Compiler
    {
        public static Hashtable table = new Hashtable();

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
                source = new List<string>(str.Split(new string[] { "\r\n" }, System.StringSplitOptions.None));
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
            try
            {
                parser.Parse();
            }
            catch (ErrorException e)
            {
                Console.WriteLine(e.Message + " " + e.line);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
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
            EmitCode("@readIntHex = constant [3 x i8] c\"%x\\00\"");
            EmitCode("@readDouble = constant [4 x i8] c\"%lf\\00\"");
            EmitCode("@writeInt = constant [4 x i8] c\"%d\\0A\\00\"");
            EmitCode("@writeIntHex = constant [6 x i8] c\"0X%X\\0A\\00\"");
            EmitCode("@writeDouble = constant [5 x i8] c\"%lf\\0A\\00\"");
            //EmitCode("@writeBool = constant [5 x i8] c\"%lf\\0A\\00\"");
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

        public static string GetIdentType(string name)
        {
            if (table.ContainsKey(name))
            {
                return (string)table[name];
            }
            return null;
        }

        public static string ToLLVMType(string type)
        {
            switch (type)
            {
                case "int":
                    return "i32";
                case "double":
                    return "double";
                case "bool":
                    return "i1";
                default:
                    break;
            }
            return null;
        }

        public static void PrintSemanticError(string message, int line = -1)
        {
            ++errors;
            Console.WriteLine(message + " " + line);
        }
    }

    public enum ElementType
    {
        Arith_op,
        Assign_op,
        Bit_op,
        Declar,
        Expression,
        Ident,
        If_else,
        If_only,
        Logical_op,
        Read,
        Rel_op,
        Statements,
        Unary_op,
        Variable,
        While,
        Write
    }

    public enum IdentType
    {
        Int,
        Double,
        Bool
    }

    public abstract class SyntaxTree
    {
        public string type = null;

        public int line = -1;

        public ElementType elementType;
        public abstract int Count();
        public abstract void Print(string delim);
        public abstract string CheckType();
        public abstract string GenCode();
    }

    class ErrorException : ApplicationException
    {
        public readonly bool Recovery;
        public int line = -1;
        public ErrorException(bool rec = true) { ++Compiler.errors; Recovery = rec; }
        public ErrorException(string msg, int line) : base(msg) { ++Compiler.errors; Recovery = false; this.line = line; }
        public ErrorException(string msg, bool rec = true) : base(msg) { ++Compiler.errors; Recovery = rec; }
        public ErrorException(string msg, Exception ex, bool rec = true) : base(msg, ex) { ++Compiler.errors; Recovery = rec; }
    }
}