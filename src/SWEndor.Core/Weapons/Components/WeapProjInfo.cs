using SWEndor.ActorTypes;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using SWEndor.ProjectileTypes;

namespace SWEndor.Weapons
{
  internal struct WeapProjInfo
  {
    private const string sNone = "";

    [INIValue(sNone, "Projectile")]
    private string sProj;

    [INIValue(sNone, "IsActor")]
    public bool IsActor;

    [INIValue(sNone, "HomingDelay")]
    public float HomingDelay;

    [INIValue(sNone, "LifeTime")]
    public float LifeTime;

    [INIValue(sNone, "WeaponType")]
    public WeaponType Type;

    [INIValue(sNone, "FireSound")]
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
      Type = WeaponType.NONE,
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
