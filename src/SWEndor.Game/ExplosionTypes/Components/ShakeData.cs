using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct ShakeData
  {
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public float Intensity;

    [INIValue]
    public float ProximityDistance;
#pragma warning restore 0649

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
  }
}
