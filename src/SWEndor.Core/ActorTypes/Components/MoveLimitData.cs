using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct MoveLimitData
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
      MaxSpeed = f.GetFloat(sectionname, "MaxSpeed", MaxSpeed);
      MinSpeed = f.GetFloat(sectionname, "MinSpeed", MinSpeed);
      MaxSpeedChangeRate = f.GetFloat(sectionname, "MaxSpeedChangeRate", MaxSpeedChangeRate);
      MaxTurnRate = f.GetFloat(sectionname, "MaxTurnRate", MaxTurnRate);
      MaxSecondOrderTurnRateFrac = f.GetFloat(sectionname, "MaxSecondOrderTurnRateFrac", MaxSecondOrderTurnRateFrac);
      XLimit = f.GetFloat(sectionname, "XLimit", XLimit);
      ZTilt = f.GetFloat(sectionname, "ZTilt", ZTilt);
      ZNormFrac = f.GetFloat(sectionname, "ZNormFrac", ZNormFrac);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloat(sectionname, "MaxSpeed", MaxSpeed);
      f.SetFloat(sectionname, "MinSpeed", MinSpeed);
      f.SetFloat(sectionname, "MaxSpeedChangeRate", MaxSpeedChangeRate);
      f.SetFloat(sectionname, "MaxTurnRate", MaxTurnRate);
      f.SetFloat(sectionname, "MaxSecondOrderTurnRateFrac", MaxSecondOrderTurnRateFrac);
      f.SetFloat(sectionname, "XLimit", XLimit);
      f.SetFloat(sectionname, "ZTilt", ZTilt);
      f.SetFloat(sectionname, "ZNormFrac", ZNormFrac);
    }
  }
}
