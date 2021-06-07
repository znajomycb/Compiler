using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Statement : SyntaxTree
    {
        public List<SyntaxTree> children;

        public Statement(string t, List<SyntaxTree> sts)
        {
            type = t;
            this.children = sts;
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

        public override void Print()
        {
            Console.WriteLine(type);
            for (int i = 0; i < children.Count; i++)
            {
                Console.WriteLine("\t" + "children: " + children[i].type);
            }
        }
    }
}
