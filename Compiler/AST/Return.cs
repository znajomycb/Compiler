using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Return : SyntaxTree
    {
        public Return(int line) 
        {
            this.line = line;
        }

        public override string CheckType()
        {
            return null;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            Compiler.EmitCode("ret i32 0");
            return null;
        }

        public override void Print(string delim)
        {
            Console.WriteLine($"Return in line {line}");
        }
    }
}
