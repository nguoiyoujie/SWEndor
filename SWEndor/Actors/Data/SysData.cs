using MTV3D65;
using SWEndor.ActorTypes;
using System;

namespace SWEndor.Actors.Data
{
  public struct SysData
  {
    public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      MaxStrength = type.MaxStrength;
      Strength = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : type.MaxStrength;
    }

    private float m_str;
    private float m_maxstr;
    public float Strength
    {
      get { return m_str; }
      set { m_str = value.Clamp(-1, MaxStrength); }
    }
    public float MaxStrength
    {
      get { return m_maxstr; }
      set
      {
        m_maxstr = value;
        if (m_str > m_maxstr)
          m_str = m_maxstr;
      }
    }
    public float StrengthFrac { get { return Strength / MaxStrength; } } // strength is already clamped
    //public float StrengthFrac { get { return (Strength / MaxStrength).Clamp(0, 1); } }

    public TV_COLOR StrengthColor
    {
      get
      {
        double quad = 1.6708;
        float sr = StrengthFrac;
        float r = (float)Math.Cos(sr * quad);
        float g = (float)Math.Sin(sr * quad);
        float b = 0;
        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;
        return new TV_COLOR(r, g, b, 1);
      }
    }

    public void Reset()
    {
      // actually, do we need to reset these values?
      MaxStrength = 1;
      Strength = 1;
    }
  }
}
