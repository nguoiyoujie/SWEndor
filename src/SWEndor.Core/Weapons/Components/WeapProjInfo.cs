using SWEndor.ActorTypes;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using SWEndor.ProjectileTypes;

namespace SWEndor.Weapons
{
  internal struct WeapProjInfo
  {
    internal ProjectileTypeInfo Projectile; // cache
    internal ActorTypeInfo ActorProj; // cache
    public bool IsActor;
    public float HomingDelay;
    public float LifeTime;
    public WeaponType Type;
    public string[] FireSound;

    public static WeapProjInfo Default = new WeapProjInfo
    {
      Projectile = null,
      ActorProj = null,
      IsActor = false,
      HomingDelay = 0,
      LifeTime = -1,
      Type = WeaponType.NONE,
      FireSound = null
    };

    public void LoadFromINI(Engine e, INIFile f, string sectionname)
    {
      this = Default;
      IsActor = f.GetBool(sectionname, "IsActor", IsActor);
      HomingDelay = f.GetFloat(sectionname, "HomingDelay", HomingDelay);
      LifeTime = f.GetFloat(sectionname, "LifeTime", LifeTime);
      Type = f.GetEnum(sectionname, "WeaponType", Type);
      FireSound = f.GetStringArray(sectionname, "FireSound", FireSound);

      string proj = f.GetString(sectionname, "Projectile", null);
      if (proj != null)
      {
        if (IsActor)
          ActorProj = e.ActorTypeFactory.Get(proj);
        else
          Projectile = e.ProjectileTypeFactory.Get(proj);
      }
    }
  }
}
