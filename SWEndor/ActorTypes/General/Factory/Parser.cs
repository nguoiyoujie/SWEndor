using SWEndor.FileFormat.INI;
using System;

  namespace SWEndor.ActorTypes
  {
  public partial class ActorTypeInfo
  {
    public static class Parser
    {
      public static ActorTypeInfo Parse(Factory factory, INIFile file, string sectionname)
      {
        ActorTypeInfo at = new ActorTypeInfo(factory, sectionname);

        //at.CollisionEnabled = file.GetBoolValue(sectionname, "CollisionEnabled", at.CollisionEnabled);
        /*
        at.CombatData.IsCombatObject = file.GetBoolValue(sectionname, "IsCombatObject", at.CombatData.IsCombatObject);

        //at.IsSelectable = file.GetBoolValue(sectionname, "IsSelectable", at.IsSelectable);
        //at.IsDamage = file.GetBoolValue(sectionname, "IsDamage", at.IsDamage);
        at.MaxStrength = file.GetFloatValue(sectionname, "MaxStrength", at.MaxStrength);
        at.ImpactDamage = file.GetFloatValue(sectionname, "ImpactDamage", at.ImpactDamage);

        at.TimedLifeData.OnTimedLife = file.GetBoolValue(sectionname, "OnTimedLife", at.TimedLifeData.OnTimedLife);
        at.TimedLifeData.TimedLife = file.GetFloatValue(sectionname, "TimedLife", at.TimedLifeData.TimedLife);

        at.MaxSpeed = file.GetFloatValue(sectionname, "MaxSpeed", at.MaxSpeed);
        at.MinSpeed = file.GetFloatValue(sectionname, "MinSpeed", at.MinSpeed);
        at.MaxSpeedChangeRate = file.GetFloatValue(sectionname, "MaxSpeedChangeRate", at.MaxSpeedChangeRate);

        at.MaxTurnRate = file.GetFloatValue(sectionname, "MaxTurnRate", at.MaxTurnRate);
        at.MaxSecondOrderTurnRateFrac = file.GetFloatValue(sectionname, "MaxSecondOrderTurnRateFrac", at.MaxSecondOrderTurnRateFrac);
        at.XLimit = file.GetFloatValue(sectionname, "XLimit", at.XLimit);
        at.ZTilt = file.GetFloatValue(sectionname, "ZTilt", at.ZTilt);
        at.ZNormFrac = file.GetFloatValue(sectionname, "ZNormFrac", at.ZNormFrac);

        at.EnableDistanceCull = file.GetBoolValue(sectionname, "EnableDistanceCull", at.EnableDistanceCull);
        at.CullDistance = file.GetFloatValue(sectionname, "CullDistance", at.CullDistance);

        at.Attack_AngularDelta = file.GetFloatValue(sectionname, "Attack_AngularDelta", at.Attack_AngularDelta);
        at.Attack_HighAccuracyAngularDelta = file.GetFloatValue(sectionname, "Attack_HighAccuracyAngularDelta", at.Attack_HighAccuracyAngularDelta);
        at.Move_CloseEnough = file.GetFloatValue(sectionname, "Move_CloseEnough", at.Move_CloseEnough);

        at.AggressiveTracker = file.GetBoolValue(sectionname, "AggressiveTracker", at.AggressiveTracker);
        at.AlwaysAccurateRotation = file.GetBoolValue(sectionname, "AlwaysAccurateRotation", at.AlwaysAccurateRotation);

        at.Score_perStrength = file.GetIntValue(sectionname, "Score_perStrength", at.Score_perStrength);
        at.Score_DestroyBonus = file.GetIntValue(sectionname, "Score_DestroyBonus", at.Score_DestroyBonus);

        at.CanEvade = file.GetBoolValue(sectionname, "CanEvade", at.CanEvade);
        at.CanRetaliate = file.GetBoolValue(sectionname, "CanRetaliate", at.CanRetaliate);
        at.CanCheckCollisionAhead = file.GetBoolValue(sectionname, "CanCheckCollisionAhead", at.CanCheckCollisionAhead);

        at.EnableDistanceCull = file.GetBoolValue(sectionname, "EnableDistanceCull", at.EnableDistanceCull);

        string[] tgts = file.GetStringValue(sectionname, "AIAttackTargets", at.TargetType.ToString()).Split('|', ',');
        TargetType tgtt = TargetType.NULL;
        foreach (string t in tgts)
          tgtt |= (TargetType)Enum.Parse(typeof(TargetType), t.Trim());
        at.TargetType = tgtt;
        at.HuntWeight = file.GetIntValue(sectionname, "HuntWeight", at.HuntWeight);

        at.SourceMeshPath = file.GetStringValue(sectionname, "SourceMeshPath", at.SourceMeshPath);
        at.SourceFarMeshPath = file.GetStringValue(sectionname, "SourceFarMeshPath", at.SourceFarMeshPath);

        //at.AnimationCyclePeriod = file.GetFloatValue(sectionname, "AnimationCyclePeriod", at.AnimationCyclePeriod);
        at.RadarSize = file.GetFloatValue(sectionname, "RadarSize", at.RadarSize);
        at.AlwaysShowInRadar = file.GetBoolValue(sectionname, "AlwaysShowInRadar", at.AlwaysShowInRadar);

        //at.NoRender = file.GetBoolValue(sectionname, "NoRender", at.NoRender);
        //at.NoProcess = file.GetBoolValue(sectionname, "NoProcess", at.NoProcess);
        //at.NoMove = file.GetBoolValue(sectionname, "NoMove", at.NoMove);
        //at.NoAI = file.GetBoolValue(sectionname, "NoAI", at.NoAI);


        // Debris
        //public List<DebrisSpawner> Debris = new List<DebrisSpawner>();

        // Sound
        //public List<ActorSoundSourceInfo> SoundSources = new List<ActorSoundSourceInfo>();
        */

        return at;
      }
    }
  }
}
