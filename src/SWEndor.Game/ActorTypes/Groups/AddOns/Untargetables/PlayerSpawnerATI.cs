using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;

namespace SWEndor.Game.ActorTypes.Instances
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
      SpawnerData.SpawnLocations = new float3[] { new float3(0, 0, 0) };
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);

      ActorInfo p = ainfo.TopParent;
      p.SpawnerInfo.Process(engine, ainfo, p);
    }
  }
}

