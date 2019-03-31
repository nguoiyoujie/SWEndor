namespace SWEndor.Actors.Types
{
  public class AsteroidGroup : ActorTypeInfo
  {
    internal AsteroidGroup(string name) : base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = true;
      CullDistance = 4500;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CombatInfo.DamageModifier = 0;
      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "Explosion";
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      ainfo.MovementInfo.ApplyZBalance = false;

      if (!ainfo.IsStateFDefined("RotateAngle"))
      {
        double d = Engine.Instance().Random.NextDouble();

        if (d > 0.5f)
        {
          ainfo.SetStateF("RotateAngle", Engine.Instance().Random.Next(180, 270));
        }
        else
        {
          ainfo.SetStateF("RotateAngle", Engine.Instance().Random.Next(-270, -180));
        }
      }

      if (!ainfo.IsStateFDefined("RotateAngleRate"))
      {
        double d = Engine.Instance().Random.NextDouble() * 2.5;
        ainfo.SetStateF("RotateAngleRate", (float)d);
      }
      float rotZ = ainfo.GetStateF("RotateAngle") * Game.Instance().TimeSinceRender * ainfo.GetStateF("RotateAngleRate");
      ainfo.Rotate(0, 0, rotZ);
      ainfo.MovementInfo.ResetTurn();
    }
  }
}

