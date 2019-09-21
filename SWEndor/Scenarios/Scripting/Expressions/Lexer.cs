using SWEndor.Primitives;
using System;
using System.IO;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  // Reference: https://stackoverflow.com/questions/673113/poor-mans-lexer-for-c-sharp
  internal sealed class Lexer : IDisposable
  {
    private readonly TextReader m_reader;
    private readonly TokenDefinition[] m_tokenDefinitions;
    public string LineText { get; private set; }

    private string lineRemaining;

    public Lexer(TextReader reader, TokenDefinition[] tokenDefinitions, int linenumber)
    {
      m_reader = reader;
      m_tokenDefinitions = tokenDefinitions;
      LineNumber = linenumber;
      nextLine();
      Next();
    }

    private void nextLine()
    {
      do
      {
        lineRemaining = m_reader.ReadLine();
        LineText = lineRemaining;
        ++LineNumber;
        Position = 0;
      } while (lineRemaining != null && lineRemaining.Length == 0);
    }

    public bool EndOfStream { get { return lineRemaining == null; } }

    public TokenEnum Peek()
    {
      TokenEnum token = TokenEnum.NOTHING;
      string content = "";
      int position = Position;
      int matched = LookAhead(out token, out content, out position);
      return token;
    }

    private int LookAhead(out TokenEnum token, out string content, out int position)
    {
      if (lineRemaining == null)
      {
        token = 0;
        content = "";
        position = Position;
        return 0;
      }
      foreach (var def in m_tokenDefinitions)
      {
        var matched = def.Matcher.Match(lineRemaining);
        if (matched > 0)
        {
          position = Position + matched;
          token = def.Token;
          content = lineRemaining.Substring(0, matched);

          // whitespace elimination
          if (content.Trim().Length == 0)
          {
            DoNext(matched, token, content, position);
            return LookAhead(out token, out content, out position);
          }

          // comment elimination
          if (token == TokenEnum.COMMENT)
          {
            nextLine();
            return LookAhead(out token, out content, out position);
          }

          return matched;
        }
      }
      throw new Exception("Unable to match against any tokens at line {0} position {1} \"{2}\"".F(
                                        LineNumber, Position, lineRemaining));
    }

    public bool Next()
    {
      TokenEnum token = TokenEnum.NOTHING;
      string content = "";
      int position = Position;
      int matched = LookAhead(out token, out content, out position);
      return DoNext(matched, token, content, position);
    }

    private bool DoNext(int matched, TokenEnum token, string content, int position)
    {
      TokenType = token;
      TokenContents = content;
      Position = position;

      if (matched > 0)
      {
        lineRemaining = lineRemaining.Substring(matched);
        if (lineRemaining.Length == 0)
          nextLine();
        return true;
      }
      return false;
    }

    public string TokenContents { get; private set; }
    public TokenEnum TokenType { get; private set; }
    public int LineNumber { get; private set; }
    public int Position { get; private set; } = 1;

    public void Dispose() => m_reader.Dispose();
  }
}


