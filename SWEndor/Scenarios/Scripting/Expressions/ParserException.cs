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
    internal EvalException(ITracker eval, string reason) : base(string.Format("Unable to execute script at line {0}:{1} \nReason: {2} \n"
                                                                              , eval.LineNumber
                                                                              , eval.Position
                                                                              , reason)) { }

    internal EvalException(ITracker eval, string opname, object target, Exception ex) : base(string.Format("Unable to perform '{0}' operation on {1} at line {2}:{3} \nReason: {4}"
                                                                              , opname
                                                                              , target?.ToString() ?? "<null>"
                                                                              , eval.LineNumber
                                                                              , eval.Position
                                                                              , ex.Message)) { }

    internal EvalException(ITracker eval, string opname, object target, object target2, Exception ex) : base(string.Format("Unable to perform '{0}' operation between {1} and {2} at line {3}:{4} \nReason: {5}"
                                                                              , opname
                                                                              , target?.ToString() ?? "<null>"
                                                                              , target2?.ToString() ?? "<null>"
                                                                              , eval.LineNumber
                                                                              , eval.Position
                                                                              , ex.Message)) { }

  }
}


