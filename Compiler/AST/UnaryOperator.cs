using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum UnaryType
    {
        Minus,
        Plus,
        BitNot,
        Not,
        ToInt,
        ToDouble
    }

    public class UnaryOperator : SyntaxTree
    {
        public SyntaxTree exp;
        public UnaryType op;
        public UnaryOperator(SyntaxTree exp, UnaryType op, int line)
        {
            this.exp = exp;
            this.op = op;
            this.line = line;
            elementType = ElementType.Unary_op;
        }

        public override string CheckType()
        {
            string ll = exp.CheckType();

            if (ll == null)
                throw new ErrorException($"Inappropriate type for unary operator in line {line}", false);

            switch (op)
            {
                case UnaryType.Plus:
                case UnaryType.Minus:
                    if (ll == "bool")
                        throw new ErrorException($"Inappropriate type for unary minus operator in line {line}", false);
                    break;
                case UnaryType.BitNot:
                    if (ll == "double" || ll == "bool")
                        throw new ErrorException($"Inappropriate type for bit negation operator in line {line}", false);
                    break;
                case UnaryType.Not:
                    if (ll == "int" || ll == "double")
                        throw new ErrorException($"Inappropriate type for logical negation operator in line {line}", false);
                    break;
                case UnaryType.ToInt:
                    break;
                case UnaryType.ToDouble:
                    if (ll == "bool")
                        throw new ErrorException($"Inappropriate type for conversation to double operator in line {line}", false);
                    break;
                default:
                    throw new ErrorException($"Inappropriate type for unary operator in line {line}", false);
            }

            return ll;
        }

        public override int Count()
        {
            return 1;
        }

        public override string GenCode()
        {
            throw new NotImplementedException();
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            exp.Print(delim);
        }
    }
}
