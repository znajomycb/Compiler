using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum UnaryType
    {
        Minus,
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
                throw new ErrorException($"semantic error - invalid operand type for unary operator in line {line}!", false);

            switch (op)
            {
                case UnaryType.Minus:
                    if (ll == "bool")
                        throw new ErrorException($"semantic error - operator '-' cannot be applied in line {line}!", false);
                    break;
                case UnaryType.BitNot:
                    if (ll == "double" || ll == "bool")
                        throw new ErrorException($"semantic error - operator '~' cannot be applied in line {line}!", false);
                    break;
                case UnaryType.Not:
                    if (ll == "int" || ll == "double")
                        throw new ErrorException($"semantic error - operator '!' cannot be applied in line {line}!", false);
                    break;
                case UnaryType.ToInt:
                    ll = "int";
                    break;
                case UnaryType.ToDouble:
                    if (ll == "bool")
                        throw new ErrorException($"semantic error - cast to 'double' cannot be applied in line {line}!", false);
                    ll = "double";
                    break;
                default:
                    throw new ErrorException($"semantic error - invalid operand type for unary operator in line {line}!", false);
            }

            type = ll;
            return type;
        }

        public override int Count()
        {
            return 1;
        }

        public override string GenCode()
        {
            string tw, t2;
            
            tw = Compiler.NewTemp();
            t2 = exp.GenCode();
            
            switch (op)
            {
                case UnaryType.Minus:
                    Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == "int" ? "mul i32" : "fmul double", t2, type == "int" ? "-1" : "-1.0");
                    break;
                case UnaryType.BitNot:
                    Compiler.EmitCode("{0} = xor i32 -1, {1}", tw, t2);
                    break;
                case UnaryType.Not:
                    string truelab, falselab, endlab;
                    truelab = Compiler.NewTemp();
                    truelab = truelab.Remove(0, 1);
                    falselab = Compiler.NewTemp();
                    falselab = falselab.Remove(0, 1);

                    Compiler.EmitCode("{0}$ = alloca i1", tw);
                    Compiler.EmitCode($"br i1 {t2}, label %{truelab}, label %{falselab}");

                    Compiler.EmitCode($"{truelab}:");
                    Compiler.EmitCode("store i1 false, i1* {0}$", tw);

                    endlab = Compiler.NewTemp();
                    endlab = endlab.Remove(0, 1);
                    Compiler.EmitCode($"br label %{endlab}");

                    Compiler.EmitCode($"{falselab}:");
                    Compiler.EmitCode("store i1 true, i1* {0}$", tw);        

                    Compiler.EmitCode($"br label %{endlab}");
                    Compiler.EmitCode($"{endlab}:");
                    Compiler.EmitCode("{0} = load i1, i1* {0}$", tw);
                    break;
                case UnaryType.ToInt:
                    if (exp.type == "int")
                        tw = t2;
                    if (exp.type == "double")
                        Compiler.EmitCode("{0} = fptosi double {1} to i32", tw, t2);
                    if (exp.type == "bool")
                        Compiler.EmitCode("{0} = zext i1 {1} to i32", tw, t2);
                    break;
                case UnaryType.ToDouble:
                    if (exp.type == "int")
                        Compiler.EmitCode("{0} = sitofp i32 {1} to double", tw, t2);
                    else
                        tw = t2;
                    break;
                default:
                    throw new ErrorException($"internal gencode error", false);
            }

            return tw;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            exp.Print(delim);
        }
    }
}
