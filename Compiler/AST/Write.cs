using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum WriteType
    {
        Decimal,
        Hexadecimal,
        Literal
    }

    public class Write : SyntaxTree
    {
        public SyntaxTree exp;
        public string literal;
        public WriteType op;
        public Write(SyntaxTree exp, WriteType op, int line)
        {
            this.exp = exp;
            this.op = op;
            this.line = line;
            elementType = ElementType.Write;
        }

        public Write(string literal, int line)
        {
            this.literal = literal;
            this.op = WriteType.Literal;
            this.line = line;
            elementType = ElementType.Write;
        }

        public override string CheckType()
        {
            string tt = exp.CheckType();
            if (tt == null)
                throw new ErrorException($"Inappropriate type for write instruction in line {line}", false);
            
            switch (op)
            {
                case WriteType.Decimal:
                    break;
                case WriteType.Hexadecimal:
                    if (tt != "int")
                        throw new ErrorException($"Inappropriate type for write instruction in line {line}", false);
                    break;
                case WriteType.Literal:
                    break;
                default:
                    break;
            }

            return tt;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            string t;
            t = exp.GenCode();
            string tt = CheckType();
            tt = Compiler.ToLLVMType(tt);
            switch (tt)
            {
                case "i32":
                    if (op == WriteType.Decimal)
                        Compiler.EmitCode("call i32(i8 *, ...) @printf(i8 * bitcast([4 x i8] * @writeInt to i8 *), i32 {0})", t);
                    else
                        Compiler.EmitCode("call i32(i8 *, ...) @printf(i8 * bitcast([6 x i8] * @writeIntHex to i8 *), i32 {0})", t);
                    break;
                case "double":
                    Compiler.EmitCode("call i32(i8 *, ...) @printf(i8 * bitcast([5 x i8] * @writeDouble to i8 *), double {0})", t);
                    break;
                case "i1":
                    break;
                default:
                    break;
            }
            
            return null;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            exp.Print(delim);
        }
    }
}
