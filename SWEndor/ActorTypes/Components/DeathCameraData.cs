namespace SWEndor.ActorTypes.Components
{
  public struct DeathCameraData
  {
    public float Radius;
    public float Height;
    public float Period;

    public DeathCameraData(float radius, float height, float period)
    {
      Radius = radius;
      Height = height;
      Period = period;
    }
  }
}
