using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Explosions;
using Primrose.FileFormat.INI;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Projectiles;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct SoundSourceData
  {
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public string[] Sound;

    [INIValue]
    public float3 RelativeLocation;

    [INIValue]
    public float Distance;

    [INIValue]
    public bool Loop;

    [INIValue]
    public bool IsEngineSound;

    [INIValue]
    public bool PlayInCutscene;
#pragma warning restore 0649

    public void Process(Engine engine, ActorInfo actor)
    {
      if (engine.GameScenarioManager.IsMainMenu)
        return;

      if (!PlayInCutscene || !engine.GameScenarioManager.Scenario.State.IsCutsceneMode)
      {
        TV_3DVECTOR engineloc = actor.GetRelativePositionXYZ(RelativeLocation.x, RelativeLocation.y, RelativeLocation.z);
        float volMax = 1;
        if (actor.MoveData.MaxSpeed > 0)
          volMax = (actor.MoveData.Speed / actor.MoveData.MaxSpeed).Clamp(0, 1);
        Play(engine, volMax, engineloc);
      }
    }

    public void Process(Engine engine, ProjectileInfo actor)
    {
      if (engine.GameScenarioManager.IsMainMenu)
        return;

      if (!PlayInCutscene || !engine.GameScenarioManager.Scenario.State.IsCutsceneMode)
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
      if (engine.GameScenarioManager.IsMainMenu)
        return;

      if (!PlayInCutscene || !expl.Engine.GameScenarioManager.Scenario.State.IsCutsceneMode)
      {
        TV_3DVECTOR engineloc = expl.GetRelativePositionXYZ(RelativeLocation.x, RelativeLocation.y, RelativeLocation.z);
        Play(engine, 1, engineloc, Distance, Sound, Loop, IsEngineSound);
      }
    }

    public static void Play(Engine engine, float volMax, TV_3DVECTOR pos, float maxDistance, string[] sound, bool loop, bool isEngineSound)
    {
      if (sound == null || sound.Length == 0)
        return;

      if (engine.GameScenarioManager.IsMainMenu)
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
            engine.SoundManager.SetSound(sd, vol, loop);
      }
    }
  }
}
