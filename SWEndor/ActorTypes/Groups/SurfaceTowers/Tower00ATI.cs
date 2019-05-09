using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower00ATI : Groups.SurfaceTower
  {
    internal Tower00ATI(Factory owner) : base(owner, "Advanced Turbolaser Tower")
    {
      MaxStrength = 120;
      ImpactDamage = 120;

      RadarSize = 2.5f;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_00.x");

      AddOns = new AddOnInfo[] 
      {
        new AddOnInfo("Turbolaser Turret", new TV_3DVECTOR(95, 155, 0), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnInfo("Turbolaser Turret", new TV_3DVECTOR(-95, 155, 0), new TV_3DVECTOR(0, 0, 0), true)
      };
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      /*
      if (ainfo.ActorState.IsDead())
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower00_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -72, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }
  }
}

