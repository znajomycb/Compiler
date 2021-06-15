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
        public static Dictionary<string, string> table = new Dictionary<string, string>();
        public static Dictionary<string, string> literal = new Dictionary<string, string>();

        private static int nr;
        public static int errors = 0;
        private static StreamWriter sw;

        public static List<string> source;
        public static List<SyntaxTree> code = new List<SyntaxTree>();

        // arg[0] określa plik źródłowy
        // pozostałe argumenty są ignorowane
        public static int Main(string[] args)
        {
            string file;
            FileStream fileStream;

            if (args.Length >= 1)
            {
                file = args[0];
            }
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
                Console.WriteLine("\t" + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("\t" + e.Message);
            }

            fileStream.Close();
            Console.WriteLine();
            if (errors == 0)
            {
                sw = new StreamWriter(file + ".ll");
                GenCode();
                sw.Close();
                Console.Write("\n\t" + "compilation successful!");
            }
            else
            {
                Console.Write($"\n  {errors} errors detected\n");
            }
                
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

            EmitCode("@writeInt = constant [3 x i8] c\"%d\\00\"");
            EmitCode("@writeIntHex = constant [5 x i8] c\"0X%X\\00\"");
            EmitCode("@writeDouble = constant [4 x i8] c\"%lf\\00\"");
 
            EmitCode("declare i32 @printf(i8*, ...)");
            EmitCode("declare i32 @scanf(i8*, ...)");

            EmitCode("@strTrue = private constant [5 x i8] c\"True\\00\"");
            EmitCode("@strFalse = private constant [6 x i8] c\"False\\00\"");

            foreach (var x in literal)
            {
                string str = x.Key;
                int nl = 0;

                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < str.Length - 1; i++)
                {
                    if (str[i] == '\\')
                    {
                        sb.Append('\\');
                        if (i + 1 < str.Length - 1 && str[i + 1] == 'n')
                        {
                            sb.Append("0A");
                            nl++;
                            i++;
                        }
                        else
                            sb.Append("5C");
                    } 
                    else
                        sb.Append(str[i]);
                }

                string foo = sb.ToString();
                sb.Clear();

                for (int i = 0; i < foo.Length; i++)
                {
                    if (foo[i] == '\"')
                        sb.Append("\\22");
                    else
                        sb.Append(foo[i]);
                }

                string bar = "\"" + sb.ToString() + "\\00\"";
                int len = str.Length - 1 - nl;
                string tt = x.Value;

                EmitCode("{0} = private constant [{1} x i8] c{2}", tt, len, bar);
            }

            EmitCode("define i32 @main()");
            EmitCode("{");

            for (int i = 0; i < code.Count; ++i)
                code[i].GenCode();

            EmitCode("ret i32 0");
            EmitCode("}");
        }

        public static int CountSubOcc(string text, string pattern)
        {
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }

            return count;
        }

        public static string GetIdentType(string name)
        {
            if (table.ContainsKey(name))
                return table[name];

            return null;
        }

        public static string GetLiteralId(string name)
        {
            if (literal.ContainsKey(name))
                return literal[name];

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
        public ElementType elementType;
        public string type = null;
        public int line = -1;
        public abstract int Count();
        public abstract string CheckType();
        public abstract string GenCode();
        public abstract void Print(string delim);
    }

    class ErrorException : ApplicationException
    {
        public readonly bool Recovery;
        public ErrorException(bool rec = true) { ++Compiler.errors; Recovery = rec; }
        public ErrorException(string msg, bool rec = true) : base(msg) { ++Compiler.errors; Recovery = rec; }
        public ErrorException(string msg, Exception ex, bool rec = true) : base(msg, ex) { ++Compiler.errors; Recovery = rec; }
    }
}
