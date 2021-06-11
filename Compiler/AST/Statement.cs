using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Statement : SyntaxTree
    {
        public List<SyntaxTree> children;

        public Statement()
        {
            this.children = new List<SyntaxTree>();
            elementType = ElementType.Statements;
        }

        public Statement(List<SyntaxTree> sts)
        {
            this.children = sts;
            elementType = ElementType.Statements;
        }

        public override string CheckType()
        {
            return type;
        }

        public override int Count()
        {
            return 3;
        }

        public override string GenCode()
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].GenCode();
            }
            return null;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Print(delim);
            }
        }
    }
}
