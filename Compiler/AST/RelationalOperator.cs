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

        public RelationalOperator(SyntaxTree left, SyntaxTree right, RelationalType op, int line)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            this.line = line;
            elementType = ElementType.Rel_op;
        }

        public override string CheckType()
        {
            string ll = left.CheckType();
            string rr = right.CheckType();

            if (ll == null || rr == null) throw new ErrorException($"Semantic error in line {line}", false);

            switch (op)
            {
                case RelationalType.Equal:
                case RelationalType.NotEqual:
                    if (ll != rr) throw new ErrorException($"Semantic error in line {line}", false);
                    break;
                case RelationalType.Greater:
                case RelationalType.GreaterOrEqual:
                case RelationalType.Less:
                case RelationalType.LessOrEqual:
                    if (ll != rr) throw new ErrorException($"Semantic error in line {line}", false);
                    if (ll == "bool") throw new ErrorException($"Semantic error in line {line}", false);
                    break;
                default:
                    throw new ErrorException($"Semantic error in line {line}", false);
            }

            return ll;
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

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            left.Print(delim);
            right.Print(delim);
        }
    }
}
