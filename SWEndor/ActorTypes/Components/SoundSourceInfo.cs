using MTV3D65;
using SWEndor.Actors;
using SWEndor.Scenarios;
using SWEndor.Sound;
using System;

namespace SWEndor.ActorTypes.Components
{
  public class SoundSourceInfo
  {
    public string Sound;
    public TV_3DVECTOR RelativeLocation;
    public float Distance;
    public bool Loop;
    public bool PlayInCutscene;

    public SoundSourceInfo(string sound, float dist)
    {
      Sound = sound;
      Distance = dist;
    }

    public SoundSourceInfo(string sound, TV_3DVECTOR position, float dist, bool loop, bool playInCutscene = false)
    {
      Sound = sound;
      RelativeLocation = position;
      Distance = dist;
      Loop = loop;
      PlayInCutscene = playInCutscene;
    }

    public void Process(ActorInfo actor)
    {
      if (!PlayInCutscene || !GameScenarioManager.Instance().IsCutsceneMode)
      {
        TV_3DVECTOR engineloc = actor.GetRelativePositionXYZ(RelativeLocation.x, RelativeLocation.y, RelativeLocation.z);
        Play(Engine.Instance().TVMathLibrary.GetDistanceVec3D(PlayerInfo.Instance().Position, engineloc));
      }
    }

    public void Play(float dist)
    {
      if (dist < Distance)
        SoundManager.Instance().SetSound(Sound, false, 1 - dist / Distance, Loop);
    }
  }
}
