using MTV3D65;

namespace SWEndor.UI
{
  public class Widget
  {
    public Widget(Screen2D owner, string name)
    {
      Owner = owner;
      Name = name;
    }

    public readonly Screen2D Owner;
    public virtual bool Visible { get; }
    public virtual string Name { get; }
    public virtual void Draw() { }

    public TVScreen2DImmediate TVScreen2DImmediate { get { return this.GetEngine().TrueVision.TVScreen2DImmediate; } }
    public TVScreen2DText TVScreen2DText { get { return this.GetEngine().TrueVision.TVScreen2DText; } }
  }
}
