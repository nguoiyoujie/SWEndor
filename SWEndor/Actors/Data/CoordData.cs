using MTV3D65;

namespace SWEndor.Actors.Data
{
  public struct CoordData
  {
    private TV_3DVECTOR m_prevpos;
    private TV_3DVECTOR m_pos;

    public void Init()
    {
      m_prevpos = default(TV_3DVECTOR);
      m_pos = default(TV_3DVECTOR);
      Rotation = default(TV_3DVECTOR);
    }

    public TV_3DVECTOR Position
    {
      get { return m_pos; }
      set
      {
        m_prevpos = m_pos.Equals(default(TV_3DVECTOR)) ? value : m_pos;
        m_pos = value;
      }
    }

    public TV_3DVECTOR PrevPosition { get { return m_prevpos; } }

    public TV_3DVECTOR LastTravelled { get { return m_pos - m_prevpos; } }

    public TV_3DVECTOR Rotation { get; set; }

    public bool Initialized { get { return !(m_pos.Equals(default(TV_3DVECTOR)) || m_prevpos.Equals(default(TV_3DVECTOR))); } }

    public void Reset()
    {
      m_prevpos = default(TV_3DVECTOR);
      m_pos = default(TV_3DVECTOR);
      Rotation = default(TV_3DVECTOR);
    }
  }
}
