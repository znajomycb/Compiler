using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class IfElse : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;
        public SyntaxTree els;

        public IfElse(SyntaxTree left, SyntaxTree right, SyntaxTree els)
        {
            this.left = left;
            this.right = right;
            this.els = els;
            elementType = ElementType.If_else;
        }

        public override string CheckType()
        {
            return null;
        }

        public override int Count()
        {
            return 3;
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
            els.GenCode();
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
            els.Print(delim);
        }
    }
}
