using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Write : SyntaxTree
    {
        public SyntaxTree exp;
        public Write(SyntaxTree exp, string t)
        {
            this.exp = exp;
            type = t;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            string t;
            t = exp.GenCode();
            //if (type == "int")
            //{
            Compiler.EmitCode("call i32(i8 *, ...) @printf(i8 * bitcast([4 x i8] * @writeInt to i8 *), i32 {0})", t);
            //}
            //else
            //{
            //    Compiler.EmitCode("call i32(i8 *, ...) @printf(i8 * bitcast([4 x i8] * @writeDouble to i8 *), double %{0})", t);
            //}
            return null;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + exp.type);
        }
    }
}
