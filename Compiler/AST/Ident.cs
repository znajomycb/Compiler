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

        public Ident(string name)
        {
            string type = Compiler.GetIdentType(name);
            if (type == null)
            {
                throw new ErrorException($"Variable {name} is not declared!");
            } 
            else
            {
                this.type = type;
            }
            elementType = ElementType.Ident;
            this.name = name;
            this.value = null;
        }

        public Ident(string name, string value)
        {
            string type = Compiler.GetIdentType(name);
            if (type == null)
            {
                throw new ErrorException($"Variable {name} is not declared!");
            }
            else
            {
                this.type = type;
            }
            elementType = ElementType.Ident;
            this.name = name;
            this.value = value;
        }

        public override string CheckType()
        {
            return type;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            string tw = Compiler.NewTemp();
            string tt = Compiler.ToLLVMType(type);
            Compiler.EmitCode("{0} = load {1}, {1}* %{2}$", tw, tt, name);
            return tw;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            Console.WriteLine(delim + "name: " + name);
            Console.WriteLine(delim + "type: " + type);
            Console.WriteLine(delim + "value: " + value);
        }
    }
}
