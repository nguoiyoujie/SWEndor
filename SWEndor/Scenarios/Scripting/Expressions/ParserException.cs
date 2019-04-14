using System;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public class ParseException : Exception
  {
    internal ParseException(Lexer lexer) : base(string.Format("Unexpected token '{0}' found at line {1}:{2}.\nLine: {3}"
                                                                            , lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.LineNumber
                                                                            , lexer.Position
                                                                            , lexer.LineText))
    { }

    internal ParseException(Lexer lexer, TokenEnum expected) : base(string.Format("Unexpected token '{0}' found at line {1}:{2}. Expected: {3}.\nLine: {4}"
                                                                            , lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.LineNumber
                                                                            , lexer.Position
                                                                            , expected.ToString()
                                                                            , lexer.LineText))
    { }
  }

  public class EvalException : Exception
  {
    internal EvalException(string reason) : base(reason) { }

    internal EvalException(string opname, object target, Exception ex) : base(string.Format("Unable to perform '{0}' operation on {1} \nReason: {2}"
                                                                              , opname
                                                                              , target?.ToString() ?? "<null>"
                                                                              , ex.Message)) { }

    internal EvalException(string opname, object target, object target2, Exception ex) : base(string.Format("Unable to perform '{0}' operation between {1} and {2} \nReason: {3}"
                                                                              , opname
                                                                              , target?.ToString() ?? "<null>"
                                                                              , target2?.ToString() ?? "<null>"
                                                                              , ex.Message)) { }

  }
}


