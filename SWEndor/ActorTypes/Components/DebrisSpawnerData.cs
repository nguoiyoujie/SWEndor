using MTV3D65;
using SWEndor.Actors;

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

    public void Process(ActorInfo actor)
    {
      if (_cache == null)
        _cache = actor.ActorTypeFactory.Get(Type);

      double d = actor.Engine.Random.NextDouble();
      if (d < Chance)
      {
        float x = RotationXMin + (float)actor.Engine.Random.NextDouble() * (RotationXMax - RotationXMin);
        float y = RotationYMin + (float)actor.Engine.Random.NextDouble() * (RotationYMax - RotationYMin);
        float z = RotationZMin + (float)actor.Engine.Random.NextDouble() * (RotationZMax - RotationZMin);

        ActorCreationInfo acinfo = new ActorCreationInfo(_cache);
        acinfo.Position = actor.GetGlobalPosition() + SpawnPosition;
        acinfo.Rotation = actor.Rotation + new TV_3DVECTOR(x, y, z);
        acinfo.InitialSpeed = actor.MoveData.Speed;
        //acinfo.InitialState = ActorState.DYING;
        ActorInfo a = actor.ActorFactory.Create( acinfo);
      }
    }
  }
}
