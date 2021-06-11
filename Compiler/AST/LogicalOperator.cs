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
        public LogicalOperator(SyntaxTree left, SyntaxTree right, LogicalType op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            elementType = ElementType.Logical_op;
        }

        public override string CheckType()
        {
            return null;
        }

        public override int Count()
        {
            return 2;
        }

        public override string GenCode()
        {
            string res = null;
            string tw, t1, t2, t3, t4, tt;

            t1 = left.GenCode();
            t2 = t1;

            t3 = right.GenCode();
            t4 = t3;

            string trueLeft, falseLeft, endLeft;
            string trueRight, falseRight, endRight;

            switch (op)
            {
                case LogicalType.Or:
                    tw = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = icmp eq i1 false, {1}", tw, t2);

                    trueLeft = Compiler.NewTemp();
                    trueLeft = trueLeft.Remove(0, 1);

                    falseLeft = Compiler.NewTemp();
                    falseLeft = falseLeft.Remove(0, 1);

                    endLeft = Compiler.NewTemp();
                    endLeft = endLeft.Remove(0, 1);

                    Compiler.EmitCode($"br i1 {tw}, label %{trueLeft}, label %{falseLeft}");
                    Compiler.EmitCode($"{trueLeft}:");

                    tt = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = icmp eq i1 true, {1}", tt, t4);

                    trueRight = Compiler.NewTemp();
                    trueRight = trueRight.Remove(0, 1);

                    falseRight = Compiler.NewTemp();
                    falseRight = falseRight.Remove(0, 1);

                    endRight = Compiler.NewTemp();
                    endRight = endRight.Remove(0, 1);

                    Compiler.EmitCode($"br i1 {tt}, label %{trueRight}, label %{falseRight}");
                    Compiler.EmitCode($"{trueRight}:");

                    //Left is false, right is true
                    res = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = alloca i1", res);
                    Compiler.EmitCode("store i1 true, i1* {0}", res);

                    Compiler.EmitCode($"br label %{endRight}");
                    Compiler.EmitCode($"{falseRight}:");

                    //Both are false
                    res = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = alloca i1", res);
                    Compiler.EmitCode("store i1 false, i1* {0}", res);

                    Compiler.EmitCode($"br label %{endRight}");
                    Compiler.EmitCode($"{endRight}:");

                    Compiler.EmitCode($"br label %{endLeft}");
                    Compiler.EmitCode($"{falseLeft}:");

                    //Left is true
                    res = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = alloca i1", res);
                    Compiler.EmitCode("store i1 true, i1* {0}", res);

                    Compiler.EmitCode($"br label %{endLeft}");
                    Compiler.EmitCode($"{endLeft}:");
                    break;
                case LogicalType.And:
                    tw = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = icmp eq i1 true, {1}", tw, t2);

                    trueLeft = Compiler.NewTemp();
                    trueLeft = trueLeft.Remove(0, 1);

                    falseLeft = Compiler.NewTemp();
                    falseLeft = falseLeft.Remove(0, 1);

                    endLeft = Compiler.NewTemp();
                    endLeft = endLeft.Remove(0, 1);

                    Compiler.EmitCode($"br i1 {tw}, label %{trueLeft}, label %{falseLeft}");
                    Compiler.EmitCode($"{trueLeft}:");

                    tt = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = icmp eq i1 true, {1}", tt, t4);

                    trueRight = Compiler.NewTemp();
                    trueRight = trueRight.Remove(0, 1);

                    falseRight = Compiler.NewTemp();
                    falseRight = falseRight.Remove(0, 1);

                    endRight = Compiler.NewTemp();
                    endRight = endRight.Remove(0, 1);

                    Compiler.EmitCode($"br i1 {tt}, label %{trueRight}, label %{falseRight}");
                    Compiler.EmitCode($"{trueRight}:");

                    //Both are true
                    res = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = alloca i1", res);
                    Compiler.EmitCode("store i1 true, i1* {0}", res);

                    Compiler.EmitCode($"br label %{endRight}");
                    Compiler.EmitCode($"{falseRight}:");

                    //Right is false
                    res = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = alloca i1", res);
                    Compiler.EmitCode("store i1 false, i1* {0}", res);

                    Compiler.EmitCode($"br label %{endRight}");
                    Compiler.EmitCode($"{endRight}:");

                    Compiler.EmitCode($"br label %{endLeft}");
                    Compiler.EmitCode($"{falseLeft}:");

                    //Left is false
                    res = Compiler.NewTemp();
                    Compiler.EmitCode("{0} = alloca i1", res);
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
