using MTV3D65;
using SWEndor.Game.Models;
using SWEndor.Game.Particles;
using SWEndor.Game.ParticleTypes;

namespace SWEndor.Game.ProjectileTypes
{
  public struct ParticleCreationInfo : ICreationInfo<ParticleInfo, ParticleTypeInfo>
  {
    public ParticleTypeInfo TypeInfo { get; }
    public string Name;
    public float CreationTime { get; set; }
    public ActorState InitialState { get; set; }
    public float InitialScale;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;

    public ParticleCreationInfo(ParticleTypeInfo at)
    {
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

