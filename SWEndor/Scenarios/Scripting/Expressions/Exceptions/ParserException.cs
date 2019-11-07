using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public class ParseException : Exception
  {
    internal ParseException(Lexer lexer) : base(TextLocalization.Get(TextLocalKeys.SCRIPT_PARSER_UNEXPECTED_TOKEN)
                                                                            .F(lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.LineNumber
                                                                            , lexer.Position
                                                                            , lexer.LineText))
    { }

    internal ParseException(Lexer lexer, TokenEnum expected) : base(TextLocalization.Get(TextLocalKeys.SCRIPT_PARSER_UNEXPECTED_TOKEN_2)
                                                                            .F(lexer.TokenContents.Replace("\n", "")
                                                                            , lexer.LineNumber
                                                                            , lexer.Position
                                                                            , expected.GetEnumName()
                                                                            , lexer.LineText))
    { }
  }

  public class EvalException : Exception
  {
    internal EvalException(ITracker eval, string reason) : base(TextLocalization.Get(TextLocalKeys.SCRIPT_EVAL_INVALID)
                                                                              .F(eval.LineNumber
                                                                              , eval.Position
                                                                              , reason)) { }

    internal EvalException(ITracker eval, string opname, Val v, Exception ex) : base(TextLocalization.Get(TextLocalKeys.SCRIPT_EVAL_INVALID_OP)
                                                                              .F(opname
                                                                              , v.Value?.ToString() ?? "<null>"
                                                                              , eval.LineNumber
                                                                              , eval.Position
                                                                              , ex.Message)) { }

    internal EvalException(ITracker eval, string opname, Val v1, Val v2, Exception ex) : base(TextLocalization.Get(TextLocalKeys.SCRIPT_EVAL_INVALID_BOP)
                                                                              .F(opname
                                                                              , v1.Value?.ToString() ?? "<null>"
                                                                              , v2.Value?.ToString() ?? "<null>"
                                                                              , eval.LineNumber
                                                                              , eval.Position
                                                                              , ex.Message)) { }

  }
}


