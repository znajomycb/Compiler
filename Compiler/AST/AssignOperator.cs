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

        public AssignOperator(SyntaxTree left, SyntaxTree right, int line)
        {
            this.left = left;
            this.right = right;
            this.line = line;
            elementType = ElementType.Assign_op;
        }

        public override string CheckType()
        {
            if (left.elementType != ElementType.Ident)
                throw new ErrorException($"Left argument of an assign operator is not an identifier in line {line}", false);
            //Compiler.PrintSemanticError("Left argument of an assign operator is not an identifier in line", line);

            string ll = left.CheckType();
            string rr = right.CheckType();

            if (ll == null || rr == null)
                throw new ErrorException($"Inappropriate types for an assign operator in line {line}", false);

            if (ll != rr)
                throw new ErrorException($"Types do not match for an assign operator in line {line}", false);

            return ll;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string t1, t2;

            t1 = (left as Ident).name;
            t2 = right.GenCode();

            string tt = CheckType();
            tt = Compiler.ToLLVMType(tt);

            Compiler.EmitCode("store {0} {1}, {0}* %{2}$", tt, t2, t1);
            string tw = (left as Ident).GenCode();
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
