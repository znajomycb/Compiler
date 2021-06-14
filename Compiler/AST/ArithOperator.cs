using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum ArithType
    {
        Addition,
        Substraction,
        Multiplication,
        Division
    }

    public class ArithOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public ArithType op;

        public ArithOperator(SyntaxTree left, SyntaxTree right, ArithType op, int line)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            this.line = line;
            elementType = ElementType.Arith_op;
        }

        public override string CheckType()
        {
            string ll = left.CheckType();
            string rr = right.CheckType();

            if (ll == null || rr == null)
                throw new ErrorException($"semantic error - invalid operand type for arithmetic operator in line {line}!", false);

            if (ll == "bool" || rr == "bool")
                throw new ErrorException($"semantic error - invalid operand type for arithmetic operator in line {line}!", false);

            type = ll == rr ? ll : "double";
            return type;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string tw, t1, t2, t3, t4;

            t1 = left.GenCode();
            if (type != left.type)
            {
                t2 = Compiler.NewTemp();
                Compiler.EmitCode($"{t2} = sitofp i32 {t1} to double");
            }
            else
            {
                t2 = t1;
            }
 
            t3 = right.GenCode();
            if (type != right.type)
            {
                t4 = Compiler.NewTemp();
                Compiler.EmitCode($"{t4} = sitofp i32 {t3} to double");
            } 
            else
            {
                t4 = t3;
            }
 
            tw = Compiler.NewTemp();
            switch (op)
            {
                case ArithType.Addition:
                    Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == "int" ? "add i32" : "fadd double", t2, t4);
                    break;
                case ArithType.Substraction:
                    Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == "int" ? "sub i32" : "fsub double", t2, t4);
                    break;
                case ArithType.Multiplication:
                    Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == "int" ? "mul i32" : "fmul double", t2, t4);
                    break;
                case ArithType.Division:
                    Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == "int" ? "sdiv i32" : "fdiv double", t2, t4);
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
            left.Print(delim);
            right.Print(delim);
        }
    }
}
