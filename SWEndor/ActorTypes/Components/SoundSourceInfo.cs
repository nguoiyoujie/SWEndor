using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;
using SWEndor.Scenarios;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Components
{
  public struct SoundSourceInfo
  {
    public readonly string Sound;
    public readonly TV_3DVECTOR RelativeLocation;
    public readonly float Distance;
    public readonly bool Loop;
    public readonly bool PlayInCutscene;

    public SoundSourceInfo(string sound, float dist, TV_3DVECTOR position = default(TV_3DVECTOR), bool loop = false, bool playInCutscene = false)
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
