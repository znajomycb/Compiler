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
%token Or And Not BitOr BitAnd BitNot
%token Equal NotEqual Greater GreaterOrEqual Less LessOrEqual
%token OpenRound CloseRound OpenCurly CloseCurly
%token Comma SemiColon
%token <val> Ident IntNumber DoubleNumber
%token Error

%type <type> block body
%type <type> inst inst_s inst_block 
%type <type> /*if while*/ read write return
%type <type> declar type names
%type <type> exp logic relat addit multip bit unary factor
%type <type> statement instaa open_statement closed_statement

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
            Console.WriteLine("Here is the main block!");
            Console.WriteLine("Compilation successful\n");
            YYACCEPT;
        }
    |   OpenCurly CloseCurly
        {
            Console.WriteLine("Here is the empty main block");
            Console.WriteLine("Compilation successful\n");
            YYACCEPT;
        }
    ;

body    
    :   body declar
    |   body instaa
    |   declar
    |   instaa
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
    /*|   while*/
    /*|   if*/
    |   return
    |   read
    |   write
    |   exp SemiColon
    ;

instaa
    :   statement
    ;

/*inst_s
    : inst_s inst
    | inst
    ;*/

inst_s
    : inst_s statement
    | statement
    ;

inst_block
    :   OpenCurly inst_s CloseCurly { Console.WriteLine("inst block"); }
    |   OpenCurly CloseCurly { Console.WriteLine("Empty inst block"); }
    ;

statement
    :   open_statement { Console.WriteLine("Open statement"); }
    |   closed_statement { Console.WriteLine("Closed statement"); }
    ;

open_statement 
    :   If OpenRound exp CloseRound statement { Console.WriteLine("Just if"); }
    |   If OpenRound exp CloseRound closed_statement Else open_statement { Console.WriteLine("Open if"); }
    |   While OpenRound exp CloseRound open_statement  { Console.WriteLine("Open while"); }
    ;

closed_statement
    :   simple_statement
    |   If OpenRound exp CloseRound closed_statement Else closed_statement { Console.WriteLine("Closed if"); }
    |   While OpenRound exp CloseRound closed_statement { Console.WriteLine("Closed while"); }
    ;

simple_statement
    :   inst
    ;

/*if
    :   If OpenRound exp CloseRound inst { Console.WriteLine("If is here"); }
    |   If OpenRound exp CloseRound inst Else inst { Console.WriteLine("if with else is here"); }
    ;

while 
    :   While OpenRound exp CloseRound statement { Console.WriteLine("While is here"); }
    ;*/

read    
    :   Read Ident SemiColon
        {
            Console.WriteLine("Reading identifier!");
        }
    ;

write   
    :   Write exp SemiColon
        {
            Console.WriteLine("Writing expression!");
        }
    ;

return
    :   Return SemiColon { Console.WriteLine("Return is here"); }
    ;

exp 
    :   logic Assign exp { Console.WriteLine("Here is assignment"); }
    |   logic
    ;

logic
    :   logic Or relat
    |   logic And relat
    |   relat
    ;

relat
    :   relat Equal addit
    |   relat NotEqual addit
    |   relat Greater addit
    |   relat GreaterOrEqual addit
    |   relat Less addit
    |   relat LessOrEqual addit
    |   addit
    ;

addit
    :   addit Plus multip
    |   addit Minus multip
    |   multip
    ;

multip
    :   multip Multiplies bit
    |   multip Divides bit
    |   bit
    ;

bit
    :   bit BitOr unary
    |   bit BitAnd unary
    |   unary
    ;

unary   
    :   Minus unary
    |   Plus unary
    |   BitNot unary
    |   Not unary
    |   OpenRound IntKeyword CloseRound unary
    |   OpenRound DoubleKeyword CloseRound unary
    |   factor
    ;

factor
    :   OpenRound exp CloseRound 
    |   Ident
    |   IntNumber
    |   DoubleNumber
    |   True
    |   False
    ;

%%

public Parser(Scanner scanner) : base(scanner) { }