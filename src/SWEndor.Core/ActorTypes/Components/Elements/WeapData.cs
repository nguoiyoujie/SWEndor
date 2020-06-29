using Primrose.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct WeapData
  {
    [INIValue]
    public string Name;

    [INIValue]
    public string Load;

    [INIValue]
    public string Aim;

    [INIValue]
    public string Ammo;

    [INIValue]
    public string Port;

    [INIValue]
    public string Proj;

    [INIValue]
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
