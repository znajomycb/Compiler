%namespace Compiler

%union
{
    public string val;
    public char type;
}

%token Program Eof Return
%token If Else
%token While
%token Read Write
%token IntKeyword DoubleKeyword BoolKeyword IntHex
%token True False
%token Assign
%token Plus Minus Multiplies Divides
%token Or And BitOr BitAnd Not BitNot
%token Equal NotEqual Greater GreaterOrEqual Less LessOrEqual
%token OpenRound CloseRound OpenCurly CloseCurly
%token Comma SemiColon
%token <val> Ident IntNumber DoubleNumber
%token Error

%type <type> block body
%type <type> inst inst_s inst_block read write
%type <type> declar type names

%%

start   
    :   Program block Eof
        {
            Console.WriteLine("Here is program!");
            YYACCEPT;
        }
    ;

block   
    :   OpenCurly body CloseCurly
        {
            Console.WriteLine("Here is block!");
            Console.WriteLine("Compilation successful\n");
            YYACCEPT;
        }
    |   OpenCurly CloseCurly
        {
            Console.WriteLine("Here is empty block");
            Console.WriteLine("Compilation successful\n");
            YYACCEPT;
        }
    ;

body    
    :   body declar
    |   body inst
    |   declar
    |   inst
    ;

/* Declaration */

declar
    :   type names SemiColon
    ;

type
    :   IntKeyword  { Console.WriteLine("Integer declar here"); }
    |   DoubleKeyword { Console.WriteLine("Double declar here"); }
    |   BoolKeyword { Console.WriteLine("Bool declar here"); }
    |   IntHex
    ;

names
    :   names Comma Ident { Console.WriteLine("Many"); }
    |   Ident { Console.WriteLine("Only one"); }
    ;

/* Instruction */

inst
    :   inst_block
    |   read
    |   write
    ;

inst_s
    : inst_s inst
    | inst
    ;

inst_block
    :   OpenCurly inst_s CloseCurly { Console.WriteLine("inst block"); }
    |   OpenCurly CloseCurly { Console.WriteLine("Empty inst block"); }
    ;

read    
    :   Read Ident SemiColon
        {
            Console.WriteLine("Reading identifier!");
        }
    ;

write   
    :   Write Ident SemiColon
        {
            Console.WriteLine("Writing identifier!");
        }
    ;

%%

public Parser(Scanner scanner) : base(scanner) { }