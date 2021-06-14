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
                throw new ErrorException($"semantic error - invalid type for 'write' instruction in line {line}!", false);
            
            switch (op)
            {
                case WriteType.Decimal:
                    break;
                case WriteType.Hexadecimal:
                    if (tt != "int")
                        throw new ErrorException($"semantic error - invalid type for 'write' instruction in line {line}!", false);
                    break;
                default:
                    break;
            }

            type = tt;
            return type;
        }

        public override int Count()
        {
            return 1;
        }

        public override string GenCode()
        {
            string t;
            switch (op)
            {
                case WriteType.Decimal:
                    t = exp.GenCode();
                    switch (type)
                    {
                        case "int":
                            Compiler.EmitCode("call i32(i8 *, ...) @printf(i8 * bitcast([3 x i8] * @writeInt to i8 *), i32 {0})", t);
                            break;
                        case "double":
                            Compiler.EmitCode("call i32(i8 *, ...) @printf(i8 * bitcast([4 x i8] * @writeDouble to i8 *), double {0})", t);
                            break;
                        case "bool":
                            string truelab, falselab, endlab;
                            truelab = Compiler.NewTemp();
                            truelab = truelab.Remove(0, 1);
                            falselab = Compiler.NewTemp();
                            falselab = falselab.Remove(0, 1);

                            string tw;
                            Compiler.EmitCode($"br i1 {t}, label %{truelab}, label %{falselab}");

                            Compiler.EmitCode($"{truelab}:");
                            tw = Compiler.NewTemp();
                            Compiler.EmitCode("{0} = getelementptr [5 x i8], [5 x i8]* @strTrue, i32 0, i32 0", tw);
                            Compiler.EmitCode("call i32 (i8 *, ...) @printf(i8* {0})", tw);

                            endlab = Compiler.NewTemp();
                            endlab = endlab.Remove(0, 1);
                            Compiler.EmitCode($"br label %{endlab}");

                            Compiler.EmitCode($"{falselab}:");
                            tw = Compiler.NewTemp();
                            Compiler.EmitCode("{0} = getelementptr [6 x i8], [6 x i8]* @strFalse, i32 0, i32 0", tw);
                            Compiler.EmitCode("call i32 (i8 *, ...) @printf(i8* {0})", tw);
                            Compiler.EmitCode($"br label %{endlab}");
                            Compiler.EmitCode($"{endlab}:");
                            break;
                        default:
                            throw new ErrorException($"internal gencode error", false);
                    }
                    break;
                case WriteType.Hexadecimal:
                    t = exp.GenCode();
                    Compiler.EmitCode("call i32(i8 *, ...) @printf(i8 * bitcast([5 x i8] * @writeIntHex to i8 *), i32 {0})", t);
                    break;
                case WriteType.Literal:
                    string ident = Compiler.GetLiteralId(literal);
                    if (ident != null)
                    {
                        string tw = Compiler.NewTemp();
                        int len = literal.Length - 1;
                        if (literal == "\"\\n\"") 
                            len = 2;
 
                        Compiler.EmitCode("{0} = getelementptr [{1} x i8], [{1} x i8]* {2}, i32 0, i32 0", tw, len, ident);
                        Compiler.EmitCode("call i32 (i8 *, ...) @printf(i8* {0})", tw);
                    }
                    break;
                default:
                    throw new ErrorException($"internal gencode error", false);
            }
            
            return null;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            if (op == WriteType.Literal)
                Console.WriteLine(delim + literal);
            else
                exp.Print(delim);
        }
    }
}
