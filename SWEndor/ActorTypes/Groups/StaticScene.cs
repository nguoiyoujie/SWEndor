using SWEndor.Actors;

namespace SWEndor.ActorTypes.Groups
{
  public class StaticScene : ActorTypeInfo
  {
    internal StaticScene(Factory owner, string name) : base(owner, name)
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      MaxStrength = 100000.0f;
      MaxSpeed = 0.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 0.0f;
      MaxTurnRate = 0.0f;
      EnableDistanceCull = false;
      CollisionEnabled = false;

      Score_perStrength = 0;
      Score_DestroyBonus = 0;

      NoMove = true;
      NoRotate = true;
      NoAI = true;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CombatInfo.DamageModifier = 0.00001f; // check why not 0.
    }
  }
}

