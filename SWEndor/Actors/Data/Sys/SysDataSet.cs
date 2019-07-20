using MTV3D65;
using SWEndor.Primitives;

namespace SWEndor.Actors.Data
{
  public class SysDataSet : DataCache<SysData>
  {
    public SysDataSet(Engine engine)
    {
    }

    // Strength { get; set; }
    public void SetStrength(int id, float value) { setFunc_Strength(ref list[id % Globals.ActorLimit], value); }
    public float GetStrength(int id) { return getFunc_Strength(ref list[id % Globals.ActorLimit]); }

    // MaxStrength { get; set; }
    public void SetMaxStrength(int id, float value) { setFunc_MaxStrength(ref list[id % Globals.ActorLimit], value); }
    public float GetMaxStrength(int id) { return getFunc_MaxStrength(ref list[id % Globals.ActorLimit]); }

    // StrengthFrac { get; }
    public float GetStrengthFrac(int id) { return getFunc_StrengthFrac(ref list[id % Globals.ActorLimit]); }

    // StrengthColor { get; }
    public TV_COLOR GetStrengthColor(int id) { return getFunc_StrengthColor(ref list[id % Globals.ActorLimit]); }


    private SetFunc<SysData, float> setFunc_Strength = (ref SysData d, float v) => { d.Strength = v; };
    private GetFunc<SysData, float> getFunc_Strength = (ref SysData d) => { return d.Strength; };
    private SetFunc<SysData, float> setFunc_MaxStrength = (ref SysData d, float v) => { d.MaxStrength = v; };
    private GetFunc<SysData, float> getFunc_MaxStrength = (ref SysData d) => { return d.MaxStrength; };
    private GetFunc<SysData, float> getFunc_StrengthFrac = (ref SysData d) => { return d.StrengthFrac; };
    private GetFunc<SysData, TV_COLOR> getFunc_StrengthColor = (ref SysData d) => { return d.StrengthColor; };
  }
}
