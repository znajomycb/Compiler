using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.IO
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

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine(exp.type);
        }
    }
}
