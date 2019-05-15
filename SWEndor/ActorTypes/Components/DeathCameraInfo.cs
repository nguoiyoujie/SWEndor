namespace SWEndor.ActorTypes.Components
{
  public struct DeathCameraInfo
  {
    public float Radius;
    public float Height;
    public float Period;

    public DeathCameraInfo(float radius, float height, float period)
    {
      Radius = radius;
      Height = height;
      Period = period;
    }
  }
}
