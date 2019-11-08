using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System;

namespace Primrose.Expressions
{
  public class ParseException : Exception
  {
    internal ParseException(Lexer lexer) : base("Unexpected token '{0}' found at line {1}:{2}.\nLine: {3}"
                                                                            .F(lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.LineNumber
                                                                            , lexer.Position
                                                                            , lexer.LineText))
    { }

  internal ParseException(Lexer lexer, TokenEnum expected) : base("Unexpected token '{0}' found at line {1}:{2}. Expected: {3}.\nLine: {4}"
                                                                            .F(lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.LineNumber
                                                                            , lexer.Position
                                                                            , expected.GetEnumName()
                                                                            , lexer.LineText))
    { }
  }

  public class EvalException : Exception
  {
    public EvalException(ITracker eval, string reason) : base("Unable to execute script at line {0}:{1} \nReason: {2} \n"
                                                                                .F(eval.LineNumber
                                                                                , eval.Position
                                                                                , reason))
    { }

    public EvalException(ITracker eval, string opname, Val v, Exception ex) : base("Unable to perform '{0}' operation on {1} at line {2}:{3} \nReason: {4}"
                                                                                .F(opname
                                                                                , v.Value?.ToString() ?? "<null>"
                                                                                , eval.LineNumber
                                                                                , eval.Position
                                                                                , ex.Message))
    { }

    public EvalException(ITracker eval, string opname, Val v1, Val v2, Exception ex) : base("Unable to perform '{0}' operation between {1} and {2} at line {3}:{4} \nReason: {5}"
                                                                                .F(opname
                                                                                , v1.Value?.ToString() ?? "<null>"
                                                                                , v2.Value?.ToString() ?? "<null>"
                                                                                , eval.LineNumber
                                                                                , eval.Position
                                                                                , ex.Message))
    { }

  }
}


