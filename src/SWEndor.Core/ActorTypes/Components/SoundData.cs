using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Explosions;
using Primitives.FileFormat.INI;
using SWEndor.Projectiles;

namespace SWEndor.ActorTypes.Components
{
  internal struct SoundData
  {
    private static SoundSourceData[] NullSound = new SoundSourceData[0];
    internal SoundSourceData[] InitialSoundSources;
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

    public void LoadFromINI(INIFile f, string sectionname)
    {
      SoundSourceData.LoadFromINI(f, sectionname, "InitialSoundSources", out InitialSoundSources);
      SoundSourceData.LoadFromINI(f, sectionname, "SoundSources", out SoundSources);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      SoundSourceData.SaveToINI(f, sectionname, "InitialSoundSources", "ISN", InitialSoundSources);
      SoundSourceData.SaveToINI(f, sectionname, "SoundSources", "SND", SoundSources);
    }
  }
}
