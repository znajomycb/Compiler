using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Declar : SyntaxTree
    {
        public string name;

        public Declar(string t, string name)
        {
            type = t;
            this.name = name;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            Compiler.EmitCode("%{0}$ = alloca {1}", name, type == "int" ? "i32" : "double");
            return null;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + "name: " + name);
        }
    }
}
