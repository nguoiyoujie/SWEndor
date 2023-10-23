using SWEndor.Game.ActorTypes;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using SWEndor.Game.ProjectileTypes;
using SWEndor.Game.Actors;
using MTV3D65;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.AI;
using SWEndor.Game.Models;
using SWEndor.Game.Primitives.Extensions;
using SWEndor.Game.Projectiles;

namespace SWEndor.Game.Weapons
{
  internal struct WeapProjInfo
  {
    [INIValue("Projectile")]
    private string sProj;

    [INIValue]
    private bool IsActor;

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
      FireSound = new string[0]
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

    public float ProjSpeed
    {
      get
      {
        if (IsActor)
          return ActorProj?.MoveLimitData.MaxSpeed ?? 0;
        else
          return Projectile?.MoveLimitData.MaxSpeed ?? 0;
      }
    }

    public bool IsHoming
    {
      get
      {
        if (IsActor)
          return (ActorProj?.MoveLimitData.MaxTurnRate ?? 0) > 0;
        else
          return (Projectile?.MoveLimitData.MaxTurnRate ?? 0) > 0;
      }
    }

    public bool Create(Engine engine, ActorInfo owner, ActorInfo target, bool isPlayer, TV_3DVECTOR firePos, ref WeapAimInfo aim)
    {
      if (IsActor)
        return CreateActor(engine, owner, target, isPlayer, firePos, ref aim);
      else
        return CreateProjectile(engine, owner, target, isPlayer, firePos, ref aim);
    }

    private TV_3DVECTOR GetAimRotation(Engine engine, ActorInfo owner, ActorInfo target, float speed, ref WeapAimInfo aim)
    {
      float dist = DistanceModel.GetDistance(engine, owner, target);
      float d = aim.ApplyDeviation(engine, speed, dist);

      ActorInfo a2 = target.ParentForCoords;
      TV_3DVECTOR dir = (a2 == null)
                      ? target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d) - owner.GetGlobalPosition()
                      : target.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition() - owner.GetGlobalPosition();

      return dir.ConvertDirToRot(engine.TrueVision.TVMathLibrary);
    }

    private bool CreateProjectile(Engine engine, ActorInfo owner, ActorInfo target, bool isPlayer, TV_3DVECTOR firePos, ref WeapAimInfo aim)
    {
      if (Projectile == null)
        return true;

      if (!isPlayer
        && target != null
        && (owner.IsAggregateMode && target.IsAggregateMode))
      {
        owner.TypeInfo.FireAggregation(owner, target, Projectile);
        return true;
      }

      TV_3DVECTOR targetloc = firePos;

      ProjectileCreationInfo acinfo = new ProjectileCreationInfo(Projectile);
      acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

      if (owner.MoveData.Speed > Projectile.MoveLimitData.MinSpeed)
        acinfo.InitialSpeed = owner.MoveData.Speed;

      if (aim.IsAutoAim(isPlayer) && target != null)
      {
        acinfo.Rotation = GetAimRotation(engine, owner, target, Projectile.MoveLimitData.MaxSpeed, ref aim);
      }
      else
      {
        acinfo.Rotation = owner.GetGlobalRotation();
      }

      acinfo.OwnerID = owner.ID;
      acinfo.TargetID = target?.ID ?? -1;
      acinfo.LifeTime = LifeTime;
      ProjectileInfo a = engine.ProjectileFactory.Create(acinfo);
      return true;
    }

    private bool CreateActor(Engine engine, ActorInfo owner, ActorInfo target, bool isPlayer, TV_3DVECTOR firePos, ref WeapAimInfo aim)
    {
      if (ActorProj == null)
        return true;

      TV_3DVECTOR targetloc = firePos;
      
      ActorCreationInfo acinfo = new ActorCreationInfo(ActorProj);
      acinfo.Position = owner.GetRelativePositionXYZ(targetloc.x, targetloc.y, targetloc.z);

      if (owner.MoveData.Speed > ActorProj.MoveLimitData.MinSpeed)
        acinfo.InitialSpeed = owner.MoveData.Speed;

      if (aim.IsAutoAim(isPlayer) && target != null)
      {
        acinfo.Rotation = GetAimRotation(engine, owner, target, ActorProj.MoveLimitData.MaxSpeed, ref aim);
      }
      else
      {
        acinfo.Rotation = owner.GetGlobalRotation();
      }

      ActorInfo a = engine.ActorFactory.Create(acinfo);
      a.Faction = owner.Faction;
      owner.AddChild(a);

      if (a.Mask.Has(ComponentMask.HAS_AI))
      {
        a.QueueLast(Wait.GetOrCreate(HomingDelay));
        a.QueueLast(ProjectileAttackActor.GetOrCreate(target));
        a.QueueLast(Lock.GetOrCreate());
      }
      else // define CurrentAction.target
      {
        a.QueueLast(ProjectileAttackActor.GetOrCreate(target));
      }

      if (LifeTime > 0)
        a.DyingTimerSet(LifeTime, true);
      return true;
    }
  }
}
