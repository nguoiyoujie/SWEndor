namespace SWEndor.UI
{
  public class Widget
  {
    public Widget(string name)
    {
      Name = name;
    }

    public virtual bool Visible { get; }
    public virtual string Name { get; }
    public virtual void Draw() { }
  }
}
