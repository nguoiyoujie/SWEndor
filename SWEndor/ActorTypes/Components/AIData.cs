using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct AIData
  {
    // AI
    public float Attack_AngularDelta;
    public float Attack_HighAccuracyAngularDelta;
    public float Move_CloseEnough;

    public bool AggressiveTracker;
    public bool AlwaysAccurateRotation;

    public bool CanEvade;
    public bool CanRetaliate;
    public bool CanCheckCollisionAhead;

    // Targeting
    public TargetType TargetType;
    public int HuntWeight;

    // Projectiles

    public float ImpactCloseEnoughDistance;

    public readonly static AIData Default = 
      new AIData
        {
          Attack_AngularDelta = 5f,
          Attack_HighAccuracyAngularDelta = 1f,
          Move_CloseEnough = 500,
          TargetType = TargetType.NULL,
          HuntWeight = 1
        };

    public void LoadFromINI(INIFile f, string sectionname)
    {
      Attack_AngularDelta = f.GetFloatValue(sectionname, "Attack_AngularDelta", Attack_AngularDelta);
      Attack_HighAccuracyAngularDelta = f.GetFloatValue(sectionname, "Attack_HighAccuracyAngularDelta", Attack_HighAccuracyAngularDelta);
      Move_CloseEnough = f.GetFloatValue(sectionname, "Move_CloseEnough", Move_CloseEnough);

      AggressiveTracker = f.GetBoolValue(sectionname, "AggressiveTracker", AggressiveTracker);
      AlwaysAccurateRotation = f.GetBoolValue(sectionname, "AlwaysAccurateRotation", AlwaysAccurateRotation);

      CanEvade = f.GetBoolValue(sectionname, "CanEvade", CanEvade);
      CanRetaliate = f.GetBoolValue(sectionname, "CanRetaliate", CanRetaliate);
      CanCheckCollisionAhead = f.GetBoolValue(sectionname, "CanCheckCollisionAhead", CanCheckCollisionAhead);

      TargetType = f.GetEnumValue(sectionname, "TargetType", TargetType);
      HuntWeight = f.GetIntValue(sectionname, "HuntWeight", HuntWeight);

      Move_CloseEnough = f.GetFloatValue(sectionname, "ImpactCloseEnoughDistance", ImpactCloseEnoughDistance);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloatValue(sectionname, "Attack_AngularDelta", Attack_AngularDelta);
      f.SetFloatValue(sectionname, "Attack_HighAccuracyAngularDelta", Attack_HighAccuracyAngularDelta);
      f.SetFloatValue(sectionname, "Move_CloseEnough", Move_CloseEnough);

      f.SetBoolValue(sectionname, "AggressiveTracker", AggressiveTracker);
      f.SetBoolValue(sectionname, "AlwaysAccurateRotation", AlwaysAccurateRotation);

      f.SetBoolValue(sectionname, "CanEvade", CanEvade);
      f.SetBoolValue(sectionname, "CanRetaliate", CanRetaliate);
      f.SetBoolValue(sectionname, "CanCheckCollisionAhead", CanCheckCollisionAhead);

      f.SetEnumValue(sectionname, "TargetType", TargetType);
      f.SetIntValue(sectionname, "HuntWeight", HuntWeight);

      f.SetFloatValue(sectionname, "ImpactCloseEnoughDistance", ImpactCloseEnoughDistance);
    }
  }
}

