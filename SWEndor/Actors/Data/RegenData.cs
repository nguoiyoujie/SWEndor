namespace SWEndor.Actors.Data
{
  public struct RegenData
  {
    public bool NoRegen;
    public float SelfRegenRate;
    public float ParentRegenRate;
    public float ChildRegenRate;
    public float SiblingRegenRate;

    public void Reset()
    {
      NoRegen = false;
      SelfRegenRate = 0;
      ParentRegenRate = 0;
      ChildRegenRate = 0;
      SiblingRegenRate = 0;
    }

    public void CopyFrom(RegenData src)
    {
      NoRegen = src.NoRegen;
      SelfRegenRate = src.SelfRegenRate;
      ParentRegenRate = src.ParentRegenRate;
      ChildRegenRate = src.ChildRegenRate;
      SiblingRegenRate = src.SiblingRegenRate;
    }
  }
}
