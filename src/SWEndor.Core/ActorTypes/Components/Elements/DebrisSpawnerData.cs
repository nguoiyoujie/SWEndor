using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using Primrose.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.ActorTypes.Components
{
  internal struct DebrisSpawnerData
  {
    private ActorTypeInfo _cache;

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public string Type;

    [INIValue]
    public float3 SpawnPosition;

    [INIValue]
    public float RotationXMax;

    [INIValue]
    public float RotationXMin;

    [INIValue]
    public float RotationYMax;

    [INIValue]
    public float RotationYMin;

    [INIValue]
    public float RotationZMax;

    [INIValue]
    public float RotationZMin;

    [INIValue]
    public float Chance;
#pragma warning restore 0649

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
        a.SetState_Dying();
      }
    }
  }
}
