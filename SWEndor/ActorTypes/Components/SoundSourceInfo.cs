using MTV3D65;
using SWEndor.Actors;

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
      if (!PlayInCutscene || !actor.GameScenarioManager.IsCutsceneMode)
      {
        TV_3DVECTOR engineloc = actor.GetRelativePositionXYZ(RelativeLocation.x, RelativeLocation.y, RelativeLocation.z);
        Play(actor, actor.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.PlayerInfo.Position, engineloc));
      }
    }

    public void Play(ActorInfo actor, float dist)
    {
      if (dist < Distance)
      {
        float vol = actor.MoveComponent.Speed / actor.MoveComponent.MaxSpeed;
        vol = vol.Clamp(0, 1);
        vol -= dist / Distance;
        if (vol > 0)
          actor.SoundManager.SetSound(Sound, false, vol, Loop);
      }
    }
  }
}
