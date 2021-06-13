using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum LogicalType
    {
        Or,
        And
    }

    public class LogicalOperator : SyntaxTree
    {
        public SyntaxTree left;
        public SyntaxTree right;

        public LogicalType op;
        public LogicalOperator(SyntaxTree left, SyntaxTree right, LogicalType op, int line)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            this.line = line;
            elementType = ElementType.Logical_op;
        }

        public override string CheckType()
        {
            string ll = left.CheckType();
            string rr = right.CheckType();

            if (ll == null || rr == null)
                throw new ErrorException($"Inappropriate types for logical operator in line {line}", false);

            if (ll != "bool" || rr != "bool")
                throw new ErrorException($"Inappropriate types for logical operator in line {line}", false);

            type = ll;
            return type;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string ll, rr, t1, t2;
            t1 = left.GenCode();
            t2 = right.GenCode();

            string trueLeft, falseLeft, endLeft;
            trueLeft = Compiler.NewTemp();
            trueLeft = trueLeft.Remove(0, 1);

            falseLeft = Compiler.NewTemp();
            falseLeft = falseLeft.Remove(0, 1);

            endLeft = Compiler.NewTemp();
            endLeft = endLeft.Remove(0, 1);

            string trueRight, falseRight, endRight;
            trueRight = Compiler.NewTemp();
            trueRight = trueRight.Remove(0, 1);

            falseRight = Compiler.NewTemp();
            falseRight = falseRight.Remove(0, 1);

            endRight = Compiler.NewTemp();
            endRight = endRight.Remove(0, 1);

            string res = Compiler.NewTemp();
            Compiler.EmitCode("{0} = alloca i1", res);

            switch (op)
            {
                case LogicalType.Or:
                    ll = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = icmp eq i1 false, {1}", ll, t1);
                    Compiler.EmitCode($"br i1 {ll}, label %{trueLeft}, label %{falseLeft}");

                    Compiler.EmitCode($"{trueLeft}:");              // left is false
                    rr = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = icmp eq i1 true, {1}", rr, t2);
                    Compiler.EmitCode($"br i1 {rr}, label %{trueRight}, label %{falseRight}");

                    Compiler.EmitCode($"{trueRight}:");             //left is false, right is true
                    Compiler.EmitCode("store i1 true, i1* {0}", res);
                    Compiler.EmitCode($"br label %{endRight}");
                    
                    Compiler.EmitCode($"{falseRight}:");            // left is false, right is false
                    Compiler.EmitCode("store i1 false, i1* {0}", res);
                    Compiler.EmitCode($"br label %{endRight}");

                    Compiler.EmitCode($"{endRight}:");
                    Compiler.EmitCode($"br label %{endLeft}");
        
                    Compiler.EmitCode($"{falseLeft}:");             //left is true
                    Compiler.EmitCode("store i1 true, i1* {0}", res);

                    Compiler.EmitCode($"br label %{endLeft}");
                    Compiler.EmitCode($"{endLeft}:");
                    break;
                case LogicalType.And:
                    ll = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = icmp eq i1 true, {1}", ll, t1);
                    Compiler.EmitCode($"br i1 {ll}, label %{trueLeft}, label %{falseLeft}");
                    
                    Compiler.EmitCode($"{trueLeft}:");              //left is true
                    rr = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = icmp eq i1 true, {1}", rr, t2);
                    Compiler.EmitCode($"br i1 {rr}, label %{trueRight}, label %{falseRight}");
                    
                    Compiler.EmitCode($"{trueRight}:");             //left is true, right is true
                    Compiler.EmitCode("store i1 true, i1* {0}", res);
                    Compiler.EmitCode($"br label %{endRight}");

                    Compiler.EmitCode($"{falseRight}:");            //left is true, right is false
                    Compiler.EmitCode("store i1 false, i1* {0}", res);
                    Compiler.EmitCode($"br label %{endRight}");

                    Compiler.EmitCode($"{endRight}:");
                    Compiler.EmitCode($"br label %{endLeft}");

                    Compiler.EmitCode($"{falseLeft}:");             //left is false
                    Compiler.EmitCode("store i1 false, i1* {0}", res);

                    Compiler.EmitCode($"br label %{endLeft}");
                    Compiler.EmitCode($"{endLeft}:");
                    break;
                default:
                    break;
            }

            return res;
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
