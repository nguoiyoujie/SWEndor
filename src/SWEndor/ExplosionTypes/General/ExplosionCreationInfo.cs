using MTV3D65;
using SWEndor.Explosions;
using SWEndor.Models;

namespace SWEndor.ExplosionTypes
{
  public struct ExplosionCreationInfo : ICreationInfo<ExplosionInfo, ExplosionTypeInfo>
  {
    public ExplosionTypeInfo TypeInfo { get; }
    public string Name;
    public float CreationTime { get; set; }
    public ActorState InitialState { get; set; }
    public float InitialScale;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;

    public ExplosionCreationInfo(ExplosionTypeInfo at)
    {
      // Load defaults from actortype
      TypeInfo = at;
      Name = at.Name;

      CreationTime = 0;
      InitialState = ActorState.NORMAL;
      InitialScale = 1;
      Position = new TV_3DVECTOR();
      Rotation = new TV_3DVECTOR();
    }
  }
}
