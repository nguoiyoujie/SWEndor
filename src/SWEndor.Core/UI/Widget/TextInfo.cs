namespace SWEndor.UI
{
  public class TextInfo
  {
    public int Priority = 0;
    public string Text = "";
    public float ExpireTime = 0;
    public COLOR Color;
  }

  public class TopTextInfo
  {
    public string Text = "";
    public COLOR Color = ColorLocalization.Get(ColorLocalKeys.WHITE);
  }
}
