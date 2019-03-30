namespace SWEndor.UI
{
  public class UIWidget
  {
    public UIWidget(string name)
    {
      Name = name;
    }

    public virtual bool Visible { get; }
    public virtual string Name { get; }
    public virtual void Draw() { }
  }
}
