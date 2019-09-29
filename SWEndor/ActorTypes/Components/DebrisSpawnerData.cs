using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct DebrisSpawnerData
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
        acinfo.Position = actor.GetGlobalPosition() + SpawnPosition;
        acinfo.Rotation = actor.Rotation + new TV_3DVECTOR(x, y, z);
        acinfo.InitialSpeed = actor.MoveData.Speed;
        //acinfo.InitialState = ActorState.DYING;
        ActorInfo a = actor.ActorFactory.Create( acinfo);
      }
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      string type = f.GetStringValue(sectionname, "Type", Type);
      TV_3DVECTOR pos = f.GetTV_3DVECTOR(sectionname, "SpawnPosition", SpawnPosition);
      float xMin = f.GetFloatValue(sectionname, "RotationXMin", RotationXMin);
      float xMax = f.GetFloatValue(sectionname, "RotationXMax", RotationXMax);
      float yMin = f.GetFloatValue(sectionname, "RotationYMin", RotationYMin);
      float yMax = f.GetFloatValue(sectionname, "RotationYMax", RotationYMax);
      float zMin = f.GetFloatValue(sectionname, "RotationZMin", RotationZMin);
      float zMax = f.GetFloatValue(sectionname, "RotationZMax", RotationZMax);
      float chance = f.GetFloatValue(sectionname, "Chance", Chance);
      this = new DebrisSpawnerData(type, pos, xMin, xMax, yMin, yMax, zMin, zMax, chance);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetStringValue(sectionname, "Type", Type);
      f.SetTV_3DVECTOR(sectionname, "SpawnPosition", SpawnPosition);
      f.SetFloatValue(sectionname, "RotationXMin", RotationXMin);
      f.SetFloatValue(sectionname, "RotationXMax", RotationXMax);
      f.SetFloatValue(sectionname, "RotationYMin", RotationYMin);
      f.SetFloatValue(sectionname, "RotationYMax", RotationYMax);
      f.SetFloatValue(sectionname, "RotationZMin", RotationZMin);
      f.SetFloatValue(sectionname, "RotationZMax", RotationZMax);
      f.SetFloatValue(sectionname, "Chance", Chance);
    }
  }
}
