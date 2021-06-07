using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
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

        public override string GenCode()
        {
            throw new NotImplementedException();
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + exp.type);
        }
    }
}
