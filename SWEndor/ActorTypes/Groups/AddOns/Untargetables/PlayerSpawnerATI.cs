using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Instances
{
  public class PlayerSpawnerATI : Groups.AddOn
  {
    internal PlayerSpawnerATI(Factory owner) : base(owner, "Player Spawner")
    {
      RenderData.RadarSize = 0;

      AIData.TargetType = TargetType.NULL;
      RenderData.RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);
    }

    public override void Initialize(Engine engine, ActorInfo ainfo)
    {
      ainfo.SpawnerInfo = SpawnerInfoDecorator.PlayerSpawn_Default;
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);

      ActorInfo p = ainfo.TopParent;
      SpawnerInfo s = p.SpawnerInfo;
      s.UnlockSpawns(engine, ainfo, p);

      foreach (ActorInfo a in ainfo.Children)
        if (!a.UseParentCoords)
          s.MoveSpawns(engine, a, p);
    }
  }
}

