namespace SWEndor.Game.UI
{
  public struct TextInfo
  {
    public int Priority;
    public string Text;
    public float ExpireTime;
    public COLOR Color;

    public static TextInfo Default
    {
      get
      {
        return new TextInfo
        {
          Priority = 0,
          Text = "",
          ExpireTime = 0
        };
      }
    }
  }

  public struct TopTextInfo
  {
    public string Text;
    public COLOR Color;

    public static TopTextInfo Default
    {
      get
      {
        return new TopTextInfo
        {
          Text = "",
          Color = ColorLocalization.Get(ColorLocalKeys.WHITE)
        };
      }
    }
  }
}
