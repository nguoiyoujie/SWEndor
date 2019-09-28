using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.Scenarios;

namespace SWEndor.ActorTypes.Components
{
  public struct SoundSourceData
  {
    public readonly string[] Sound;
    public readonly TV_3DVECTOR RelativeLocation;
    public readonly float Distance;
    public readonly bool Loop;
    public readonly bool IsEngineSound;
    public readonly bool PlayInCutscene;

    public SoundSourceData(string sound, float dist, TV_3DVECTOR position = default(TV_3DVECTOR), bool loop = false, bool playInCutscene = false, bool isEngineSound = false)
    {
      Sound = new string[] { sound };
      RelativeLocation = position;
      Distance = dist;
      Loop = loop;
      PlayInCutscene = playInCutscene;
      IsEngineSound = isEngineSound;
    }

    public SoundSourceData(string[] sounds, float dist, TV_3DVECTOR position = default(TV_3DVECTOR), bool loop = false, bool playInCutscene = false, bool isEngineSound = false)
    {
      Sound = sounds;
      RelativeLocation = position;
      Distance = dist;
      Loop = loop;
      PlayInCutscene = playInCutscene;
      IsEngineSound = isEngineSound;
    }

    public void Process(Engine engine, ActorInfo actor)
    {
      if (engine.GameScenarioManager.Scenario is GSMainMenu)
        return;

      if (!PlayInCutscene || !actor.Engine.GameScenarioManager.IsCutsceneMode)
      {
        TV_3DVECTOR engineloc = actor.GetRelativePositionXYZ(RelativeLocation.x, RelativeLocation.y, RelativeLocation.z);
        float volMax = 1;
        if (actor.MoveData.MaxSpeed > 0)
          volMax = (actor.MoveData.Speed / actor.MoveData.MaxSpeed).Clamp(0, 1);
        Play(engine, volMax, engineloc);
      }
    }

    private void Play(Engine engine, float volMax, TV_3DVECTOR pos)
    {
      Play(engine, volMax, pos, Distance, Sound, Loop, IsEngineSound);
    }

    public void Process(Engine engine, ExplosionInfo expl)
    {
      if (engine.GameScenarioManager.Scenario is GSMainMenu)
        return;

      if (!PlayInCutscene || !expl.Engine.GameScenarioManager.IsCutsceneMode)
      {
        TV_3DVECTOR engineloc = expl.GetRelativePositionXYZ(RelativeLocation.x, RelativeLocation.y, RelativeLocation.z);
        Play(engine, 1, engineloc, Distance, Sound, Loop, IsEngineSound);
      }
    }

    public static void Play(Engine engine, float volMax, TV_3DVECTOR pos, float maxDistance, string[] sound, bool loop, bool isEngineSound)
    {
      if (sound == null || sound.Length == 0)
        return;

      if (engine.GameScenarioManager.Scenario is GSMainMenu)
        return;

      float df = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(engine.PlayerCameraInfo.Position, pos) / maxDistance;

      if (df < 1)
      {
        string sd = null;
        if (sound.Length == 1)
          sd = sound[0];
        else
          sd = sound[engine.Random.Next(0, sound.Length)];

        float vol = volMax.Clamp(0, 1);
        vol -= df;
        if (vol > 0)
          if (isEngineSound)
            engine.SoundManager.SetSoundSingle(sd, false, vol, loop);
          else
            engine.SoundManager.SetSound(sd, false, vol, loop);
      }
    }
  }
}
