using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.AST
{
    public class Read : SyntaxTree
    {
        public string ident;

        public Read(string ident, string t)
        {
            this.ident = ident;
            type = t;
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
                Compiler.EmitCode("call i32 (i8*, ...) @scanf(i8* bitcast ([3 x i8]* @readInt to i8*), i32* %{0}$)", ident);
            //} 
            //else
            //{
            //    Compiler.EmitCode($"%{ident}$ = alloca double");
            //    Compiler.EmitCode("call i32 (i8*, ...) @scanf(i8* bitcast ([4 x i8]* @readDouble to i8*), double* %{0}$)", ident);
            //    Compiler.EmitCode("%{0} = load double, double* %{0}$", ident);
            //}
            return null;
        }

        public override void Print()
        {
            Console.WriteLine(type);
            Console.WriteLine("\t Ident: " + ident);
        }
    }
}
