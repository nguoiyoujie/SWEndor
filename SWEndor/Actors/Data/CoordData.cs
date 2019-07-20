using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public struct CoordData
  {
    //private float scalar;

    public TV_3DMATRIX GetLocalMatrixT()
    {
      TV_3DMATRIX mT = new TV_3DMATRIX();
      TVMathLibrary mlib = Globals.Engine.TrueVision.TVMathLibrary;
      mlib.TVMatrixTranslation(ref mT, Position.x, Position.y, Position.z);
      return mT;
    }

    public TV_3DMATRIX GetLocalMatrixR()
    {
      TV_3DMATRIX mR = new TV_3DMATRIX();
      TVMathLibrary mlib = Globals.Engine.TrueVision.TVMathLibrary;
      mlib.TVMatrixRotationYawPitchRoll(ref mR, Rotation.y, Rotation.x, Rotation.z);
      return mR;
    }

    public TV_3DMATRIX GetLocalMatrixS()
    {
      TV_3DMATRIX mS = new TV_3DMATRIX();
      TVMathLibrary mlib = Globals.Engine.TrueVision.TVMathLibrary;
      mlib.TVMatrixScaling(ref mS, Scale, Scale, Scale);
      return mS;
    }

    private TV_3DVECTOR m_prevpos;
    private TV_3DVECTOR m_pos;

    public void Init(ActorTypeInfo type, ActorCreationInfo acreate)
    {
      m_pos = acreate.Position;
      m_prevpos = acreate.Position;
      Rotation = acreate.Rotation;
      Scale = type.Scale * acreate.InitialScale;
    }

    public TV_3DVECTOR Position
    {
      get { return m_pos; }
      set
      {
        m_prevpos = m_pos;
        m_pos = value;
      }
    }

    public TV_3DVECTOR PrevPosition { get { return m_prevpos; } }

    public TV_3DVECTOR LastTravelled { get { return m_pos - m_prevpos; } }

    public TV_3DVECTOR Rotation { get; set; }

    public float Scale { get; set; }

    public void Reset()
    {
      m_prevpos = default(TV_3DVECTOR);
      m_pos = default(TV_3DVECTOR);
      Rotation = default(TV_3DVECTOR);
    }
  }
}
