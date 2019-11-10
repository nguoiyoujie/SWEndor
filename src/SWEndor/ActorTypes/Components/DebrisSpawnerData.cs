using MTV3D65;
using Primrose.Primitives.ValueTypes;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
using SWEndor.Primitives.Extensions;

namespace SWEndor.ActorTypes.Components
{
  internal struct DebrisSpawnerData
  {
    public readonly string Type;
    private ActorTypeInfo _cache;
    public readonly TV_3DVECTOR SpawnPosition;
    public readonly float RotationXMax;
    public readonly float RotationXMin;
    public readonly float RotationYMax;
    public readonly float RotationYMin;
    public readonly float RotationZMax;
    public readonly float RotationZMin;
    public readonly float Chance;

    public DebrisSpawnerData(string type, TV_3DVECTOR position, float xMin, float xMax, float yMin, float yMax, float zMin, float zMax, float chance)
    {
      Type = type;
      _cache = null;
      SpawnPosition = position;
      RotationXMax = xMax;
      RotationXMin = xMin;
      RotationYMax = yMax;
      RotationYMin = yMin;
      RotationZMax = zMax;
      RotationZMin = zMin;
      Chance = chance;
    }

    public void Process(Engine engine, ActorInfo actor)
    {
      if (_cache == null)
        _cache = engine.ActorTypeFactory.Get(Type);

      double d = engine.Random.NextDouble();
      if (d < Chance)
      {
        float x = RotationXMin + (float)engine.Random.NextDouble() * (RotationXMax - RotationXMin);
        float y = RotationYMin + (float)engine.Random.NextDouble() * (RotationYMax - RotationYMin);
        float z = RotationZMin + (float)engine.Random.NextDouble() * (RotationZMax - RotationZMin);

        ActorCreationInfo acinfo = new ActorCreationInfo(_cache);
        acinfo.Position = actor.GetRelativePositionXYZ(SpawnPosition.x, SpawnPosition.y, SpawnPosition.z);
        acinfo.Rotation = actor.Rotation + new TV_3DVECTOR(x, y, z);
        acinfo.InitialSpeed = actor.MoveData.Speed;
        ActorInfo a = actor.ActorFactory.Create(acinfo);
      }
    }

    public static void LoadFromINI(INIFile f, string sectionname, string sourcename, out DebrisSpawnerData[] dest)
    {
      string[] src = f.GetStringArray(sectionname, sourcename, new string[0]);
      dest = new DebrisSpawnerData[src.Length];
      for (int i = 0; i < src.Length; i++)
        dest[i].LoadFromINI(f, src[i]);
    }

    public static void SaveToINI(INIFile f, string sectionname, string sourcename, DebrisSpawnerData[] src)
    {
      string[] ss = new string[src.Length];
      for (int i = 0; i < src.Length; i++)
      {
        string s = sourcename + i.ToString();
        ss[i] = s;
        src[i].SaveToINI(f, s);
      }
      f.SetStringArray(sectionname, sourcename, ss);
    }

    private void LoadFromINI(INIFile f, string sectionname)
    {
      string type = f.GetString(sectionname, "Type", Type);
      TV_3DVECTOR pos = f.GetTV_3DVECTOR(sectionname, "SpawnPosition", SpawnPosition);
      float xMin = f.GetFloat(sectionname, "RotationXMin", RotationXMin);
      float xMax = f.GetFloat(sectionname, "RotationXMax", RotationXMax);
      float yMin = f.GetFloat(sectionname, "RotationYMin", RotationYMin);
      float yMax = f.GetFloat(sectionname, "RotationYMax", RotationYMax);
      float zMin = f.GetFloat(sectionname, "RotationZMin", RotationZMin);
      float zMax = f.GetFloat(sectionname, "RotationZMax", RotationZMax);
      float chance = f.GetFloat(sectionname, "Chance", Chance);
      this = new DebrisSpawnerData(type, pos, xMin, xMax, yMin, yMax, zMin, zMax, chance);
    }

    private void SaveToINI(INIFile f, string sectionname)
    {
      f.SetString(sectionname, "Type", Type);
      f.GetTV_3DVECTOR(sectionname, "SpawnPosition", SpawnPosition);
      f.SetFloat(sectionname, "RotationXMin", RotationXMin);
      f.SetFloat(sectionname, "RotationXMax", RotationXMax);
      f.SetFloat(sectionname, "RotationYMin", RotationYMin);
      f.SetFloat(sectionname, "RotationYMax", RotationYMax);
      f.SetFloat(sectionname, "RotationZMin", RotationZMin);
      f.SetFloat(sectionname, "RotationZMax", RotationZMax);
      f.SetFloat(sectionname, "Chance", Chance);
    }
  }
}
