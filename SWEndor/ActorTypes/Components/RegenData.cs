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

    public RegenData(INIFile f, string sectionname)
    {
      NoRegen = f.GetBoolValue(sectionname, "NoRegen", false);
      SelfRegenRate = f.GetFloatValue(sectionname, "SelfRegenRate", 0);
      ParentRegenRate = f.GetFloatValue(sectionname, "ParentRegenRate", 0);
      ChildRegenRate = f.GetFloatValue(sectionname, "ChildRegenRate", 0);
      SiblingRegenRate = f.GetFloatValue(sectionname, "SiblingRegenRate", 0);
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
