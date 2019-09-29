using MTV3D65;
using SWEndor.Models;

namespace SWEndor.ExplosionTypes
{
  public struct ExplosionCreationInfo
  {
    public ExplosionTypeInfo ExplosionTypeInfo;
    public string Name;
    public float CreationTime;
    public ActorState InitialState;
    public float InitialScale;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;


    public ExplosionCreationInfo(ExplosionTypeInfo at)
    {
      // Load defaults from actortype
      ExplosionTypeInfo = at;
      Name = at.Name;

      CreationTime = 0;
      InitialState = ActorState.NORMAL;
      InitialScale = 1;
      Position = new TV_3DVECTOR();
      Rotation = new TV_3DVECTOR();
    }
  }
}
