using Primrose.Primitives.ValueTypes;

namespace SWEndor
{
  public struct COLOR
  {
    public COLOR(float r, float g, float b, float a)
    {
      Value = 0;
      fA = a;
      fR = r;
      fG = g;
      fB = b;
    }

    public COLOR(float3 f)
    {
      Value = 0;
      fA = 1;
      fR = f[0];
      fG = f[1];
      fB = f[2];
    }

    public COLOR(float4 f)
    {
      Value = 0;
      fA = f[3];
      fR = f[0];
      fG = f[1];
      fB = f[2];
    }

    public COLOR(int value)
    {
      Value = value;
    }

    public int Value;

    public float fA
    {
      get { return ((Value & unchecked((int)0xFF000000)) >> 24) / 255f; }
      set { Value = Value & 0x00FFFFFF | (unchecked((int)(value * 255u)) << 24); }
    }

    public float fR
    {
      get { return ((Value & 0xFF0000) >> 16) / 255f; }
      set { Value = Value & unchecked((int)0xFF00FFFF) | (unchecked((int)(value * 255u)) << 16); }
    }

    public float fG
    {
      get { return ((Value & 0xFF00) >> 8) / 255f; }
      set { Value = Value & unchecked((int)0xFFFF00FF) | (unchecked((int)(value * 255u)) << 8); }
    }

    public float fB
    {
      get { return (Value & 0xFF) / 255f; }
      set { Value = Value & unchecked((int)0xFFFFFF00) | unchecked((int)(value * 255u)); }
    }

    public byte bA
    {
      get { return (byte)((Value & unchecked((int)0xFF000000)) >> 24); }
      set { Value = Value & 0x00FFFFFF | (value << 24); }
    }

    public byte bR
    {
      get { return (byte)((Value & 0xFF0000) >> 16); }
      set { Value = Value & unchecked((int)0xFF00FFFF) | (value << 16); }
    }

    public byte bG
    {
      get { return (byte)((Value & 0xFF00) >> 8); }
      set { Value = Value & unchecked((int)0xFFFF00FF) | (value << 8); }
    }

    public byte bB
    {
      get { return (byte)(Value & 0xFF); }
      set { Value = Value & unchecked((int)0xFFFFFF00) | value; }
    }
  }
}
