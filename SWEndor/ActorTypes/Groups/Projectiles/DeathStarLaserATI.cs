﻿using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStarLaserATI : Group.Projectile
  {
    private static DeathStarLaserATI _instance;
    public static DeathStarLaserATI Instance()
    {
      if (_instance == null) { _instance = new DeathStarLaserATI(); }
      return _instance;
    }

    private DeathStarLaserATI() : base("Death Star Laser")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 10;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = true;
      ImpactDamage = 99999;
      MaxSpeed = Globals.LaserSpeed * 8.5f;
      MinSpeed = Globals.LaserSpeed * 8.5f;

      NoAI = true;
      EnableDistanceCull = false;

      SourceMesh = Globals.Engine.TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Globals.Engine.TVScene.CreateMeshBuilder(Key);

        SourceMesh.CreateBox(40, 40, 1000);
        SourceMesh.SetMeshCenter(0, 0, 2200);
        SourceMesh.SetColor(new TV_COLOR(0, 1, 0, 1).GetIntColor());

        SourceMesh.Enable(false);

        SourceMesh.SetCollisionEnable(false);

        ImpactCloseEnoughDistance = 200;
      }
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      /*
      if (Armour == "green")
      {
        Armour = "yellow";
        ainfo.Mesh.SetColor(new TV_COLOR(0, 1, 0, 1).GetIntColor());
      }
      else
      {
        Armour = "green";
        ainfo.Mesh.SetColor(new TV_COLOR(1, 1, 0, 1).GetIntColor());
      }
      */

      /*
      if (ainfo.State == ActorState.DYING)
      {
        if (ainfo.TimedLife > 0)
        {
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeInfo.Factory.Get("Explosion"));
          acinfo.Position = ainfo.Position;
          ActorInfo.Create(acinfo);
        }
        //ainfo.State = ActorState.DEAD;

      }
      */
    }

    public override void ProcessState(ActorInfo ainfo)
    {
    }

    public override void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      base.ProcessHit(ownerActorID, hitbyActorID, impact, normal);
      ActorInfo owner = ActorInfo.Factory.Get(ownerActorID);
      ActorInfo hitby = ActorInfo.Factory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      if (hitby.CombatInfo.TimedLife > 0.5f)
        hitby.CombatInfo.TimedLife = 0.5f;

      if (owner.CombatInfo.TimedLife > 0)
        owner.CombatInfo.TimedLife = 0;
    }
  }
}

