using System.Collections.Generic;

namespace Primrose.Primitives.Extensions
{
  public static class BitExts
  {
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

    public static BitEnumerable GetUniqueBits(this uint value) 
    {
      // index 0 = int 1
      // index 1 = int 2
      // index 2 = int 4 etc.
      return new BitEnumerable(value);
    }

    public struct BitEnumerable
    {
      readonly uint T;
      public BitEnumerable(uint value)
      {
        T = value;
      }
      public BitEnumerator GetEnumerator() { return new BitEnumerator(T); }
    }

    public struct BitEnumerator
    {
      readonly uint T;
      uint V;
      int curr;
      public BitEnumerator(uint value)
      {
        T = value;
        V = 1;
        curr = 0;
      }
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
      public int Current { get { return curr; } }
    }
  }
}
