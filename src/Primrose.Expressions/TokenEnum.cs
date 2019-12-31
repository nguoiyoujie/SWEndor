namespace Primrose.Expressions
{
  internal enum TokenEnum
  {
    NOTHING,

    // Comment
    COMMENT,

    // Types
    DECL_BOOL,
    DECL_FLOAT,
    DECL_INT,
    DECL_STRING,
    DECL_FLOAT2,
    DECL_FLOAT3,
    DECL_FLOAT4,
    DECL_BOOL_ARRAY,
    DECL_FLOAT_ARRAY,
    DECL_INT_ARRAY,
    DECL_STRING_ARRAY,

    // Keywords
    //NEW,
    IF,
    THEN,
    ELSE,
    FOREACH,
    IN,
    FOR,
    WHILE,

    // Literals
    NULLLITERAL,
    BOOLEANLITERAL,
    DECIMALINTEGERLITERAL,
    REALLITERAL,
    HEXINTEGERLITERAL,
    STRINGLITERAL,

    FUNCTION,
    //INDEXED_VARIABLE,
    VARIABLE,
    CONSTANT,
    BRACEOPEN,
    BRACECLOSE,
    BRACKETOPEN,
    BRACKETCLOSE,
    SQBRACKETOPEN,
    SQBRACKETCLOSE,
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


