using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{

    public class Variable : SyntaxTree
    {
        public string value;
 
        public Variable(string type, string value)
        {
            this.type = type;
            this.value = value;
            elementType = ElementType.Variable;
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
            return value;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            Console.WriteLine(delim + "type: " + type);
            Console.WriteLine(delim + "value: " + value);
        }
    }
}
