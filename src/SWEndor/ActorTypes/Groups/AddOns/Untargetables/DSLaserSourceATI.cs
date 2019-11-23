using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Weapons;

namespace SWEndor.ActorTypes.Instances
{
  internal class DSLaserSourceATI : Groups.Turbolasers //ActorTypeInfo //: AddOnGroup
  {
    internal DSLaserSourceATI(Factory owner) : base(owner, "DSLSRSRC", "Death Star Laser Source")
    {
      SystemData.MaxShield = 1500.0f;
      CombatData.ImpactDamage = 4000.0f;

      AIData.TargetType = TargetType.NULL;
      RenderData.RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);

      Loadouts = new WeapData[] { new WeapData("LASR", "PRI_1_AI", "AI_AUTOAIM", "DEFAULT", "DSTAR_LSR", "DEATH_STAR", "DSTAR_LSR") };
    }
  }
}

