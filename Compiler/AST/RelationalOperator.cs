using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum RelationalType
    {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual
    }
    public class RelationalOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public RelationalType op;

        public RelationalOperator(SyntaxTree left, SyntaxTree right, RelationalType op)
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

        public override string GenCode()
        {
            string tw, t1, t2, t3, t4, tt;

            t1 = left.GenCode();
            t2 = t1;
 
            t3 = right.GenCode();
            t4 = t3;
   
            tw = Compiler.NewTemp();
            tt = "i32";
            switch (op)
            {
                case RelationalType.Equal:
                    Compiler.EmitCode("{0} = icmp eq {1} {2}, {3}", tw, tt, t2, t4);
                    break;
                case RelationalType.NotEqual:
                    Compiler.EmitCode("{0} = icmp ne {1} {2}, {3}", tw, tt, t2, t4);
                    break;
                case RelationalType.Greater:
                    Compiler.EmitCode("{0} = icmp sgt {1} {2}, {3}", tw, tt, t2, t4);
                    break;
                case RelationalType.GreaterOrEqual:
                    Compiler.EmitCode("{0} = icmp sge {1} {2}, {3}", tw, tt, t2, t4);
                    break;
                case RelationalType.Less:
                    Compiler.EmitCode("{0} = icmp slt {1} {2}, {3}", tw, tt, t2, t4);
                    break;
                case RelationalType.LessOrEqual:
                    Compiler.EmitCode("{0} = icmp sle {1} {2}, {3}", tw, tt, t2, t4);
                    break;
                default:
                    throw new ErrorException($"internal gencode error", false);
            }
            return tw;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + left.type);
            Console.WriteLine("\t" + right.type);
        }
    }
}
