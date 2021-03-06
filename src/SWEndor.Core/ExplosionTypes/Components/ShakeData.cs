﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
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
