using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct RegenData
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
      bool noRegen = f.GetBool(sectionname, "NoRegen", false);
      float self = f.GetFloat(sectionname, "SelfRegenRate", 0);
      float parent = f.GetFloat(sectionname, "ParentRegenRate", 0);
      float child = f.GetFloat(sectionname, "ChildRegenRate", 0);
      float sibling = f.GetFloat(sectionname, "SiblingRegenRate", 0);
      this = new RegenData(noRegen, self, parent, child, sibling);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetBool(sectionname, "NoRegen", NoRegen);
      f.SetFloat(sectionname, "SelfRegenRate", SelfRegenRate);
      f.SetFloat(sectionname, "ParentRegenRate", ParentRegenRate);
      f.SetFloat(sectionname, "ChildRegenRate", ChildRegenRate);
      f.SetFloat(sectionname, "SiblingRegenRate", SiblingRegenRate);
    }
  }
}
