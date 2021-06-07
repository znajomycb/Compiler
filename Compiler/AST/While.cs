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
        public While(SyntaxTree left, SyntaxTree right, string t)
        {
            this.left = left;
            this.right = right;
            type = t;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string t, startlab, innerlab, endlab;
            startlab = Compiler.NewTemp();
            startlab = startlab.Remove(0, 1);
            innerlab = Compiler.NewTemp();
            innerlab = innerlab.Remove(0, 1);
            endlab = Compiler.NewTemp();
            endlab = endlab.Remove(0, 1);
            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{startlab}:");
            t = left.GenCode();
            Compiler.EmitCode($"br i1 {t}, label %{innerlab}, label %{endlab}");
            Compiler.EmitCode($"{innerlab}:");
            right.GenCode();
            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{endlab}:");
            return null;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + left.type);
            Console.WriteLine("\t" + right.type);
        }
    }
}
