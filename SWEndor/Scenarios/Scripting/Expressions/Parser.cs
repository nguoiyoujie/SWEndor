using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Expressions;
using SWEndor.Scenarios.Scripting.Expressions.TokenTypes.Statements;
using System.IO;
using System.Text.RegularExpressions;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public class Parser
  {
    public static RegexOptions RegexOption = RegexOptions.Compiled;
    public static TokenDefinition[] Definitions =
      new TokenDefinition[]
      {
        // white space collector
        new TokenDefinition(@"\s+", TokenEnum.WHITESPACE, RegexOption),

        // keywords
        new TokenDefinition(@"if", TokenEnum.IF, RegexOption),
        new TokenDefinition(@"then", TokenEnum.THEN, RegexOption),
        new TokenDefinition(@"else", TokenEnum.ELSE, RegexOption),

        // literals
        new TokenDefinition(@"true|false", TokenEnum.BOOLEANLITERAL, RegexOption),
        new TokenDefinition(@"[a-zA-Z_][a-zA-Z0-9_]*(?=\s*\()", TokenEnum.FUNCTION, RegexOption),
        new TokenDefinition(@"[a-zA-Z_][a-zA-Z0-9_]*(?!\s*\()", TokenEnum.VARIABLE, RegexOption),
        new TokenDefinition(@"\""(\""\""|[^\""])*\""", TokenEnum.STRINGLITERAL, RegexOption),
        new TokenDefinition(@"([0-9]+\.[0-9]+([eE][+-]?[0-9]+)?([fFdDMm]?)?)|(\.[0-9]+([eE][+-]?[0-9]+)?([fFdDMm]?)?)|([0-9]+([eE][+-]?[0-9]+)([fFdDMm]?)?)|([0-9]+([fFdDMm]?))", TokenEnum.REALLITERAL, RegexOption),
        new TokenDefinition(@"0(x|X)[0-9a-fA-F]+", TokenEnum.HEXINTEGERLITERAL, RegexOption),
        new TokenDefinition(@"[0-9]+(UL|Ul|uL|ul|LU|Lu|lU|lu|U|u|L|l)?", TokenEnum.DECIMALINTEGERLITERAL, RegexOption),

        //?
        //new TokenDefinition(@"(?i)pi|e", TokenEnum.CONSTANT, RegexOption),

        // Brackets
        new TokenDefinition(@"{\s*", TokenEnum.BRACEOPEN, RegexOption),
        new TokenDefinition(@"\s*}", TokenEnum.BRACECLOSE, RegexOption),
        new TokenDefinition(@"\(\s*", TokenEnum.BRACKETOPEN, RegexOption),
        new TokenDefinition(@"\s*\)", TokenEnum.BRACKETCLOSE, RegexOption),

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

    //internal readonly Lexer Lexer;
    //internal readonly RootStatement Root;
    //internal readonly Expression RootExpr;

    public static void Parse(string text, out RootStatement result)
    {
      using (StringReader reader = new StringReader(text))
      {
        Lexer lex = new Lexer(reader, Definitions);
        result = new RootStatement(lex);
      }
    }

    public static void Parse(string text, out Expression result)
    {
      using (StringReader reader = new StringReader(text))
      {
        Lexer lex = new Lexer(reader, Definitions);
        result = new Expression(lex);
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

    public void Evaluate(Context context)
    {
      Root.Evaluate(context);
    }
    */
  }
}


