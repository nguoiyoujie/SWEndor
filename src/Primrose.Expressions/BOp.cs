namespace Primrose.Expressions
{
  /// <summary>
  /// An enumeration of binary operators
  /// </summary>
  public enum BOp : byte
  {
    /// <summary>The logical OR operator (a || b)</summary>
    LOGICAL_OR,

    /// <summary>The logical AND operator (a &amp;&amp; b)</summary>
    LOGICAL_AND,

    /// <summary>The numerical addition or string concatenation operator (a + b)</summary>
    ADD,

    /// <summary>The numerical subtraction operator (a - b)</summary>
    SUBTRACT,

    /// <summary>The numerical multiplication operator (a * b)</summary>
    MULTIPLY,

    /// <summary>The numerical modulus operator (a % b)</summary>
    MODULUS,

    /// <summary>The numerical division operator (a / b)</summary>
    DIVIDE,

    /// <summary>The equality comparer (a == b)</summary>
    EQUAL_TO,

    /// <summary>The inequality comparer (a != b)</summary>
    NOT_EQUAL_TO,

    /// <summary>The numerical greater than comparer (a &gt; b)</summary>
    MORE_THAN,

    /// <summary>The numerical greater than and equality comparer (a &gt;= b)</summary>
    MORE_THAN_OR_EQUAL_TO,

    /// <summary>The numerical less than comparer (a &lt; b)</summary>
    LESS_THAN,

    /// <summary>The numerical less than and equality comparer (a &lt;= b)</summary>
    LESS_THAN_OR_EQUAL_TO,
  }
}
