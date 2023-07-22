using MTV3D65;
using SWEndor.Game.Models;
using SWEndor.Game.Projectiles;
using SWEndor.Game.ProjectileTypes;

namespace SWEndor.Game.Scenarios
{
  public struct ProjectileSpawnInfo
  {
    public ProjectileTypeInfo Type;
    public string Name;
    public string SidebarName;
    public float SpawnTime;
    public float LifeTime;

    public FactionInfo Faction;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;

    public ProjectileInfo Spawn(ScenarioBase scenario)
    {
      ProjectileCreationInfo acinfo;
      ProjectileInfo ainfo;

      acinfo = new ProjectileCreationInfo(Type);
      if (Name != null && Name != "")
        acinfo.Name = Name;
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.CreationTime = SpawnTime;
      acinfo.Position = Position;
      acinfo.Rotation = Rotation;
      ainfo = scenario.Engine.ProjectileFactory.Create(acinfo);
      ainfo.SideBarName = SidebarName;
      acinfo.LifeTime = LifeTime;

      return ainfo;
    }
  }
}
