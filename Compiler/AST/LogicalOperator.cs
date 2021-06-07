using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum LogicalType
    {
        Or,
        And
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
