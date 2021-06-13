using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class While : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public string which;
        public While(SyntaxTree left, SyntaxTree right, string which)
        {
            this.left = left;
            this.right = right;
            this.which = which;
            elementType = ElementType.While;
        }

        public override string CheckType()
        {
            string ll = left.CheckType();
            if (ll != "bool")
                throw new ErrorException($"Expression type in 'while' must be a boolean!", false);

            type = ll;
            return type;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string startlab, innerlab, endlab;
            startlab = Compiler.NewTemp();
            startlab = startlab.Remove(0, 1);

            innerlab = Compiler.NewTemp();
            innerlab = innerlab.Remove(0, 1);

            endlab = Compiler.NewTemp();
            endlab = endlab.Remove(0, 1);

            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{startlab}:");
            string t = left.GenCode();
            Compiler.EmitCode($"br i1 {t}, label %{innerlab}, label %{endlab}");

            Compiler.EmitCode($"{innerlab}:");
            right.GenCode();
            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{endlab}:");
            return null;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType + " " + (which));
            delim += "\t";
            left.Print(delim);
            right.Print(delim);
        }
    }
}
