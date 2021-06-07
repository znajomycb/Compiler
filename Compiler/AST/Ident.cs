using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Ident : SyntaxTree
    {
        public string name;
        public string value;

        public Ident(string t, string name)
        {
            type = t;
            this.name = name;
            this.value = null;
        }

        public Ident(string t, string name, string value)
        {
            type = t;
            this.name = name;
            this.value = value;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            string tw = Compiler.NewTemp();
            Compiler.EmitCode("{0} = load i32, i32* %{1}$", tw, name);
            return tw;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + "name: " + name);
            Console.WriteLine("\t" + "value: " + value);
        }
    }
}
