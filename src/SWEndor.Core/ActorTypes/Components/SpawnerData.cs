using Primitives.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.ActorTypes.Components
{
  internal struct SpawnerData
  {
    private const string sSpawner = "Spawner";

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue(sSpawner, "SpawnTypes")]
    public string[] SpawnTypes;

    [INIValue(sSpawner, "SpawnMoveDelay")]
    public float SpawnMoveDelay;

    [INIValue(sSpawner, "SpawnInterval")]
    public float SpawnInterval;

    [INIValue(sSpawner, "SpawnPlayerDelay")]
    public float SpawnPlayerDelay;

    [INIValue(sSpawner, "SpawnsRemaining")]
    public int SpawnsRemaining;

    [INIValue(sSpawner, "SpawnLocations")]
    public float3[] SpawnLocations;

    [INIValue(sSpawner, "PlayerSpawnLocation")]
    public float3 PlayerSpawnLocation;

    [INIValue(sSpawner, "SpawnSpeed")]
    public float SpawnSpeed; // -1 means follow spawner, -2 means maxSpeed of spawned

    [INIValue(sSpawner, "SpawnRotation")]
    public float3 SpawnRotation;

    [INIValue(sSpawner, "SpawnManualPositioningMult")]
    public float3 SpawnManualPositioningMult;

    [INIValue(sSpawner, "SpawnSpeedPositioningMult")]
    public float3 SpawnSpeedPositioningMult;
#pragma warning restore 0649

    public static SpawnerData Default = new SpawnerData
    {
      SpawnTypes = new string[0],
      SpawnMoveDelay = 4,
      SpawnInterval = 5,
      SpawnPlayerDelay = 10,
      SpawnsRemaining = 30,
      SpawnLocations = new float3[0],
      SpawnSpeed = -1
    };
    /*
    public void LoadFromINI(INIFile f, string sectionname)
    {
      SpawnTypes = f.GetStringArray(sectionname, "SpawnTypes", SpawnTypes);
      SpawnMoveDelay = f.GetFloat(sectionname, "SpawnMoveDelay", SpawnMoveDelay);
      SpawnInterval = f.GetFloat(sectionname, "SpawnInterval", SpawnInterval);
      SpawnPlayerDelay = f.GetFloat(sectionname, "SpawnPlayerDelay", SpawnPlayerDelay);
      SpawnsRemaining = f.GetInt(sectionname, "SpawnsRemaining", SpawnsRemaining);

      float[] spos = f.GetFloatArray(sectionname, "SpawnLocations", new float[0]);
      SpawnLocations = new float3[spos.Length / 3];
      for (int p = 0; p + 2 < spos.Length; p += 3)
        SpawnLocations[p / 3] = new float3(spos[p], spos[p + 1], spos[p + 2]);

      PlayerSpawnLocation = f.GetFloat3(sectionname, "PlayerSpawnLocation", PlayerSpawnLocation);
      SpawnSpeed = f.GetFloat(sectionname, "SpawnSpeed", SpawnSpeed);
      SpawnRotation = f.GetFloat3(sectionname, "SpawnRotation", SpawnRotation);
      SpawnManualPositioningMult = f.GetFloat3(sectionname, "SpawnManualPositioningMult", SpawnManualPositioningMult);
      SpawnSpeedPositioningMult = f.GetFloat3(sectionname, "SpawnSpeedPositioningMult", SpawnSpeedPositioningMult);
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

      f.SetFloat3(sectionname, "PlayerSpawnLocation", PlayerSpawnLocation);
      f.SetFloat(sectionname, "SpawnSpeed", SpawnSpeed);
      f.SetFloat3(sectionname, "SpawnRotation", SpawnRotation);
      f.SetFloat3(sectionname, "SpawnManualPositioningMult", SpawnManualPositioningMult);
      f.SetFloat3(sectionname, "SpawnSpeedPositioningMult", SpawnSpeedPositioningMult);
    }
    */
  }
}

