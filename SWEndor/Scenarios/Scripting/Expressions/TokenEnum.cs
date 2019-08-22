namespace SWEndor.Scenarios.Scripting.Expressions
{
  public enum TokenEnum
  {
    NOTHING,

    // Comment
    COMMENT,

    // Keywords
    IF,
    THEN,
    ELSE,
    FOREACH,
    IN,

    // Literals
    BOOLEANLITERAL,
    DECIMALINTEGERLITERAL,
    REALLITERAL,
    HEXINTEGERLITERAL,
    STRINGLITERAL,

    FUNCTION,
    VARIABLE,
    CONSTANT,
    BRACEOPEN,
    BRACECLOSE,
    BRACKETOPEN,
    BRACKETCLOSE,
    SEMICOLON,
    PLUSPLUS,
    MINUSMINUS,
    PIPEPIPE,
    AMPAMP,
    AMP,
    //POWER,
    PLUS,
    MINUS,
    EQUAL,
    ASSIGN,
    NOTEQUAL,
    NOT,
    ASTERISK,
    SLASH,
    PERCENT,
    QUESTIONMARK,
    COMMA,
    LESSEQUAL,
    GREATEREQUAL,
    LESSTHAN,
    GREATERTHAN,
    COLON,
    PLUSASSIGN,
    MINUSASSIGN,
    ASTERISKASSIGN,
    SLASHASSIGN,
    PERCENTASSIGN,
    AMPASSIGN,
    PIPEASSIGN,

    //EOF,
    WHITESPACE
  }
}


