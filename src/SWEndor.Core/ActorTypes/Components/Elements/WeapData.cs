using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct WeapData
  {
    private const string sNone = "";

    [INIValue(sNone, "Name")]
    public string Name;

    [INIValue(sNone, "Load")]
    public string Load;

    [INIValue(sNone, "Aim")]
    public string Aim;

    [INIValue(sNone, "Ammo")]
    public string Ammo;

    [INIValue(sNone, "Port")]
    public string Port;

    [INIValue(sNone, "Proj")]
    public string Proj;

    [INIValue(sNone, "Tgt")]
    public string Tgt;

    public WeapData(string name, string load, string aim, string ammo, string port, string proj, string tgt)
    {
      Name = name;
      Load = load;
      Aim = aim;
      Ammo = ammo;
      Port = port;
      Proj = proj;
      Tgt = tgt;
    }
  }
}
