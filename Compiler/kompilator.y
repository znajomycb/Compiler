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
%token <val> Ident IntNumber IntNumberHex DoubleNumber Literal
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
    |   Program error_eof
        {   
            PrintSyntaxError("Syntax error in line", @2.StartLine);
            Console.WriteLine("AAAAA");
            YYABORT;
        }
    |   error_eof
        {
            PrintSyntaxError("The starting word is missing");
            Console.WriteLine("BBBBBB");
            YYABORT;
        }
    ;

block   
    :   OpenCurly body CloseCurly
        {
            $$ = $2;
            Compiler.code = $2;
            foreach (var x in $$) {
                Print(x);
            }
        }
    |   OpenCurly CloseCurly
        {}
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

declar
    :   type name_s SemiColon
        {
            $$ = new List<SyntaxTree>();
            var node = $2;
            foreach (var x in node) {
                $$.Add(new Declar($1, x));
                Compiler.table.Add(x, $1);
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

simple_statement
    :   inst_block
    |   return
    |   read 
    |   write
    |   exp SemiColon
    ;

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
            $$ = new Statement($2);
        }
    |   OpenCurly CloseCurly 
        {
            $$ = new Statement();
        }
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

read    
    :   Read Ident SemiColon
        {
            $$ = new Read($2, ReadType.Decimal, @1.StartLine);
            $$.CheckType();
        }
    |   Read Ident Comma IntHex SemiColon
        {
            $$ = new Read($2, ReadType.Hexadecimal, @1.StartLine);
            $$.CheckType();
        }
    ;

write   
    :   Write exp SemiColon
        {
            $$ = new Write($2, WriteType.Decimal, @1.StartLine);
            $$.CheckType();
        }
    |   Write exp Comma IntHex SemiColon
        {
            $$ = new Write($2, WriteType.Hexadecimal, @1.StartLine);
            $$.CheckType();
        }
    |   Write Literal SemiColon
        {
            $$ = new Write($2, @1.StartLine);
            $$.CheckType();
            string tt = Compiler.NewTemp();
            tt = "@" + tt.Substring(1);
            Compiler.literal.Add($2, tt);
        }
    ;

return
    :   Return SemiColon 
        {
            $$ = new Return(@1.StartLine);
        }
    ;

exp 
    :   logic Assign exp 
        {
            $$ = new AssignOperator($1, $3, @2.StartLine);
            $$.CheckType();
        }
    |   logic
        {
            $$ = $1;
        }
    ;

logic
    :   logic Or relat
        {
            $$ = new LogicalOperator($1, $3, LogicalType.Or, @2.StartLine);
        }
    |   logic And relat
        {
            $$ = new LogicalOperator($1, $3, LogicalType.And, @2.StartLine);
        }
    |   relat
        {
            $$ = $1;
        }
    ;

relat
    :   relat Equal addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.Equal, @2.StartLine);
            $$.CheckType();
        }
    |   relat NotEqual addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.NotEqual, @2.StartLine);
            $$.CheckType();
        }
    |   relat Greater addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.Greater, @2.StartLine);
            $$.CheckType();
        }
    |   relat GreaterOrEqual addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.GreaterOrEqual, @2.StartLine);
            $$.CheckType();
        }
    |   relat Less addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.Less, @2.StartLine);
            $$.CheckType();
        }
    |   relat LessOrEqual addit
        {
            $$ = new RelationalOperator($1, $3, RelationalType.LessOrEqual, @2.StartLine);
            $$.CheckType();
        }
    |   addit
        {
            $$ = $1;
        }
    ;

addit
    :   addit Plus multip
        {
            $$ = new ArithOperator($1, $3, ArithType.Addition, @2.StartLine);
            $$.CheckType();
        }
    |   addit Minus multip
        {
            $$ = new ArithOperator($1, $3, ArithType.Substraction, @2.StartLine);
            $$.CheckType();
        }
    |   multip
        {
            $$ = $1;
        }
    ;

multip
    :   multip Multiplies bit
        {
            $$ = new ArithOperator($1, $3, ArithType.Multiplication, @2.StartLine);
            $$.CheckType();
        }
    |   multip Divides bit
        {
            $$ = new ArithOperator($1, $3, ArithType.Division, @2.StartLine);
            $$.CheckType();
        }
    |   bit
        {
            $$ = $1;
        }
    ;

bit
    :   bit BitOr unary
        {
            $$ = new BitOperator($1, $3, BitType.Or, @2.StartLine);
            $$.CheckType();
        }
    |   bit BitAnd unary
        {
            $$ = new BitOperator($1, $3, BitType.And, @2.StartLine);
            $$.CheckType();
        }
    |   unary
        {
            $$ = $1;
        }
    ;

unary   
    :   Minus unary
        {
            $$ = new UnaryOperator($2, UnaryType.Minus, @1.StartLine);
            $$.CheckType();
        }
    |   Plus unary
        {
            $$ = new UnaryOperator($2, UnaryType.Plus, @1.StartLine);
            $$.CheckType();
        }
    |   BitNot unary
        {
            $$ = new UnaryOperator($2, UnaryType.BitNot, @1.StartLine);
            $$.CheckType();
        }
    |   Not unary
        {
            $$ = new UnaryOperator($2, UnaryType.Not, @1.StartLine);
            $$.CheckType();
        }
    |   OpenRound IntKeyword CloseRound unary
        {
            $$ = new UnaryOperator($4, UnaryType.ToInt, @2.StartLine);
            $$.CheckType();
        }
    |   OpenRound DoubleKeyword CloseRound unary
        {
            $$ = new UnaryOperator($4, UnaryType.ToDouble, @2.StartLine);
            $$.CheckType();
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
            $$ = new Ident($1);
            $$.CheckType();
        }
    |   IntNumber
        {
            $$ = new Variable("int", $1);
        }
    |   DoubleNumber
        {
            $$ = new Variable("double", $1);
        }
    |   True
        {
            $$ = new Variable("bool", "true");
        }
    |   False
        {
            $$ = new Variable("bool", "false");
        }
    ;

error_eof
    :   error Eof
        {
            Console.WriteLine(@1.StartLine + " " + @1.EndLine + " " + @1.StartColumn + " " + @1.EndColumn);
        }
    ;

%%

    public Parser(Scanner scanner) : base(scanner) { }

    public void Print(SyntaxTree node)
    {
        string delim = "";
        node.Print(delim);
    }

    public void PrintSyntaxError(string message, int line = -1) {
        ++Compiler.errors;
        Console.WriteLine(message + " " + line);
    }