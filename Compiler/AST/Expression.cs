using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Expression : SyntaxTree
    {
        public Expression(string t)
        {
            type = t;
            elementType = ElementType.Expression;
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
            return null;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            Console.WriteLine(delim + type);
        }
    }
}
