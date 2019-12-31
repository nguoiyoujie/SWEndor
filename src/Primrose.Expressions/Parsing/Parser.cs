using Primrose.Expressions.Tree.Expressions;
using Primrose.Expressions.Tree.Statements;
using System.IO;
using System.Text.RegularExpressions;

namespace Primrose.Expressions
{
  internal class Parser
  {
    public static RegexOptions RegexOption = RegexOptions.CultureInvariant;
    public static TokenDefinition[] Definitions =
      new TokenDefinition[]
      {
        // white space collector
        new TokenDefinition(@"\s+", TokenEnum.WHITESPACE, RegexOption),

        // type keywords
        new TokenDefinition(@"bool\b(?!\[)", TokenEnum.DECL_BOOL, RegexOption),
        new TokenDefinition(@"float\b(?!\[)", TokenEnum.DECL_FLOAT, RegexOption),
        new TokenDefinition(@"int\b(?!\[)", TokenEnum.DECL_INT, RegexOption),
        new TokenDefinition(@"string\b(?!\[)", TokenEnum.DECL_STRING, RegexOption),
        new TokenDefinition(@"float2\b(?!\[)", TokenEnum.DECL_FLOAT2, RegexOption),
        new TokenDefinition(@"float3\b(?!\[)", TokenEnum.DECL_FLOAT3, RegexOption),
        new TokenDefinition(@"float4\b(?!\[)", TokenEnum.DECL_FLOAT4, RegexOption),
        new TokenDefinition(@"bool\[\]", TokenEnum.DECL_BOOL_ARRAY, RegexOption),
        new TokenDefinition(@"float\[\]", TokenEnum.DECL_FLOAT_ARRAY, RegexOption),
        new TokenDefinition(@"string\[\]", TokenEnum.DECL_STRING_ARRAY, RegexOption),
        new TokenDefinition(@"int\[\]", TokenEnum.DECL_INT_ARRAY, RegexOption),

        // keywords
        //new TokenDefinition(@"new\b", TokenEnum.NEW, RegexOption),
        new TokenDefinition(@"if\b", TokenEnum.IF, RegexOption),
        new TokenDefinition(@"then\b", TokenEnum.THEN, RegexOption),
        new TokenDefinition(@"else\b", TokenEnum.ELSE, RegexOption),
        new TokenDefinition(@"foreach\b", TokenEnum.FOREACH, RegexOption),
        new TokenDefinition(@"in\b", TokenEnum.IN, RegexOption),
        new TokenDefinition(@"for\b", TokenEnum.FOR, RegexOption),
        new TokenDefinition(@"while\b", TokenEnum.WHILE, RegexOption),

        // literals
        new TokenDefinition(@"(((N|n)ull)|NULL)\b", TokenEnum.NULLLITERAL, RegexOption),
        new TokenDefinition(@"((T|t)rue|(F|f)alse|TRUE|FALSE)\b", TokenEnum.BOOLEANLITERAL, RegexOption),
        new TokenDefinition(@"[a-zA-Z_][a-zA-Z0-9_\.]*(?=\s*\()", TokenEnum.FUNCTION, RegexOption),
        new TokenDefinition(@"[a-zA-Z_][a-zA-Z0-9_\.]*(?!\s*\()", TokenEnum.VARIABLE, RegexOption),
        new TokenDefinition(@"\""(\""\""|[^\""])*\""", TokenEnum.STRINGLITERAL, RegexOption),
        new TokenDefinition(@"0(x|X)[0-9a-fA-F]+\b", TokenEnum.HEXINTEGERLITERAL, RegexOption),
        new TokenDefinition(@"[0-9]+(?![fFdDMmeE\.])\b", TokenEnum.DECIMALINTEGERLITERAL, RegexOption),
        new TokenDefinition(@"([0-9]+\.[0-9]+([eE][+-]?[0-9]+)?([fFdDMm]?)?)|(\.[0-9]+([eE][+-]?[0-9]+)?([fFdDMm]?)?)|([0-9]+([eE][+-]?[0-9]+)([fFdDMm]?)?)|([0-9]+([fFdDMm]?))\b", TokenEnum.REALLITERAL, RegexOption),

        // Brackets
        new TokenDefinition(@"{\s*", TokenEnum.BRACEOPEN, RegexOption),
        new TokenDefinition(@"\s*}", TokenEnum.BRACECLOSE, RegexOption),
        new TokenDefinition(@"\(\s*", TokenEnum.BRACKETOPEN, RegexOption),
        new TokenDefinition(@"\s*\)", TokenEnum.BRACKETCLOSE, RegexOption),
        new TokenDefinition(@"\[\s*", TokenEnum.SQBRACKETOPEN, RegexOption),
        new TokenDefinition(@"\s*\]", TokenEnum.SQBRACKETCLOSE, RegexOption),

        // Comment
        new TokenDefinition(@"//.*", TokenEnum.COMMENT, RegexOption),

        // Multi-character Operators
        new TokenDefinition(@"\+\+", TokenEnum.PLUSPLUS, RegexOption),
        new TokenDefinition(@"--", TokenEnum.MINUSMINUS, RegexOption),
        new TokenDefinition(@"\|\|", TokenEnum.PIPEPIPE, RegexOption),
        new TokenDefinition(@"&&", TokenEnum.AMPAMP, RegexOption),
        new TokenDefinition(@"!=|<>", TokenEnum.NOTEQUAL, RegexOption),
        new TokenDefinition(@"==", TokenEnum.EQUAL, RegexOption),
        new TokenDefinition(@"<=", TokenEnum.LESSEQUAL, RegexOption),
        new TokenDefinition(@">=", TokenEnum.GREATEREQUAL, RegexOption),

        new TokenDefinition(@"\+=", TokenEnum.PLUSASSIGN, RegexOption),
        new TokenDefinition(@"\-=", TokenEnum.MINUSASSIGN, RegexOption),
        new TokenDefinition(@"\*=", TokenEnum.ASTERISKASSIGN, RegexOption),
        new TokenDefinition(@"/=", TokenEnum.SLASHASSIGN, RegexOption),
        new TokenDefinition(@"%=", TokenEnum.PERCENTASSIGN, RegexOption),
        new TokenDefinition(@"&=", TokenEnum.AMPASSIGN, RegexOption),
        new TokenDefinition(@"\|=", TokenEnum.PIPEASSIGN, RegexOption),

        // Single-character Operators
        new TokenDefinition(@"=", TokenEnum.ASSIGN, RegexOption),
        new TokenDefinition(@";", TokenEnum.SEMICOLON, RegexOption),
        new TokenDefinition(@"&(?!&)", TokenEnum.AMP, RegexOption),
        //new TokenDefinition(@"\^", TokenEnum.POWER, RegexOption),
        new TokenDefinition(@"\+", TokenEnum.PLUS, RegexOption),
        new TokenDefinition(@"-", TokenEnum.MINUS, RegexOption),
        new TokenDefinition(@"!", TokenEnum.NOT, RegexOption),
        new TokenDefinition(@"\*", TokenEnum.ASTERISK, RegexOption),
        new TokenDefinition(@"/", TokenEnum.SLASH, RegexOption),
        new TokenDefinition(@"%", TokenEnum.PERCENT, RegexOption),
        new TokenDefinition(@"\?", TokenEnum.QUESTIONMARK, RegexOption),
        new TokenDefinition(@",", TokenEnum.COMMA, RegexOption),
        new TokenDefinition(@"<(?!>)", TokenEnum.LESSTHAN, RegexOption),
        new TokenDefinition(@">", TokenEnum.GREATERTHAN, RegexOption),
        new TokenDefinition(@":", TokenEnum.COLON, RegexOption)
      };

    public static void Parse(ContextScope scope, string text, out RootStatement result, string srcname, ref int linenumber)
    {
      using (StringReader reader = new StringReader(text))
      {
        Lexer lex = new Lexer(reader, Definitions, srcname, linenumber);
        result = new RootStatement(scope, lex);
        linenumber = lex.LineNumber;
      }
    }

    public static void Parse(ContextScope scope, string text, out Expression result, string srcname, ref int linenumber)
    {
      using (StringReader reader = new StringReader(text))
      {
        Lexer lex = new Lexer(reader, Definitions, srcname, linenumber);
        result = new Expression(scope, lex);
        linenumber = lex.LineNumber;
      }
    }

    /*
    public Parser(string text)
    {
      using (StringReader reader = new StringReader(text))
      {
        Lexer = new Lexer(reader, Definitions);
        Root = new RootStatement(this);
      }
    }

    public void Evaluate(Script local, Context context)
    {
      Root.Evaluate(context);
    }
    */
  }
}


