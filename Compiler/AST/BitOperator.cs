using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum Bit
    {
        Or,
        And
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

        public override string GenCode()
        {
            throw new NotImplementedException();
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + left.type);
            Console.WriteLine("\t" + right.type);
        }
    }
}
