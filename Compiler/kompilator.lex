%using QUT.Gppg;
%namespace Compiler

IntNumber		0|[1-9][0-9]*
IntNumberHex	(0x|0X)[0-9a-fA-F]*
DoubleNumber	0\.[0-9]+|[1-9][0-9]*\.[0-9]+
Identifier		[a-zA-Z][a-zA-Z0-9]*
Comment			\/\/.*/\n
Endl			\n|\r\n
Literal			\"(\\.|[^\n\\"])*\"

%%

"program"		{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Program; }
"if"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.If; }
"else"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Else; }
"while"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.While; }
"read"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Read; }
"write"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Write; }
"return"		{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Return; }
"int"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.IntKeyword; }
"double"		{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.DoubleKeyword; }
"bool"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.BoolKeyword; }
"true"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.True; }
"false"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.False; }
"hex"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.IntHex; }
"="				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Assign; }
"||"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Or; }
"&&"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.And; }
"|"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.BitOr; }
"&"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.BitAnd; }
"=="			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Equal; }
"!="			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.NotEqual; }
">"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Greater; }
">="			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.GreaterOrEqual; }
"<"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Less; }
"<="			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.LessOrEqual; }
"+"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Plus; }
"-"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Minus; }
"*"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Multiplies; }
"/"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Divides; }
"!"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Not; }
"~"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.BitNot; }
"("				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.OpenRound; }
")"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.CloseRound; }
"{"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.OpenCurly; }
"}"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.CloseCurly; }
","				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Comma; }
";"				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.SemiColon; }
{Identifier}	{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.Ident; }
{IntNumber}		{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.IntNumber; }
{IntNumberHex}	{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.IntNumberHex; }
{DoubleNumber}	{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.DoubleNumber; }
{Literal}		{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); yylval.val = yytext; return (int)Tokens.Literal; }
<<EOF>>			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Eof; }
" "				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); }
"\t"			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); }
{Comment}		{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); }
{Endl}			{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); }
.				{ yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); return (int)Tokens.Error; }