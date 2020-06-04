using SWEndor.ActorTypes;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using SWEndor.ProjectileTypes;

namespace SWEndor.Weapons
{
  internal struct WeapProjInfo
  {
    [INIValue("Projectile")]
    private string sProj;

    [INIValue]
    public bool IsActor;

    [INIValue]
    public float HomingDelay;

    [INIValue]
    public float LifeTime;

    [INIValue]
    public WeaponType WeaponType;

    [INIValue]
    public string[] FireSound;

    internal ProjectileTypeInfo Projectile; // cache
    internal ActorTypeInfo ActorProj; // cache

    public static WeapProjInfo Default = new WeapProjInfo
    {
      sProj = null,
      Projectile = null,
      ActorProj = null,
      IsActor = false,
      HomingDelay = 0,
      LifeTime = -1,
      WeaponType = WeaponType.NONE,
      FireSound = null
    };

    public void Load(Engine e)
    {
      if (sProj != null)
      {
        if (IsActor)
          ActorProj = e.ActorTypeFactory.Get(sProj);
        else
          Projectile = e.ProjectileTypeFactory.Get(sProj);
      }
    }
  }
}
