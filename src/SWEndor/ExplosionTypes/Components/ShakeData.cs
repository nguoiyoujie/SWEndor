using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct ShakeData
  {
    public readonly float Intensity;
    public readonly float ProximityDistance;

    public ShakeData(float intensity, float dist)
    {
      Intensity = intensity;
      ProximityDistance = dist;
    }

    public void Process(Engine engine, TV_3DVECTOR position)
    {
      if (Intensity > 0)
      {
        if (ProximityDistance > 0)
        {
          ActorInfo p = engine.PlayerInfo.Actor;
          if (p != null && !p.IsDyingOrDead)
            engine.PlayerCameraInfo.ProximityShake(Intensity, ProximityDistance, position);
        }
        else
          engine.PlayerCameraInfo.Shake(Intensity);
      }
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      float intensity = f.GetFloat(sectionname, "Intensity", Intensity);
      float dist = f.GetFloat(sectionname, "ProximityDistance", ProximityDistance);
      this = new ShakeData(intensity, dist);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloat(sectionname, "Intensity", Intensity);
      f.SetFloat(sectionname, "ProximityDistance", ProximityDistance);
    }
  }
}
