namespace Primrose.Expressions
{
  /// <summary>
  /// An enumeration of unary operators
  /// </summary>
  public enum UOp : byte
  {
    /// <summary>The identiy operator. Does nothing</summary>
    IDENTITY,

    /// <summary>The logical not operator (!a)</summary>
    LOGICAL_NOT,

    /// <summary>The numerical negation operator (-a)</summary>
    NEGATION,
  }
}
