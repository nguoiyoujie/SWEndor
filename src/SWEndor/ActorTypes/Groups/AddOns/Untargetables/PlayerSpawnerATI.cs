using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class PlayerSpawnerATI : Groups.AddOn
  {
    internal PlayerSpawnerATI(Factory owner) : base(owner, "SPAWN", "Player Spawner")
    {
      RenderData.RadarSize = 0;

      AIData.TargetType = TargetType.NULL;
      RenderData.RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);

      SpawnerData.SpawnMoveDelay = 3;
      SpawnerData.SpawnInterval = 5;
      SpawnerData.SpawnsRemaining = 0;
      SpawnerData.SpawnLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 0, 0) };
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);

      ActorInfo p = ainfo.TopParent;
      p.SpawnerInfo.Process(engine, ainfo, p);
    }
  }
}

