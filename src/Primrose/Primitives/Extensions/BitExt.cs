namespace Primrose.Primitives.Extensions
{
  /// <summary>
  /// Provides extension methods for bit manipulation functions
  /// </summary>
  public static class BitExts
  {
    /// <summary>Retrieves the most significant bit of a value</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint GetMostSignificantBit(this uint value)
    {
      if (value == 0) return 0;

      uint r = 1;
      if (value >> 16 == 0) { r += 16; value <<= 16; }
      if (value >> 24 == 0) { r += 8; value <<= 8; }
      if (value >> 28 == 0) { r += 4; value <<= 4; }
      if (value >> 30 == 0) { r += 2; value <<= 2; }
      r -= value >> 31;

      return 32 - r;
    }

    /// <summary>Enumerates the bits of a value by position</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static BitEnumerable GetUniqueBits(this uint value) 
    {
      // index 0 = int 1
      // index 1 = int 2
      // index 2 = int 4 etc.
      return new BitEnumerable(value);
    }

    /// <summary>Represents an enumeration of bit positions</summary>
    public struct BitEnumerable
    {
      readonly uint T;
      /// <summary>Creates the enumerable</summary>
      public BitEnumerable(uint value)
      {
        T = value;
      }
      /// <summary>Gets the enumerator</summary>
      public BitEnumerator GetEnumerator() { return new BitEnumerator(T); }
    }

    /// <summary>Represents an enumerator of bit positions</summary>
    public struct BitEnumerator
    {
      readonly uint T;
      uint V;
      int curr;
      /// <summary>Creates the enumerator</summary>
      public BitEnumerator(uint value)
      {
        T = value;
        V = 1;
        curr = 0;
      }
      /// <summary>Retrieves the next bit position</summary>
      public bool MoveNext()
      {
        while (V <= T)
        {
          if ((V & T) > 0)
          {
            V <<= 1;
            curr++;
            return true;
          }
          V <<= 1;
          curr++;
        }

        V <<= 1;
        curr++;
        return false;
      }
      /// <summary>Retrieves the current bit position</summary>
      public int Current { get { return curr; } }
    }
  }
}
