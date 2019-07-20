namespace SWEndor.Actors.Data
{
  public class MaskDataSet
  {
    private ComponentMask[] list = new ComponentMask[Globals.ActorLimit];
    public ComponentMask this[int id] { get { return list[id % Globals.ActorLimit]; } set { list[id % Globals.ActorLimit] = value; } }

    public bool Contains(int id, ComponentMask match) { return (this[id] & match) == match; }
  }
}
