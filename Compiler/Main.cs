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

    public abstract class SyntaxTree
    {
        public string type;
        public abstract int Count();
        public abstract void Print();
    }

    public class BinaryOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public int op;

        public BinaryOperator(SyntaxTree left, SyntaxTree right, int op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            type = "Binary_op";
        }

        public override int Count()
        {
            return 2;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
        }
    }

    public class UnaryOperator : SyntaxTree
    {
        public SyntaxTree exp;
        public int op;
        public UnaryOperator(SyntaxTree exp, int op)
        {
            this.exp = exp;
            this.op = op;
            type = "Unary_op";
        }

        public override int Count()
        {
            return 1;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(exp.type);
        }
    }

    public class BitOperator : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;

        public int op;

        public BitOperator(SyntaxTree left, SyntaxTree right, int op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            type = "Bit_op";
        }

        public override int Count()
        {
            return 2;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
        }
    }

    public class ArithOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public int op;

        public ArithOperator(SyntaxTree left, SyntaxTree right, int op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            type = "Arith_op";
        }
        public override int Count()
        {
            return 2;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
        }
    }

    public class RelationalOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public int op;

        public RelationalOperator(SyntaxTree left, SyntaxTree right, int op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            type = "Rel_op";
        }

        public override int Count()
        {
            return 2;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
        }
    }

    public class LogicalOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public int op;
        public LogicalOperator(SyntaxTree left, SyntaxTree right, int op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            type = "Logical_op";
        }
        public override int Count()
        {
            return 2;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
        }
    }

    public class AssignOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;
        public AssignOperator(SyntaxTree left, SyntaxTree right)
        {
            this.left = left;
            this.right = right;
            type = "Assign_op";
        }
        public override int Count()
        {
            return 2;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
        }
    }

    public class Variable : SyntaxTree
    {
        public List<string> name_s;
        public string value;
        public Variable(string t, List<string> name_s)
        {
            type = t;
            this.name_s = name_s;
        }

        public Variable(string t, string val)
        {
            type = t;
            this.value = val;
        }

        public override int Count()
        {
            return 0;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            for (int i = 0; i < name_s.Count; i++)
            {
                Console.WriteLine("Var: " + name_s[i]);
            }
        }
    }

    public class While : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;
        public While(SyntaxTree left, SyntaxTree right, string t)
        {
            this.left = left;
            this.right = right;
            type = t;
        }

        public override int Count()
        {
            return 2;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
        }
    }

    public class IfElse : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;
        public SyntaxTree els;

        public IfElse(SyntaxTree left, SyntaxTree right, SyntaxTree els)
        {
            this.left = left;
            this.right = right;
            this.els = els;
        }

        public override int Count()
        {
            return 3;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
            Console.WriteLine(els.type);
        }
    }

    public class IfOnly : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public IfOnly(SyntaxTree left, SyntaxTree right)
        {
            this.left = left;
            this.right = right;
            type = "If_only";
        }
        public override int Count()
        {
            return 2;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(left.type);
            Console.WriteLine(right.type);
        }
    }

    public class Exp : SyntaxTree
    {
        public Exp(string t)
        {
            type = t;
        }

        public override int Count()
        {
            return 0;
        }

        public override void Print()
        {
            Console.WriteLine(type);
        }
    }

    public class Statement : SyntaxTree
    {
        public List<SyntaxTree> children;

        public Statement(string t, List<SyntaxTree> sts)
        {
            type = t;
            this.children = sts;
        }

        public override int Count()
        {
            return 3;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            for (int i = 0; i < children.Count; i++)
            {
                Console.WriteLine("Chidlren: " + children[i].type);
            }
        }
    }
}