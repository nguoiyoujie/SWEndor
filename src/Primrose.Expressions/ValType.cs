namespace Primrose.Expressions
{
  /// <summary>Represents a value type</summary>
  public enum ValType : byte
  {
    /// <summary>Represents a null value</summary>
    NULL,

    /// <summary>Represents a boolean value</summary>
    BOOL,

    /// <summary>Represents a 32-bit integer value</summary>
    INT,

    /// <summary>Represents a 32-bit floating point value</summary>
    FLOAT,

    /// <summary>Represents a string value</summary>
    STRING,

    /// <summary>Represents two floating point values</summary>
    FLOAT2,

    /// <summary>Represents three floating point values</summary>
    FLOAT3,

    /// <summary>Represents four floating point values</summary>
    FLOAT4,

    /// <summary>Represents an array of boolean values</summary>
    BOOL_ARRAY,

    /// <summary>Represents an array of integer values</summary>
    INT_ARRAY,

    /// <summary>Represents an array of floating point values</summary>
    FLOAT_ARRAY,

    /// <summary>Represents an array of strings</summary>
    STRING_ARRAY
  }
}
