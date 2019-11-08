using System.Text.RegularExpressions;

namespace Primrose.Expressions
{
  // Reference https://stackoverflow.com/questions/673113/poor-mans-lexer-for-c-sharp
  internal sealed class RegexMatcher : IMatcher
  {
    private readonly Regex m_regex;
    public RegexMatcher(string regex, RegexOptions options) { m_regex = new Regex(string.Format("^({0})", regex), options); }

    public int Match(string text)
    {
      var m = m_regex.Match(text);
      return m.Success ? m.Length : 0;
    }
    public override string ToString() => m_regex.ToString();
  }
}


