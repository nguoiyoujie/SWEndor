using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct MoveLimitData
  {
    public float MaxSpeed;
    public float MinSpeed;
    public float MaxSpeedChangeRate;
    public float MaxTurnRate;
    public float MaxSecondOrderTurnRateFrac;
    public float XLimit;
    public float ZTilt;
    public float ZNormFrac;

    public readonly static MoveLimitData Default =
        new MoveLimitData
        {
          MaxSecondOrderTurnRateFrac = 0.2f,
          XLimit = 75.0f,
          ZNormFrac = 0.025f,
        };

    public void LoadFromINI(INIFile f, string sectionname)
    {
      MaxSpeed = f.GetFloatValue(sectionname, "MaxSpeed", MaxSpeed);
      MinSpeed = f.GetFloatValue(sectionname, "MinSpeed", MinSpeed);
      MaxSpeedChangeRate = f.GetFloatValue(sectionname, "MaxSpeedChangeRate", MaxSpeedChangeRate);
      MaxTurnRate = f.GetFloatValue(sectionname, "MaxTurnRate", MaxTurnRate);
      MaxSecondOrderTurnRateFrac = f.GetFloatValue(sectionname, "MaxSecondOrderTurnRateFrac", MaxSecondOrderTurnRateFrac);
      XLimit = f.GetFloatValue(sectionname, "XLimit", XLimit);
      ZTilt = f.GetFloatValue(sectionname, "ZTilt", ZTilt);
      ZNormFrac = f.GetFloatValue(sectionname, "ZNormFrac", ZNormFrac);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloatValue(sectionname, "MaxSpeed", MaxSpeed);
      f.SetFloatValue(sectionname, "MinSpeed", MinSpeed);
      f.SetFloatValue(sectionname, "MaxSpeedChangeRate", MaxSpeedChangeRate);
      f.SetFloatValue(sectionname, "MaxTurnRate", MaxTurnRate);
      f.SetFloatValue(sectionname, "MaxSecondOrderTurnRateFrac", MaxSecondOrderTurnRateFrac);
      f.SetFloatValue(sectionname, "XLimit", XLimit);
      f.SetFloatValue(sectionname, "ZTilt", ZTilt);
      f.SetFloatValue(sectionname, "ZNormFrac", ZNormFrac);
    }
  }
}
