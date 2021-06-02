using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.IO
{
    public class Read : SyntaxTree
    {
        public string ident;

        public Read(string ident, string t)
        {
            this.ident = ident;
            type = t;
        }

        public override int Count()
        {
            return 0;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("Ident: " + ident);
        }
    }
}
