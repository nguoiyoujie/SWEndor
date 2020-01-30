using SWEndor.Actors;
using SWEndor.Core;
using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct DebrisSystemData
  {
    private static DebrisSpawnerData[] NullDebris = new DebrisSpawnerData[0];
    internal DebrisSpawnerData[] Debris;

    public static DebrisSystemData Default { get { return new DebrisSystemData(NullDebris); } }

    public DebrisSystemData(DebrisSpawnerData[] debris)
    {
      Debris = debris;
    }

    public void Process(Engine e, ActorInfo a)
    {
      foreach (DebrisSpawnerData ds in Debris)
        ds.Process(e, a);
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      DebrisSpawnerData.LoadFromINI(f, sectionname, "Debris", out Debris);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      DebrisSpawnerData.SaveToINI(f, sectionname, "Debris", Debris);
    }
  }
}
