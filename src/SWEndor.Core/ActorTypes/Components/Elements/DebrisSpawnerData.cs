﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.ActorTypes.Components
{
  internal struct DebrisSpawnerData
  {
    private const string sNone = "";

    private ActorTypeInfo _cache;

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue(sNone, "Type")]
    public string Type;

    [INIValue(sNone, "SpawnPosition")]
    public float3 SpawnPosition;

    [INIValue(sNone, "RotationXMax")]
    public float RotationXMax;

    [INIValue(sNone, "RotationXMin")]
    public float RotationXMin;

    [INIValue(sNone, "RotationYMax")]
    public float RotationYMax;

    [INIValue(sNone, "RotationYMin")]
    public float RotationYMin;

    [INIValue(sNone, "RotationZMax")]
    public float RotationZMax;

    [INIValue(sNone, "RotationZMin")]
    public float RotationZMin;

    [INIValue(sNone, "Chance")]
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
