using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes.Components
{
  public struct DebrisSpawnerInfo
  {
    public readonly string Type;
    private ActorTypeInfo _cache;
    public readonly TV_3DVECTOR SpawnPosition;
    public readonly int RotationXMax;
    public readonly int RotationXMin;
    public readonly int RotationYMax;
    public readonly int RotationYMin;
    public readonly int RotationZMax;
    public readonly int RotationZMin;
    public readonly float Chance;

    public DebrisSpawnerInfo(string type, TV_3DVECTOR position, int xMin, int xMax, int yMin, int yMax, int zMin, int zMax, float chance)
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
        float x = actor.Engine.Random.Next(RotationXMin, RotationXMax) / 100f;
        float y = actor.Engine.Random.Next(RotationYMin, RotationYMax) / 100f;
        float z = actor.Engine.Random.Next(RotationZMin, RotationZMax) / 100f;

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
