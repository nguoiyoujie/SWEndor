using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.FileFormat.INI;
using Primrose.Primitives.Extensions;
using SWEndor.Projectiles;
using SWEndor.Scenarios;
using SWEndor.Primitives.Extensions;

namespace SWEndor.ActorTypes.Components
{
  internal struct SoundSourceData
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
            engine.SoundManager.SetSound(sd, false, vol, loop);
      }
    }

    public static void LoadFromINI(INIFile f, string sectionname, string key, out SoundSourceData[] dest)
    {
      string[] src = f.GetStringArray(sectionname, key, new string[0]);
      dest = new SoundSourceData[src.Length];
      for (int i = 0; i < src.Length; i++)
        dest[i].LoadFromINI(f, src[i]);
    }

    public static void SaveToINI(INIFile f, string sectionname, string key, string membername, SoundSourceData[] src)
    {
      string[] ss = new string[src.Length];
      for (int i = 0; i < src.Length; i++)
      {
        string s = membername + i.ToString();
        ss[i] = s;
        src[i].SaveToINI(f, s);
      }
      f.SetStringArray(sectionname, key, ss);
    }

    private void LoadFromINI(INIFile f, string sectionname)
    {
      string[] sound = f.GetStringArray(sectionname, "Sound", Sound);
      TV_3DVECTOR rloc = f.GetTV_3DVECTOR(sectionname, "RelativeLocation", RelativeLocation);
      float dist = f.GetFloat(sectionname, "Distance", Distance);
      bool loop = f.GetBool(sectionname, "Loop", Loop);
      bool engs = f.GetBool(sectionname, "IsEngineSound", IsEngineSound);
      bool cuts = f.GetBool(sectionname, "PlayInCutscene", PlayInCutscene);
      this = new SoundSourceData(sound, dist, rloc, loop, cuts, engs);
    }

    private void SaveToINI(INIFile f, string sectionname)
    {
      f.SetStringArray(sectionname, "Sound", Sound);
      f.SetTV_3DVECTOR(sectionname, "RelativeLocation", RelativeLocation);
      f.SetFloat(sectionname, "Distance", Distance);
      f.SetBool(sectionname, "Loop", Loop);
      f.SetBool(sectionname, "IsEngineSound", IsEngineSound);
      f.SetBool(sectionname, "PlayInCutscene", PlayInCutscene);
    }
  }
}
