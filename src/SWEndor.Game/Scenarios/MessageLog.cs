namespace SWEndor.Game.Scenarios
{
  public struct MessageLog
  {
    public float Time;
    public string Text;
    public COLOR Color;

    public MessageLog(float time, string text, COLOR color)
    {
      Time = time;
      Text = text;
      Color = color;
    }
  }
}
