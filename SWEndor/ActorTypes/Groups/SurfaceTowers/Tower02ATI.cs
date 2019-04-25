using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower02ATI : Groups.SurfaceTower
  {
    internal Tower02ATI(Factory owner) : base(owner, "Gun Tower")
    {
      MaxStrength = 25;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      NoMove = true;
      NoAI = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_02.x");
      AddOns = new AddOnInfo[] { new AddOnInfo("Turbolaser Turret", new TV_3DVECTOR(0, 50, 0), new TV_3DVECTOR(0, 0, 0), true) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.DeathExplosionSize = 5;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      /*
      if (ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower02_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -21, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }
  }
}

