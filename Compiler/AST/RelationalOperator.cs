using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum RelationalType
    {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual
    }

    public class RelationalOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public RelationalType op;

        public RelationalOperator(SyntaxTree left, SyntaxTree right, RelationalType op, int line)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            this.line = line;
            elementType = ElementType.Rel_op;
        }

        public override string CheckType()
        {
            string ll = left.CheckType();
            string rr = right.CheckType();

            if (ll == null || rr == null) 
                throw new ErrorException($"semantic error - invalid operand type for relational operator in line {line}!", false);

            switch (op)
            {
                case RelationalType.Equal:
                    if ((ll == "bool" && rr != ll) || (rr == "bool" && ll != rr))
                        throw new ErrorException($"semantic error - operator '==' cannot be applied in line {line}!", false);
                    if (ll != rr)
                        ll = "double";
                    break;
                case RelationalType.NotEqual:
                    if ((ll == "bool" && rr != ll) || (rr == "bool" && ll != rr)) 
                        throw new ErrorException($"semantic error - operator '!=' cannot be applied in line {line}!", false);
                    if (ll != rr)
                        ll = "double";
                    break;
                case RelationalType.Greater:
                    if (ll == "bool" || rr == "bool")
                        throw new ErrorException($"semantic error - operator '>' cannot be applied in line {line}!", false);
                    if (ll != rr)
                        ll = "double";
                    break;
                case RelationalType.GreaterOrEqual:
                    if (ll == "bool" || rr == "bool")
                        throw new ErrorException($"semantic error - operator '>=' cannot be applied in line {line}!", false);
                    if (ll != rr)
                        ll = "double";
                    break;
                case RelationalType.Less:
                    if (ll == "bool" || rr == "bool")
                        throw new ErrorException($"semantic error - operator '<' cannot be applied in line {line}!", false);
                    if (ll != rr)
                        ll = "double";
                    break;
                case RelationalType.LessOrEqual:
                    if (ll == "bool" || rr == "bool")
                        throw new ErrorException($"semantic error - operator '<=' cannot be applied in line {line}!", false);
                    if (ll != rr)
                        ll = "double";
                    break;
                default:
                    throw new ErrorException($"semantic error - invalid operand type for relational operator in line {line}!", false);
            }

            type = ll;
            return "bool";
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string tw, t1, t2, t3, t4, tt;

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
            tt = Compiler.ToLLVMType(type);
            string comm;

            switch (op)
            {
                case RelationalType.Equal:
                    comm = "icmp eq";
                    if (tt == "double")
                        comm = "fcmp oeq";
                    Compiler.EmitCode("{0} = {4} {1} {2}, {3}", tw, tt, t2, t4, comm);
                    break;
                case RelationalType.NotEqual:
                    comm = "icmp ne";
                    if (tt == "double")
                        comm = "fcmp one";
                    Compiler.EmitCode("{0} = {4} {1} {2}, {3}", tw, tt, t2, t4, comm);
                    break;
                case RelationalType.Greater:
                    comm = "icmp sgt";
                    if (tt == "double")
                        comm = "fcmp ogt";
                    Compiler.EmitCode("{0} = {4} {1} {2}, {3}", tw, tt, t2, t4, comm);
                    break;
                case RelationalType.GreaterOrEqual:
                    comm = "icmp sge";
                    if (tt == "double")
                        comm = "fcmp oge";
                    Compiler.EmitCode("{0} = {4} {1} {2}, {3}", tw, tt, t2, t4, comm);
                    break;
                case RelationalType.Less:
                    comm = "icmp slt";
                    if (tt == "double")
                        comm = "fcmp olt";
                    Compiler.EmitCode("{0} = {4} {1} {2}, {3}", tw, tt, t2, t4, comm);
                    break;
                case RelationalType.LessOrEqual:
                    comm = "icmp sle";
                    if (tt == "double")
                        comm = "fcmp ole";
                    Compiler.EmitCode("{0} = {4} {1} {2}, {3}", tw, tt, t2, t4, comm);
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
