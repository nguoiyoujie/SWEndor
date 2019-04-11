using System;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public class ParseException : Exception
  {
    internal ParseException(Lexer lexer) : base(string.Format("Unexpected token '{0}' found at line {1}:{2}."
                                                                            , lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.LineNumber
                                                                            , lexer.Position)) { }

    internal ParseException(Lexer lexer, TokenEnum expected) : base(string.Format("Unexpected token '{0}' found at line {1}:{2}. Expected: {3}"
                                                                            , lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.LineNumber
                                                                            , lexer.Position
                                                                            , expected.ToString())) { }
  }
}


