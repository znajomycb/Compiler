%namespace Compiler
%using Compiler.AST

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
%token <val> IntKeyword DoubleKeyword BoolKeyword IntHex
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
%type <node> read write return
%type <node_s> declar
%type <val> type
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
            Compiler.code = $2;
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
            foreach (var x in $2) {
                $1.Add(x);
            }
            $$ = $1;
        }
    |   body statement 
        {
            $1.Add($2);
            $$ = $1;
        }
    |   declar
        {
            $$ = $1;
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
            $$ = new List<SyntaxTree>();
            var node = $2;
            foreach (var x in node) {
                $$.Add(new Declar($1, x));
            }
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
            $$ = new RelationalOperator($1, $3, RelationalType.Equal);
        }
    |   relat NotEqual addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.NotEqual);
        }
    |   relat Greater addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.Greater);
        }
    |   relat GreaterOrEqual addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.GreaterOrEqual);
        }
    |   relat Less addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.Less);
        }
    |   relat LessOrEqual addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.LessOrEqual);
        }
    |   addit
        {
            $$ = $1;
        }
    ;

addit
    :   addit Plus multip
        {
            $$ = new ArithOperator($1, $3, ArithType.Addition);
        }
    |   addit Minus multip
        {
            $$ = new ArithOperator($1, $3, ArithType.Substraction);
        }
    |   multip
        {
            $$ = $1;
        }
    ;

multip
    :   multip Multiplies bit
        {
            $$ = new ArithOperator($1, $3, ArithType.Multiplication);
        }
    |   multip Divides bit
        {
            $$ = new ArithOperator($1, $3, ArithType.Division);
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
            $$ = new Ident("Id", $1);
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
            $$ = new Variable("Bool", "True");
        }
    |   False
        {
            $$ = new Variable("Bool", "False");
        }
    ;

%%

    public Parser(Scanner scanner) : base(scanner) { }

    public void Print(SyntaxTree node, int level)
    {
        node.Print();
    }