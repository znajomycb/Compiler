using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Variable : SyntaxTree
    {
        public string value;
 
        public Variable(string t, string value)
        {
            type = t;
            this.value = value;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            return value;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t" + "value: " + value);
        }
    }
}
