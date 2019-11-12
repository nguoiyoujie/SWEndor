using MTV3D65;
using SWEndor.Models;
using SWEndor.Projectiles;

namespace SWEndor.ProjectileTypes
{
  public struct ProjectileCreationInfo : ICreationInfo<ProjectileInfo, ProjectileTypeInfo>
  {
    public FactionInfo Faction;
    public ProjectileTypeInfo TypeInfo { get; }
    public string Name;
    public float CreationTime { get; set; }
    public ActorState InitialState { get; set; }
    public float InitialScale;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;

    public float InitialSpeed;
    public bool FreeSpeed;

    public ProjectileCreationInfo(ProjectileTypeInfo at)
    {
      // Load defaults from actortype
      TypeInfo = at;
      Name = at.Name;
      InitialSpeed = at.MoveLimitData.MaxSpeed;

      Faction = FactionInfo.Neutral;
      CreationTime = 0;
      InitialState = ActorState.NORMAL;
      InitialScale = 1;
      Position = new TV_3DVECTOR();
      Rotation = new TV_3DVECTOR();
      FreeSpeed = false;
    }
  }
}

