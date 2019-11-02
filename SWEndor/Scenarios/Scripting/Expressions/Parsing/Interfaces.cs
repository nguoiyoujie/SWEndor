namespace SWEndor.Scenarios.Scripting.Expressions
{
  // Reference https://stackoverflow.com/questions/673113/poor-mans-lexer-for-c-sharp
  internal interface IMatcher
  {
    /// <summary>
    /// Return the number of characters that this "regex" or equivalent
    /// matches.
    /// </summary>
    /// <param name="text">The text to be matched</param>
    /// <returns>The number of characters that matched</returns>
    int Match(string text);
  }

  internal interface ITracker
  {
    int LineNumber { get; }
    int Position { get; }
  }
}


