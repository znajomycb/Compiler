using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public enum ReadType
    {
        Decimal,
        Hexadecimal
    }
    public class Read : SyntaxTree
    {
        public string ident;
        public ReadType op;

        public Read(string ident, ReadType op, int line)
        {
            this.ident = ident;
            this.op = op;
            this.line = line;
            elementType = ElementType.Read;
        }

        public override string CheckType()
        {
            string type = Compiler.GetIdentType(ident);
            if (type == null)
                throw new ErrorException($"Undeclared variable");
            
            switch (op)
            {
                case ReadType.Decimal:
                    if (type == "bool")
                        throw new ErrorException($"Inappropriate type for read instruction in line {line}", false);
                    break;
                case ReadType.Hexadecimal:
                    if (type != "int")
                        throw new ErrorException($"Inappropriate type for read (hex) instruction in line {line}", false);
                    break;
                default:
                    break;
            }

            return type;
        }

        public override int Count()
        {
            return 0;
        }

        public override string GenCode()
        {
            //if (type == "int")
            //{
                //Compiler.EmitCode($"%{ident}$ = alloca i32");
            switch (op)
            {
                case ReadType.Decimal:
                    Compiler.EmitCode("call i32 (i8*, ...) @scanf(i8* bitcast ([3 x i8]* @readInt to i8*), i32* %{0}$)", ident);
                    break;
                case ReadType.Hexadecimal:
                    Compiler.EmitCode("call i32 (i8*, ...) @scanf(i8* bitcast ([3 x i8]* @readIntHex to i8*), i32* %{0}$)", ident);
                    break;
                default:
                    break;
            }
            
            //Compiler.EmitCode("call i32 (i8*, ...) @scanf(i8* bitcast ([3 x i8]* @readIntHex to i8*), i32* %{0}$)", ident);
            //} 
            //else
            //{
            //    Compiler.EmitCode($"%{ident}$ = alloca double");
            //    Compiler.EmitCode("call i32 (i8*, ...) @scanf(i8* bitcast ([4 x i8]* @readDouble to i8*), double* %{0}$)", ident);
            //    Compiler.EmitCode("%{0} = load double, double* %{0}$", ident);
            //}
            return null;
        }

        public override void Print(string delim)
        {
            Console.WriteLine(delim + elementType);
            delim += "\t";
            Console.WriteLine(delim + "Ident: " + ident);
        }
    }
}
