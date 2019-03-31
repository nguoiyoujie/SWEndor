namespace SWEndor.Actors.Types
{
  public class StaticSceneGroup : ActorTypeInfo
  {
    internal StaticSceneGroup(string name) : base(name)
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
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.CombatInfo.DamageModifier = 0.00001f; // check why not 0.
    }
  }
}

