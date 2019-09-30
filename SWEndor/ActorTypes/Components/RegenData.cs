using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct RegenData
  {
    public readonly bool NoRegen;
    public readonly float SelfRegenRate;
    public readonly float ParentRegenRate;
    public readonly float ChildRegenRate;
    public readonly float SiblingRegenRate;

    public RegenData(bool noRegen, float self, float parent, float child, float sibling)
    {
      NoRegen = noRegen;
      SelfRegenRate = self;
      ParentRegenRate = parent;
      ChildRegenRate = child;
      SiblingRegenRate = sibling;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      bool noRegen = f.GetBoolValue(sectionname, "NoRegen", false);
      float self = f.GetFloatValue(sectionname, "SelfRegenRate", 0);
      float parent = f.GetFloatValue(sectionname, "ParentRegenRate", 0);
      float child = f.GetFloatValue(sectionname, "ChildRegenRate", 0);
      float sibling = f.GetFloatValue(sectionname, "SiblingRegenRate", 0);
      this = new RegenData(noRegen, self, parent, child, sibling);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetBoolValue(sectionname, "NoRegen", NoRegen);
      f.SetFloatValue(sectionname, "SelfRegenRate", SelfRegenRate);
      f.SetFloatValue(sectionname, "ParentRegenRate", ParentRegenRate);
      f.SetFloatValue(sectionname, "ChildRegenRate", ChildRegenRate);
      f.SetFloatValue(sectionname, "SiblingRegenRate", SiblingRegenRate);
    }
  }
}
