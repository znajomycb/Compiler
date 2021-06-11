using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Declar : SyntaxTree
    {
        public string name;

        public Declar(string type, string name)
        {
            this.type = type;
            this.name = name;
            elementType = ElementType.Declar;
        }

        public override string CheckType()
        {
            string tt = Compiler.GetIdentType(name);
            if (tt != null)
                throw new ErrorException($"Variable already declared", false);

            return type;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            string tt = Compiler.ToLLVMType(type);
            Compiler.EmitCode("%{0}$ = alloca {1}", name, tt);
            return null;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            Console.WriteLine(delim + "type: " + type);
            Console.WriteLine(delim + "name: " + name);
        }
    }
}
