using MTV3D65;

namespace SWEndor.Actors
{
  public class DebrisSpawnerInfo
  {
    public ActorTypeInfo SpawnType;
    public TV_3DVECTOR SpawnPosition;
    public int RotationXMax;
    public int RotationXMin;
    public int RotationYMax;
    public int RotationYMin;
    public int RotationZMax;
    public int RotationZMin;
    public float Chance = 1;

    public DebrisSpawnerInfo(ActorTypeInfo type)
    {
      SpawnType = type;
    }

    public DebrisSpawnerInfo(ActorTypeInfo type, TV_3DVECTOR position, int xMin, int xMax, int yMin, int yMax, int zMin, int zMax, float chance)
    {
      SpawnType = type;
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
      double d = Engine.Instance().Random.NextDouble();
      if (d < Chance)
      {
        float x = Engine.Instance().Random.Next(RotationXMin, RotationXMax) / 100f;
        float y = Engine.Instance().Random.Next(RotationYMin, RotationYMax) / 100f;
        float z = Engine.Instance().Random.Next(RotationZMin, RotationZMax) / 100f;

        ActorCreationInfo acinfo = new ActorCreationInfo(SpawnType);
        acinfo.Position = actor.GetPosition() + SpawnPosition;
        acinfo.Rotation = new TV_3DVECTOR(actor.Rotation.x + x, actor.Rotation.y + y, actor.Rotation.z + z);
        acinfo.InitialSpeed = actor.MovementInfo.Speed;
        acinfo.InitialState = ActorState.DYING;
        ActorInfo a = ActorInfo.Create(acinfo);
      }
    }
  }
}
