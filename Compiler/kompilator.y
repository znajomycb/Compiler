%namespace Compiler
%using Compiler.IO

%union
{
    public string val;
    public SyntaxTree node;
    public List<SyntaxTree> node_s;
    public List<string> name_s;
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

%type <node_s> block body
%type <node> inst_block
%type <node_s> inst_s
%type <node> /*if while*/ read write return
%type <node> declar type /*name_s*/
%type <node> exp logic relat addit multip bit unary factor
%type <node> statement open_statement closed_statement simple_statement
%type <name_s> name_s

%%

start   
    :   Program block Eof
        {
            Console.WriteLine("Here is a program!");
            YYACCEPT;
        }
    ;

block   
    :   OpenCurly body CloseCurly
        {
            $$ = $2;
            foreach (var x in $$) {
                Print(x, 1);
            }
            YYACCEPT;
        }
    |   OpenCurly CloseCurly
        {
            YYACCEPT;
        }
    ;

body    
    :   body declar 
        { 
            $1.Add($2);
            $$ = $1;
        }
    |   body statement 
        {
            $1.Add($2);
            $$ = $1;
        }
    |   declar
        {
            $$ = new List<SyntaxTree>();
            $$.Add($1);
        }
    |   statement 
        {
            $$ = new List<SyntaxTree>();
            $$.Add($1);
        }
    ;

/* Declaration */

declar
    :   type name_s SemiColon
        {
            $$ = new Variable("Variable", $2);
        }
    ;

type
    :   IntKeyword  
    |   DoubleKeyword 
    |   BoolKeyword 
    |   IntHex
    ;

name_s
    :   name_s Comma Ident 
        {
            $1.Add($3);
            $$ = $1;
        }
    |   Ident 
        {
            $$ = new List<string>();
            $$.Add($1);
        }
    ;

/* Instruction */

simple_statement
    :   inst_block
    /*|   while*/
    /*|   if*/
    |   return
    |   read 
    |   write
    |   exp SemiColon
    ;

/*inst_s
    :   inst_s inst 
    |   inst
    ;*/

inst_s
    :   inst_s statement
        {
            $1.Add($2);
            $$ = $1;
        }
    |   statement 
        {
            $$ = new List<SyntaxTree>();
            $$.Add($1);
        }
    ;

inst_block
    :   OpenCurly inst_s CloseCurly 
        {
            $$ = new Statement("Inst_s: ", $2);
        }
    |   OpenCurly CloseCurly 
    ;

statement
    :   open_statement 
        {  
            $$ = $1;
        }
    |   closed_statement 
        {
            $$ = $1;
        }
    ;

open_statement 
    :   If OpenRound exp CloseRound statement
        {
            $$ = new IfOnly($3, $5);
        }
    |   If OpenRound exp CloseRound closed_statement Else open_statement
        {
            $$ = new IfElse($3, $5, $7);
        }
    |   While OpenRound exp CloseRound open_statement 
        {
            var node = new While($3, $5, "While_op");
            $$ = node;
        }
    ;

closed_statement
    :   simple_statement 
        {
            $$ = $1;
        }
    |   If OpenRound exp CloseRound closed_statement Else closed_statement 
        {
            $$ = new IfElse($3, $5, $7);
        }
    |   While OpenRound exp CloseRound closed_statement 
        {
            var node = new While($3, $5, "While_cl");
            $$ = node;
        }
    ;

/*if
    :   If OpenRound exp CloseRound inst 
    |   If OpenRound exp CloseRound inst Else inst 
    ;

while 
    :   While OpenRound exp CloseRound statement 
    ;*/

read    
    :   Read Ident SemiColon
        {
            $$ = new Read($2, "Read");
        }
    ;

write   
    :   Write exp SemiColon
        {
            $$ = new Write($2, "Write");
        }
    ;

return
    :   Return SemiColon 
    ;

exp 
    :   logic Assign exp 
        {
            $$ = new AssignOperator($1, $3);
        }
    |   logic
        {
            $$ = $1;
        }
    ;

logic
    :   logic Or relat
        {
            $$ = new LogicalOperator($1, $3, 0);
        }
    |   logic And relat
        {
            $$ = new LogicalOperator($1, $3, 1);
        }
    |   relat
        {
            $$ = $1;
        }
    ;

relat
    :   relat Equal addit
        {
            $$ = new RelationalOperator($1, $3, 0);
        }
    |   relat NotEqual addit
        {
            $$ = new RelationalOperator($1, $3, 1);
        }
    |   relat Greater addit
        {
            $$ = new RelationalOperator($1, $3, 2);
        }
    |   relat GreaterOrEqual addit
        {
            $$ = new RelationalOperator($1, $3, 3);
        }
    |   relat Less addit
        {
            $$ = new RelationalOperator($1, $3, 4);
        }
    |   relat LessOrEqual addit
        {
            $$ = new RelationalOperator($1, $3, 5);
        }
    |   addit
        {
            $$ = $1;
        }
    ;

addit
    :   addit Plus multip
        {
            $$ = new ArithOperator($1, $3, 0);
        }
    |   addit Minus multip
        {
            $$ = new ArithOperator($1, $3, 1);
        }
    |   multip
        {
            $$ = $1;
        }
    ;

multip
    :   multip Multiplies bit
        {
            $$ = new ArithOperator($1, $3, 2);
        }
    |   multip Divides bit
        {
            $$ = new ArithOperator($1, $3, 3);
        }
    |   bit
        {
            $$ = $1;
        }
    ;

bit
    :   bit BitOr unary
        {
            $$ = new BitOperator($1, $3, 0);
        }
    |   bit BitAnd unary
        {
            $$ = new BitOperator($1, $3, 1);
        }
    |   unary
        {
            $$ = $1;
        }
    ;

unary   
    :   Minus unary
        {
            $$ = new UnaryOperator($2, 0);
        }
    |   Plus unary
        {
            $$ = new UnaryOperator($2, 1);
        }
    |   BitNot unary
        {
            $$ = new UnaryOperator($2, 2);
        }
    |   Not unary
        {
            $$ = new UnaryOperator($2, 3);
        }
    |   OpenRound IntKeyword CloseRound unary
        {
            
        }
    |   OpenRound DoubleKeyword CloseRound unary
        {
            
        }
    |   factor
        {
            $$ = $1;
        }
    ;

factor
    :   OpenRound exp CloseRound
        {
            $$ = $2;
        }
    |   Ident
        {
            $$ = new Variable("Id", $1);
        }
    |   IntNumber
        {
            $$ = new Variable("Int", $1);
        }
    |   DoubleNumber
        {
            $$ = new Variable("Double", $1);
        }
    |   True
        {
            $$ = new Variable("True", "1");
        }
    |   False
        {
            $$ = new Variable("False", "0");
        }
    ;

%%

    public Parser(Scanner scanner) : base(scanner) { }

    public void Print(SyntaxTree node, int level)
    {
        node.Print();
    }