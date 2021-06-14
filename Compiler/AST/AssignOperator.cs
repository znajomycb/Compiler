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
                throw new ErrorException($"semantic error - left argument of operator '=' is not an identifier in line {line}!", false);

            string ll = left.CheckType();
            string rr = right.CheckType();

            if (ll == null || rr == null)
                throw new ErrorException($"semantic error - invalid operand type for assign operator in line {line}!", false);

            if (ll != "double" && rr != ll)
                throw new ErrorException($"semantic error - operator '=' cannot be applied in line {line}!", false);

            if (ll == "double" && rr == "bool")
                throw new ErrorException($"semantic error - operator '=' cannot be applied in line {line}!", false);

            type = ll;
            return type;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string t1, t2, t3;

            t1 = (left as Ident).name;
            t2 = right.GenCode();
            if (type != right.type)
            {
                t3 = Compiler.NewTemp();
                Compiler.EmitCode($"{t3} = sitofp i32 {t2} to double");
            }
            else
            {
                t3 = t2;
            }

            string tt = Compiler.ToLLVMType(type);
            Compiler.EmitCode("store {0} {1}, {0}* %{2}$", tt, t3, t1);

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
