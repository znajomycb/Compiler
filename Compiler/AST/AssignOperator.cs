using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
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

        public override string GenCode()
        {
            string t1, t2;
            //t1 = left.GenCode();
            t1 = (left as Ident).name;
            t2 = right.GenCode();
            Compiler.EmitCode("store {0} {1}, {0}* %{2}$", "i32", t2, t1);
            string tw = (left as Ident).GenCode();
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
