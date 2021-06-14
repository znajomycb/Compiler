using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum BitType
    {
        Or,
        And
    }

    public class BitOperator : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;

        public BitType op;

        public BitOperator(SyntaxTree left, SyntaxTree right, BitType op, int line)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            this.line = line;
            elementType = ElementType.Bit_op;
        }

        public override string CheckType()
        {
            string ll = left.CheckType();
            string rr = right.CheckType();

            if (ll == null || rr == null)
                throw new ErrorException($"semantic error - invalid operand type for bit operator in line {line}!", false);

            if (ll != "int" || rr != "int")
            {
                if (op == BitType.Or)
                    throw new ErrorException($"semantic error - operator '|' cannot be applied in line {line}!", false);
                else
                    throw new ErrorException($"semantic error - operator '&' cannot be applied in line {line}!", false);
            }        

            type = ll;
            return type;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string ll = left.GenCode();
            string rr = right.GenCode();

            string tw = Compiler.NewTemp();
            switch (op)
            {
                case BitType.Or:
                    Compiler.EmitCode("{0} = or i32 {1}, {2}", tw, ll, rr);
                    break;
                case BitType.And:
                    Compiler.EmitCode("{0} = and i32 {1}, {2}", tw, ll, rr);
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
