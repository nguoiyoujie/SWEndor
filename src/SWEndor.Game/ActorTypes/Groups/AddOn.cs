using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Models;

namespace SWEndor.Game.ActorTypes.Groups
{
  internal class AddOn : ActorTypeInfo
  {
    internal AddOn(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      ExplodeSystemData.Explodes = new ExplodeData[] { new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH) };
      CombatData = CombatData.Disabled;
      ArmorData = ArmorData.Immune;

      RenderData.CullDistance = 3500;

      RenderData.RadarSize = 1;

      Mask = ComponentMask.STATIC_ACTOR;
      AIData.HuntWeight = 1;
    }
  }
}

