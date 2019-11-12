namespace Primrose.Expressions
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

  /// <summary>
  /// Provides tracking for line and position
  /// </summary>
  public interface ITracker
  {
    /// <summary>The name of the source</summary>
    string SourceName { get; }

    /// <summary>The line number</summary>
    int LineNumber { get; }

    /// <summary>The character position in the line</summary>
    int Position { get; }
  }
}


