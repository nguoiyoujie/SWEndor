using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes.Components
{
  public struct SoundSourceData
  {
    public readonly string[] Sound;
    public readonly TV_3DVECTOR RelativeLocation;
    public readonly float Distance;
    public readonly bool Loop;
    public readonly bool PlayInCutscene;

    public SoundSourceData(string sound, float dist, TV_3DVECTOR position = default(TV_3DVECTOR), bool loop = false, bool playInCutscene = false)
    {
      Sound = new string[] { sound };
      RelativeLocation = position;
      Distance = dist;
      Loop = loop;
      PlayInCutscene = playInCutscene;
    }

    public SoundSourceData(string[] sounds, float dist, TV_3DVECTOR position = default(TV_3DVECTOR), bool loop = false, bool playInCutscene = false)
    {
      Sound = sounds;
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
        Play(actor, engineloc);
      }
    }

    public void Play(ActorInfo actor, TV_3DVECTOR pos)
    {
      Play(actor, pos, Distance, Sound, Loop);
    }

    public static void Play(ActorInfo actor, TV_3DVECTOR pos, float maxDistance, string[] sound, bool loop)
    {
      if (sound == null || sound.Length == 0)
        return;

      float df = actor.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.PlayerInfo.Position, pos) / maxDistance;

      if (df < 1)
      {
        string sd = null;
        if (sound.Length == 1)
          sd = sound[0];
        else if (sound.Length > 1)
          sd = sound[actor.Engine.Random.Next(0, sound.Length)];

        float vol = 1;
        if (actor.MoveData.MaxSpeed > 0)
          vol = (actor.MoveData.Speed / actor.MoveData.MaxSpeed).Clamp(0, 1);
        vol -= df;
        if (vol > 0)
          actor.SoundManager.SetSoundSingle(sd, false, vol, loop);
      }
    }
  }
}
