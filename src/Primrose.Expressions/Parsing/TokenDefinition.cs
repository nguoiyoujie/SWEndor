using System.Text.RegularExpressions;

namespace Primrose.Expressions
{
  // Reference https://stackoverflow.com/questions/673113/poor-mans-lexer-for-c-sharp
  internal sealed class TokenDefinition
  {
    internal readonly IMatcher Matcher;
    public readonly TokenEnum Token;

    public TokenDefinition(string regex, TokenEnum token, RegexOptions options)
    {
      Matcher = new RegexMatcher(regex, options);
      Token = token;
    }
  }
}


