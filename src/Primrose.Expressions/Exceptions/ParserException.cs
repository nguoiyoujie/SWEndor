using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System;

namespace Primrose.Expressions
{
  internal class ParseException : Exception
  {
    internal ParseException(Lexer lexer) : base("Unexpected token '{0}' found in function '{1}' line {2}:{3}.\nLine: {4}"
                                                                            .F(lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.SourceName
                                                                            , lexer.LineNumber
                                                                            , lexer.Position
                                                                            , lexer.LineText))
    { }

    internal ParseException(Lexer lexer, TokenEnum expected) : base("Unexpected token '{0}' found in function '{1}' at line {2}:{3}. Expected: {4}.\nLine: {5}"
                                                                              .F(lexer.TokenContents.Replace("\n", "")
                                                                              , lexer.SourceName
                                                                              , lexer.LineNumber
                                                                              , lexer.Position
                                                                              , expected.GetEnumName()
                                                                              , lexer.LineText))
    { }

    internal ParseException(Lexer lexer, string message) : base("{0}\nFunction '{1}' at line {2}:{3}. \nLine: {4}"
                                                                          .F(message
                                                                          , lexer.SourceName
                                                                          , lexer.LineNumber
                                                                          , lexer.Position
                                                                          , lexer.LineText))
    { }
  }

  /// <summary>
  /// Provides script evaluation exceptions
  /// </summary>
  public class EvalException : Exception
  {
    /// <summary>
    /// Represents an exception produced when running the script
    /// </summary>
    /// <param name="eval">The object that produced the exception</param>
    /// <param name="reason">The message of the exception</param>
    public EvalException(ITracker eval, string reason) : base("Unable to execute script function '{0}' at line {1}:{2} \nReason: {3} \n"
                                                                                .F(eval.SourceName
                                                                                , eval.LineNumber
                                                                                , eval.Position
                                                                                , reason))
    { }

    /// <summary>
    /// Represents an exception produced when attempting to process an operation during script evaluation
    /// </summary>
    /// <param name="eval">The object that produced the exception</param>
    /// <param name="opname">The operation being attempted</param>
    /// <param name="v">The value in the operation</param>
    /// <param name="ex">The inner exception message</param>
    public EvalException(ITracker eval, string opname, Val v, Exception ex) : base("Unable to perform '{0}' operation on {1} in function '{2}' at line {3}:{4} \nReason: {5}"
                                                                                .F(opname
                                                                                , v.Value?.ToString() ?? "<null>"
                                                                                , eval.SourceName
                                                                                , eval.LineNumber
                                                                                , eval.Position
                                                                                , ex.Message))
    { }

    /// <summary>
    /// Represents an exception produced when attempting to process an operation during script evaluation
    /// </summary>
    /// <param name="eval">The object that produced the exception</param>
    /// <param name="opname">The operation being attempted</param>
    /// <param name="v1">The first value in the operation</param>
    /// <param name="v2">The second value in the operation</param>
    /// <param name="ex">The inner exception message</param>
    public EvalException(ITracker eval, string opname, Val v1, Val v2, Exception ex) : base("Unable to perform '{0}' operation between {1} and {2} in function '{3}' at line {4}:{5} \nReason: {6}"
                                                                                .F(opname
                                                                                , v1.Value?.ToString() ?? "<null>"
                                                                                , v2.Value?.ToString() ?? "<null>"
                                                                                , eval.SourceName
                                                                                , eval.LineNumber
                                                                                , eval.Position
                                                                                , ex.Message))
    { }

  }
}


