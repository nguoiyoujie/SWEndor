using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct DeathCameraData
  {
    public readonly float Radius;
    public readonly float Height;
    public readonly float Period;

    public DeathCameraData(float radius, float height, float period)
    {
      Radius = radius;
      Height = height;
      Period = period;
    }

    public DeathCameraData(float3 data)
    {
      Radius = data.x;
      Height = data.y;
      Period = data.z;
    }
  }
}
