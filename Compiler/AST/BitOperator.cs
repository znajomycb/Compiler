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
                throw new ErrorException($"Inappropriate types for bit operator in line {line}", false);

            if (ll != "int" || rr != "int")
                throw new ErrorException($"Inappropriate types for bit operator in line {line}", false);

            return ll;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            throw new NotImplementedException();
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
