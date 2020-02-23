using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Explosions;
using Primitives.FileFormat.INI;
using SWEndor.Projectiles;

namespace SWEndor.ActorTypes.Components
{
  internal struct SoundData
  {
    private const string sSound = "Sound";
    private static SoundSourceData[] NullSound = new SoundSourceData[0];

    [INISubSectionList(sSound, "ISN", "InitialSoundSources")]
    internal SoundSourceData[] InitialSoundSources;

    [INISubSectionList(sSound, "SND", "SoundSources")]
    internal SoundSourceData[] SoundSources;

    public static SoundData Default { get { return new SoundData(NullSound, NullSound); } }

    public SoundData(SoundSourceData[] initsrc, SoundSourceData[] src)
    {
      InitialSoundSources = initsrc;
      SoundSources = src;
    }

    public void ProcessInitial(Engine engine, ActorInfo a)
    {
      foreach (SoundSourceData assi in InitialSoundSources)
        assi.Process(engine, a);
    }

    public void Process(Engine engine, ActorInfo a)
    {
      foreach (SoundSourceData assi in SoundSources)
        assi.Process(engine, a);
    }

    public void ProcessInitial(Engine engine, ExplosionInfo a)
    {
      foreach (SoundSourceData assi in InitialSoundSources)
        assi.Process(engine, a);
    }

    public void Process(Engine engine, ExplosionInfo a)
    {
      foreach (SoundSourceData assi in SoundSources)
        assi.Process(engine, a);
    }

    public void ProcessInitial(Engine engine, ProjectileInfo a)
    {
      foreach (SoundSourceData assi in InitialSoundSources)
        assi.Process(engine, a);
    }

    public void Process(Engine engine, ProjectileInfo a)
    {
      foreach (SoundSourceData assi in SoundSources)
        assi.Process(engine, a);
    }
  }
}
