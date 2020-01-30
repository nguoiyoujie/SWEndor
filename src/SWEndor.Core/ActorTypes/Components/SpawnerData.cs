using MTV3D65;
using Primitives.FileFormat.INI;
using SWEndor.Primitives.Extensions;

namespace SWEndor.ActorTypes.Components
{
  internal struct SpawnerData
  {
    public string[] SpawnTypes;
    public float SpawnMoveDelay;
    public float SpawnInterval;
    public float SpawnPlayerDelay;
    public int SpawnsRemaining;

    public TV_3DVECTOR[] SpawnLocations;
    public TV_3DVECTOR PlayerSpawnLocation;

    public float SpawnSpeed; // -1 means follow spawner, -2 means maxSpeed of spawned
    public TV_3DVECTOR SpawnRotation;
    public TV_3DVECTOR SpawnManualPositioningMult;
    public TV_3DVECTOR SpawnSpeedPositioningMult;

    public static SpawnerData Default = new SpawnerData
    {
      SpawnTypes = new string[0],
      SpawnMoveDelay = 4,
      SpawnInterval = 5,
      SpawnPlayerDelay = 10,
      SpawnsRemaining = 30,
      SpawnLocations = new TV_3DVECTOR[0],
      SpawnSpeed = -1
    };

    public void LoadFromINI(INIFile f, string sectionname)
    {
      SpawnTypes = f.GetStringArray(sectionname, "SpawnTypes", SpawnTypes);
      SpawnMoveDelay = f.GetFloat(sectionname, "SpawnMoveDelay", SpawnMoveDelay);
      SpawnInterval = f.GetFloat(sectionname, "SpawnInterval", SpawnInterval);
      SpawnPlayerDelay = f.GetFloat(sectionname, "SpawnPlayerDelay", SpawnPlayerDelay);
      SpawnsRemaining = f.GetInt(sectionname, "SpawnsRemaining", SpawnsRemaining);

      float[] spos = f.GetFloatArray(sectionname, "SpawnLocations", new float[0]);
      SpawnLocations = new TV_3DVECTOR[spos.Length / 3];
      for (int p = 0; p + 2 < spos.Length; p += 3)
        SpawnLocations[p / 3] = new TV_3DVECTOR(spos[p], spos[p + 1], spos[p + 2]);

      PlayerSpawnLocation = f.GetFloat3(sectionname, "PlayerSpawnLocation", PlayerSpawnLocation.ToFloat3()).ToVec3();
      SpawnSpeed = f.GetFloat(sectionname, "SpawnSpeed", SpawnSpeed);
      SpawnRotation = f.GetFloat3(sectionname, "SpawnRotation", SpawnRotation.ToFloat3()).ToVec3();
      SpawnManualPositioningMult = f.GetFloat3(sectionname, "SpawnManualPositioningMult", SpawnManualPositioningMult.ToFloat3()).ToVec3();
      SpawnSpeedPositioningMult = f.GetFloat3(sectionname, "SpawnSpeedPositioningMult", SpawnSpeedPositioningMult.ToFloat3()).ToVec3();
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetStringArray(sectionname, "SpawnTypes", SpawnTypes);
      f.SetFloat(sectionname, "SpawnMoveDelay", SpawnMoveDelay);
      f.SetFloat(sectionname, "SpawnInterval", SpawnInterval);
      f.SetFloat(sectionname, "SpawnPlayerDelay", SpawnPlayerDelay);
      f.SetInt(sectionname, "SpawnsRemaining", SpawnsRemaining);

      float[] spos = new float[SpawnLocations.Length * 3];
      for (int i = 0; i < SpawnLocations.Length; i++)
      {
        spos[i * 3] = SpawnLocations[i].x;
        spos[i * 3 + 1] = SpawnLocations[i].y;
        spos[i * 3 + 2] = SpawnLocations[i].z;
      }
      f.SetFloatArray(sectionname, "SpawnLocations", spos);

      f.SetFloat3(sectionname, "PlayerSpawnLocation", PlayerSpawnLocation.ToFloat3());
      f.SetFloat(sectionname, "SpawnSpeed", SpawnSpeed);
      f.SetFloat3(sectionname, "SpawnRotation", SpawnRotation.ToFloat3());
      f.SetFloat3(sectionname, "SpawnManualPositioningMult", SpawnManualPositioningMult.ToFloat3());
      f.SetFloat3(sectionname, "SpawnSpeedPositioningMult", SpawnSpeedPositioningMult.ToFloat3());
    }
  }
}

