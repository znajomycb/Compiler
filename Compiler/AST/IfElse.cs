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
            type = "If_else";
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

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + left.type);
            Console.WriteLine("\t" + right.type);
            Console.WriteLine("\t" + els.type);
        }
    }
}
