using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class IfOnly : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public IfOnly(SyntaxTree left, SyntaxTree right)
        {
            this.left = left;
            this.right = right;
            elementType = ElementType.If_only;
        }

        public override string CheckType()
        {
            return null;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string t, truelab, falselab, endlab;
            t = left.GenCode();
            truelab = Compiler.NewTemp();
            truelab = truelab.Remove(0, 1);
            falselab = Compiler.NewTemp();
            falselab = falselab.Remove(0, 1);
            Compiler.EmitCode($"br i1 {t}, label %{truelab}, label %{falselab}");
            Compiler.EmitCode($"{truelab}:");
            right.GenCode();
            endlab = Compiler.NewTemp();
            endlab = endlab.Remove(0, 1);
            Compiler.EmitCode($"br label %{endlab}");
            Compiler.EmitCode($"{falselab}:");
            Compiler.EmitCode($"br label %{endlab}");
            Compiler.EmitCode($"{endlab}:");
            return null;
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
