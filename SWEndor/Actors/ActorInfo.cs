﻿using MTV3D65;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public ActorTypeInfo TypeInfo { get; private set; }
    public SpawnerInfo SpawnerInfo { get; set; }

    // Identifiers
    private string _name = "New Actor";
    private string sidebar_name = "";

    public string Name { get { return _name; } }
    public string SideBarName { get { return (sidebar_name.Length == 0) ? _name : sidebar_name; } set { sidebar_name = value; } }
    public int ID { get; private set; }
    public string Key { get { return _name + " " + ID; } }

    public override string ToString()
    {
      return string.Format("ACTOR:{0},{1}", ID, _name);
    }

    // Creation and Disposal
    public float CreationTime = 0;
    public CreationState CreationState = CreationState.PLANNED;

    // Faction
    private FactionInfo _faction;
    public FactionInfo Faction
    {
      get
      {
        if (_faction == null)
        {
          _faction = FactionInfo.Neutral;
          _faction.RegisterActor(this);
        }
        return _faction;
      }
      set
      {
        if (_faction != null)
          _faction.UnregisterActor(this);
        _faction = value;
        if (_faction == null)
          _faction = FactionInfo.Neutral;
        _faction.RegisterActor(this);
      }
    }

    // Components
    public ScoreInfo Score;
    public CombatInfo CombatInfo;
    public MovementInfo MovementInfo;
    public CycleInfo CycleInfo;
    public WeaponSystemInfo WeaponSystemInfo;
    public RegenerationInfo RegenerationInfo;
    public ExplosionInfo ExplosionInfo;
    public CollisionInfo CollisionInfo;
    public CameraSystemInfo CameraSystemInfo;

    // Checks
    public bool EnteredCombatZone = false;
    public float LastProcessStateUpdateTime = 0;

    // Camera System


    // Delegate Events
    // plan to make them true delegates instead of string
    public GameEvent TickEvents;
    public GameEvent CreatedEvents;
    public GameEvent DestroyedEvents;
    public GameEvent ActorStateChangeEvents;
    public GameEvent HitEvents;

    // AI
    public ActionInfo CurrentAction = null;
    public bool CanEvade = true;
    public bool CanRetaliate = true;
    public int HuntWeight = 1;

    // Utility
    public ActorState ActorState { get; set; }
    public ActorState prevActorState { get; private set; }

    public TV_3DVECTOR Scale { get; set; }
    public TV_3DVECTOR prevScale { get; private set; }

    public TV_3DVECTOR Position { get; set; }
    public Cached3DVector WorldPosition;
    public TV_3DVECTOR PrevPosition { get; private set; }

    public TV_3DVECTOR Rotation { get; private set; }
    public Cached3DVector WorldRotation;

    // Render
    public TVMesh Mesh { get; private set; }
    public TVMesh FarMesh { get; private set; }
    public bool AttachToParent = false;

    // Particle system
    public int ParticleEmitterID = -1;
    public TVParticleSystem ParticleSystem = null;

    // Ownership
    public int ParentID { get; private set; } = -1;
    public readonly Factory Owner;

    #region Creation Methods

    private ActorInfo(Factory owner, int id, ActorCreationInfo acinfo)
    {
      Owner = owner;
      ID = id;

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      // Components
      Score = new ScoreInfo(this);
      CombatInfo = new CombatInfo(this);
      MovementInfo = new MovementInfo(this);
      CycleInfo = new CycleInfo(this, null);
      WeaponSystemInfo = new WeaponSystemInfo(this);
      RegenerationInfo = new RegenerationInfo(this);
      ExplosionInfo = new ExplosionInfo(this);
      CollisionInfo = new CollisionInfo(this);
      CameraSystemInfo = new CameraSystemInfo(this);

      // Creation
      CreationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime;

      // Combat
      CombatInfo.IsCombatObject = TypeInfo.IsCombatObject;
      CombatInfo.OnTimedLife = TypeInfo.OnTimedLife;
      CombatInfo.TimedLife = TypeInfo.TimedLife;
      CombatInfo.Strength = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : TypeInfo.MaxStrength;

      Faction = acinfo.Faction;
      ActorState = acinfo.InitialState;
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      Scale = acinfo.InitialScale;

      MovementInfo.Speed = (acinfo.InitialSpeed > 0) ? acinfo.InitialSpeed : TypeInfo.MaxSpeed;
      prevScale = acinfo.InitialScale;
      PrevPosition = Position;

      HuntWeight = TypeInfo.HuntWeight;

      TypeInfo.Initialize(this);
    }

    private void Rebuild(ActorCreationInfo acinfo)
    {
      // Clear past resources
      Destroy();

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      // Creation
      CreationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime;

      // Combat
      CombatInfo.IsCombatObject = TypeInfo.IsCombatObject;
      CombatInfo.OnTimedLife = TypeInfo.OnTimedLife;
      CombatInfo.TimedLife = TypeInfo.TimedLife;
      CombatInfo.Strength = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : TypeInfo.MaxStrength;

      Faction = acinfo.Faction;
      ActorState = acinfo.InitialState;
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      Scale = acinfo.InitialScale;

      MovementInfo.Speed = (acinfo.InitialSpeed > 0) ? acinfo.InitialSpeed : TypeInfo.MaxSpeed;
      prevScale = acinfo.InitialScale;
      PrevPosition = Position;

      HuntWeight = TypeInfo.HuntWeight;
      TypeInfo.Initialize(this);
    }

    public void Generate()
    {
      Mesh = TypeInfo.GenerateMesh();
      FarMesh = TypeInfo.GenerateFarMesh();

      SetLocalPosition(Position.x, Position.y, Position.z);
      SetLocalRotation(Rotation.x, Rotation.y, Rotation.z);

      Mesh.SetScale(Scale.x, Scale.y, Scale.z);
      Mesh.SetMeshName(ID.ToString());
      Mesh.SetTag(ID.ToString());
      Mesh.ComputeBoundings();
      //Mesh.ShowBoundingBox(true);

      FarMesh.SetScale(Scale.x, Scale.y, Scale.z);
      FarMesh.SetMeshName(ID.ToString());
      FarMesh.SetTag(ID.ToString());
      FarMesh.ComputeBoundings();

      Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      Mesh.Enable(true);
      FarMesh.Enable(true);

      CreationState = CreationState.GENERATED;
      Update();
      OnCreatedEvent(new object[] { this });

      TypeInfo.GenerateAddOns(this);
      //SpawnerInfo = new SpawnerInfo(this);
    }

    public static ActorInfo Create(Factory factory, ActorCreationInfo acinfo)
    {
      return factory.Register(acinfo);
    }
    #endregion

    public void Process()
    {
      using (new PerfElement("actor_process"))
      {
        if (TypeInfo.NoProcess)
          return;

        if (ActorState != ActorState.DEAD)
          Update();

        CycleInfo.Process();

        if (Mesh != null)
        {
          CheckState();
          if (ActorState != ActorState.DEAD)
          {
            if (TypeInfo.CollisionEnabled || TypeInfo is ActorTypes.Group.Projectile)
              CheckCollision();
            MovementInfo.Move();
          }
        }
      }
      OnTickEvent(new object[] { this, ActorState });
    }

    public void ProcessAI()
    {
      if (TypeInfo.NoProcess
        || TypeInfo.NoAI
        || CreationState != CreationState.ACTIVE
        || ActorState == ActorState.DYING
        || ActorState == ActorState.DEAD
        || (IsPlayer() && !Globals.Engine.PlayerInfo.PlayerAIEnabled)
        )
        return;

      Owner.Engine.ActionManager.Run(ID, CurrentAction);
    }

    public void ProcessCollision()
    {
      TestCollision();
    }

    private void CheckState()
    {
      CombatInfo.Process();

      if (prevActorState == ActorState)
      {
        TypeInfo.ProcessState(this);
      }
      else
      {
        TypeInfo.ProcessNewState(this);
        OnStateChangeEvent(new object[] { this, prevActorState });
        prevActorState = ActorState;
      }

      if (ActorState == ActorState.DEAD)
        Kill();
    }

    private void Update()
    {
      PrevPosition = Position;

      if (Mesh == null)
        return;

      TV_3DVECTOR pos = GetPosition();
      TV_3DVECTOR rot = GetRotation();

      Mesh.SetPosition(pos.x, pos.y, pos.z);
      Mesh.SetRotation(rot.x, rot.y, rot.z);
      FarMesh.SetPosition(pos.x, pos.y, pos.z);
      FarMesh.SetRotation(rot.x, rot.y, rot.z);

      Mesh.SetCollisionEnable(!IsAggregateMode() && TypeInfo.CollisionEnabled);
      FarMesh.SetCollisionEnable(false);

      if (Scale.x != prevScale.x || Scale.y != prevScale.y || Scale.z != prevScale.z)
      {
        prevScale = new TV_3DVECTOR(Scale.x, Scale.y, Scale.z);
        Mesh.SetScale(Scale.x, Scale.y, Scale.z);
        FarMesh.SetScale(Scale.x, Scale.y, Scale.z);
      }

      if (CreationState == CreationState.GENERATED)
        CreationState = CreationState.ACTIVE;

      if (ParticleSystem != null && !IsFarMode())
        ParticleSystem.Update();
    }

    public void Render()
    {
      if (TypeInfo.NoRender || CreationState != CreationState.ACTIVE)
      {
        return;
      }

      if (!IsAggregateMode())
      {
        if (!IsPlayer() || PlayerCameraInfo.Instance().CameraMode != CameraMode.FREEROTATION)
        {
          if (!IsFarMode())
          {
            if (Mesh != null && Mesh.IsVisible())
              using (new PerfElement("mesh_render"))
                Mesh.Render();
          }
          else
          {
            if (FarMesh != null && FarMesh.IsVisible())
              using (new PerfElement("mesh_render_far"))
                FarMesh.Render();
          }

          if (ParticleSystem != null && !IsFarMode())
            using (new PerfElement("particlesys_render"))
              ParticleSystem.Render();
        }
      }
    }

    private void CheckCollision()
    {
      CollisionInfo.IsTestingCollision = false;
      if (!PrevPosition.Equals(new TV_3DVECTOR()) && !GetPosition().Equals(new TV_3DVECTOR()))
      {
        // only check player and projectiles
        if (IsPlayer() 
          || TypeInfo is ActorTypes.Group.Projectile
          || (ActorState == ActorState.DYING && TypeInfo.TargetType.HasFlag(TargetType.FIGHTER)))
        {
          if (CollisionInfo.IsInCollision)
          {
            ActorInfo a = Owner.Engine.ActorFactory.Get(CollisionInfo.CollisionActorID);
            if (IsPlayer() && Globals.Engine.PlayerInfo.PlayerAIEnabled)
            {
              Globals.Engine.Screen2D.MessageSecondaryText(string.Format("DEV WARNING: PLAYER AI COLLIDED: {0}", a.ToString()), 1.5f, new TV_COLOR(1, 0.2f, 0.2f, 1), 99999);
              CollisionInfo.IsInCollision = false;
              CollisionInfo.IsTestingCollision = true;
              return;
            }
            if (a != null)
            {
              a.TypeInfo.ProcessHit(CollisionInfo.CollisionActorID, ID, CollisionInfo.CollisionImpact, CollisionInfo.CollisionNormal);
              TypeInfo.ProcessHit(ID, CollisionInfo.CollisionActorID, CollisionInfo.CollisionImpact, -1 * CollisionInfo.CollisionNormal);
            }
            CollisionInfo.IsInCollision = false;
          }
          CollisionInfo.IsTestingCollision = true;
        }
      }
    }

    private void TestLandscapeCollision()
    {
      TV_3DVECTOR vmin = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z, false);
      TV_3DVECTOR vmax = GetRelativePositionXYZ(0, 0, TypeInfo.min_dimensions.z, false) + PrevPosition - Position;

      // check landscape only
      if (TypeInfo is ActorTypes.Group.Projectile || TypeInfo.TargetType != TargetType.NULL)
        if (Globals.Engine.LandInfo.Land.AdvancedCollide(vmin, vmax).IsCollision())
        {
          ActorState = ActorState.DEAD;
        }
    }

    private void TestCollision()
    {
      if (CollisionInfo.IsTestingCollision)
      {
        TV_3DVECTOR vmin = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z, false);
        TV_3DVECTOR vmax = GetRelativePositionXYZ(0, 0, TypeInfo.min_dimensions.z, false) + PrevPosition - Position;

        CollisionInfo.IsInCollision = TestCollision(vmin, vmax, true, out CollisionInfo.CollisionImpact, out CollisionInfo.CollisionNormal, out CollisionInfo.CollisionActorID);
        CollisionInfo.IsTestingCollision = false;
      }
      if (CollisionInfo.IsTestingProspectiveCollision)
      {
        int dummy;

        TV_3DVECTOR prostart = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z + 10);
        TV_3DVECTOR proend0 = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z + 10 + CollisionInfo.ProspectiveCollisionScanDistance);

        TV_3DVECTOR proImpact = new TV_3DVECTOR();
        TV_3DVECTOR proNormal = new TV_3DVECTOR();

        TV_3DVECTOR _Impact = new TV_3DVECTOR();
        TV_3DVECTOR _Normal = new TV_3DVECTOR();
        int count = 0;

        CollisionInfo.IsInProspectiveCollision = false;

        if (TestCollision(prostart, proend0, false, out _Impact, out _Normal, out CollisionInfo.ProspectiveCollisionActorID))
        {
          proImpact += _Impact;
          proNormal += _Normal;
          count++;
          CollisionInfo.IsInProspectiveCollision = true;
          CollisionInfo.ProspectiveCollisionSafe = _Impact + _Normal * 10000;
        }

        if (CollisionInfo.IsInProspectiveCollision || CollisionInfo.IsAvoidingCollision)
          CollisionInfo.ProspectiveCollisionLevel = 0;
        bool nextlevel = true;
        float dist = 0;
        while (nextlevel && CollisionInfo.ProspectiveCollisionLevel < 5)
        {
          CollisionInfo.ProspectiveCollisionLevel++;
          nextlevel = true;
          for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
              if (i == 0 && j == 0)
                continue;

              proend0 = GetRelativePositionXYZ(i * CollisionInfo.ProspectiveCollisionScanDistance * 0.1f * CollisionInfo.ProspectiveCollisionLevel
                                             , j * CollisionInfo.ProspectiveCollisionScanDistance * 0.1f * CollisionInfo.ProspectiveCollisionLevel
                                             , TypeInfo.max_dimensions.z + 10 + CollisionInfo.ProspectiveCollisionScanDistance);
              if (TestCollision(prostart, proend0, false, out _Impact, out _Normal, out dummy))
              {
                proImpact += _Impact;
                proNormal += _Normal;
                count++;
                float newdist = Globals.Engine.TrueVision.TVMathLibrary.GetDistanceVec3D(Position, _Impact);
                if (dist < newdist)
                {
                  dist = newdist;
                  CollisionInfo.ProspectiveCollisionSafe = Position + (proend0 - Position) * 1000;
                  if (CollisionInfo.IsAvoidingCollision)
                    nextlevel = false;
                }
              }
              else
              {
                dist = float.MaxValue;
                if (!CollisionInfo.IsAvoidingCollision)
                  nextlevel = false;
                CollisionInfo.ProspectiveCollisionSafe = Position + (proend0 - Position) * 1000;
              }
            }
        }

        if (count > 0)
        {
          CollisionInfo.ProspectiveCollisionImpact = proImpact / count;
          CollisionInfo.ProspectiveCollisionNormal = proNormal / count;
        }
        CollisionInfo.IsTestingProspectiveCollision = false;
      }
    }

    private bool TestCollision(TV_3DVECTOR start, TV_3DVECTOR end, bool getActorInfo, out TV_3DVECTOR vImpact, out TV_3DVECTOR vNormal, out int actorID)
    {
      using (new PerfElement("fn_testcollision"))
      {
        try
        {
          actorID = -1;
          vImpact = new TV_3DVECTOR();
          vNormal = new TV_3DVECTOR();

          TV_COLLISIONRESULT tvcres = new TV_COLLISIONRESULT();

          if (Globals.Engine.TrueVision.TVScene.AdvancedCollision(start, end, ref tvcres))
          {
            if (IsPlayer())
            {
              Globals.Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
              Globals.Engine.TrueVision.TVScreen2DImmediate.Draw_Line3D(start.x
                                                , start.y
                                                , start.z
                                                , end.x
                                                , end.y
                                                , end.z
                                                , new TV_COLOR(0.5f, 1, 0.2f, 1).GetIntColor()
                                                );
              Globals.Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
            }

            if (tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_MESH && tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              return false;

            vImpact = new TV_3DVECTOR(tvcres.vCollisionImpact.x, tvcres.vCollisionImpact.y, tvcres.vCollisionImpact.z);
            vNormal = new TV_3DVECTOR(tvcres.vCollisionNormal.x, tvcres.vCollisionNormal.y, tvcres.vCollisionNormal.z);

            if (getActorInfo)
            {
              if (tvcres.eCollidedObjectType == CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              {
                ActorState = ActorState.DEAD;
                return true;
              }

              TVMesh tvm = Globals.Engine.TrueVision.TVGlobals.GetMeshFromID(tvcres.iMeshID);
              if (tvm != null) // && tvm.IsVisible())
              {
                if (int.TryParse(tvm.GetTag(), out actorID))
                {
                  ActorInfo actor = Owner.Engine.ActorFactory.Get(actorID);
                  return (actor != null
                       && actorID != ID
                       && !HasRelative(actorID)
                       && actor.TypeInfo.CollisionEnabled
                       && !actor.IsAggregateMode()
                       && actor.CreationState == CreationState.ACTIVE);
                }
              }
              return false;
            }
            else
            {
              if (tvcres.eCollidedObjectType == CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              {
                return true;
              }

              TVMesh tvm = Globals.Engine.TrueVision.TVGlobals.GetMeshFromID(tvcres.iMeshID);
              if (tvm != null) // && tvm.IsVisible())
              {
                if (int.TryParse(tvm.GetTag(), out actorID))
                {
                  return true;
                }
              }
              return false;
            }
          }
          else
          {
            if (IsPlayer())
            {
              Globals.Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
              Globals.Engine.TrueVision.TVScreen2DImmediate.Draw_Line3D(start.x
                                                , start.y
                                                , start.z
                                                , end.x
                                                , end.y
                                                , end.z
                                                , new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor()
                                                );
              Globals.Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
            }
          }
          return false;
        }
        catch
        {
          actorID = -1;
          vImpact = new TV_3DVECTOR();
          vNormal = new TV_3DVECTOR();
          return false;
        }
      }
    }

    public void FireWeapon(int targetActorID, string weapon)
    {
      if (ActorState != ActorState.DYING && ActorState != ActorState.DEAD)
        TypeInfo.FireWeapon(ID, targetActorID, weapon);
    }

    #region Position / Rotation
    public TV_3DVECTOR GetPosition()
    {
      //if (WorldPosition.Time >= Globals.Engine.Game.GameTime)
      //  return WorldPosition.Value;

      TV_3DVECTOR ret = Position;
      ActorInfo a = AttachToParent ? Owner.Engine.ActorFactory.Get(ParentID) : null;
      if (a != null)
        ret = a.GetRelativePositionXYZ(ret.x, ret.y, ret.z);
      WorldPosition.Value = ret;
      WorldPosition.Time = Globals.Engine.Game.GameTime;
      return ret;
    }

    public TV_3DVECTOR GetLocalPosition()
    {
      return Position;
    }

    public void SetLocalPosition(float x, float y, float z)
    {
      Position = new TV_3DVECTOR(x, y, z);
    }

    public TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, bool uselocal = false)
    {
      TV_3DVECTOR pos = GetPosition();
      TV_3DVECTOR rot = GetRotation();
      if (uselocal)
      {
        pos = GetLocalPosition();
        rot = GetLocalRotation();
      }

      TV_3DVECTOR ret = new TV_3DVECTOR();

      Globals.Engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref ret, new TV_3DVECTOR(right, up, front), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool uselocal = false)
    {
      TV_3DVECTOR pos = GetPosition();
      TV_3DVECTOR rot = GetRotation();
      if (uselocal)
      {
        pos = GetLocalPosition();
        rot = GetLocalRotation();
      }

      TV_3DVECTOR ret = new TV_3DVECTOR();

      Globals.Engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref ret, new TV_3DVECTOR(x, y, z), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public TV_3DVECTOR GetRotation()
    {
      //if (WorldRotation.Time >= Globals.Engine.Game.GameTime)
      //  return WorldRotation.Value;

      ActorInfo a = AttachToParent ? Owner.Engine.ActorFactory.Get(ParentID) : null;
      if (a != null && a.Mesh != null)
      {
        TVMathLibrary mathl = Globals.Engine.TrueVision.TVMathLibrary;
        //TV_3DVECTOR aret = a.GetRotation();

        TV_3DMATRIX pmat = new TV_3DMATRIX();
        TV_3DMATRIX pymat = new TV_3DMATRIX();
        TV_3DMATRIX pyxmat = new TV_3DMATRIX();
        TV_3DMATRIX pyxzmat = new TV_3DMATRIX();
        mathl.TVMatrixIdentity(ref pmat);
        TV_3DMATRIX xmat = new TV_3DMATRIX();
        TV_3DMATRIX ymat = new TV_3DMATRIX();
        TV_3DMATRIX zmat = new TV_3DMATRIX();
        TV_3DVECTOR front = new TV_3DVECTOR();
        TV_3DVECTOR up = new TV_3DVECTOR();
        TV_3DVECTOR right = new TV_3DVECTOR();
        a.Mesh.GetBasisVectors(ref front, ref up, ref right);

        mathl.TVMatrixRotationAxis(ref ymat, up, Rotation.y);
        mathl.TVMatrixRotationAxis(ref xmat, right, Rotation.x);
        mathl.TVMatrixRotationAxis(ref zmat, front, Rotation.z);
        mathl.TVMatrixMultiply(ref pymat, pmat, ymat);
        mathl.TVMatrixMultiply(ref pyxmat, pymat, xmat);
        mathl.TVMatrixMultiply(ref pyxzmat, pyxmat, zmat);

        TV_3DVECTOR dir = Utilities.GetDirection(a.GetRotation());
        TV_3DVECTOR rdir = new TV_3DVECTOR();
        mathl.TVVec3TransformCoord(ref rdir, dir, pyxzmat);
        TV_3DVECTOR rot = Utilities.GetRotation(rdir);

        WorldRotation.Value = rot;
        WorldRotation.Time = Globals.Engine.Game.GameTime;
        return rot;
      }
      else
      {
        WorldRotation.Value = Rotation;
        WorldRotation.Time = Globals.Engine.Game.GameTime;
        return Rotation;
      }
    }

    public void SetRotation(float x, float y, float z)
    {
      ActorInfo a = AttachToParent ? Owner.Engine.ActorFactory.Get(ParentID) : null;
      if (a != null)
      {
        //TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
        //TV_3DVECTOR ret = new TV_3DVECTOR(x, y, z);
        /*
        TV_3DVECTOR aret = a.GetRotation();
        TV_3DQUATERNION pmat = new TV_3DQUATERNION();
        Globals.Engine.TrueVision.TVMathLibrary.TVQuaternionIdentity(ref pmat);
        Globals.Engine.TrueVision.TVMathLibrary.TVQuaternionRotationYawPitchRoll(ref pmat, aret.y, aret.x, aret.z);
        Globals.Engine.TrueVision.TVMathLibrary.TVQuaternionRotationYawPitchRoll(ref pmat, -Rotation.y, -Rotation.x, -Rotation.z);
        Globals.Engine.TrueVision.TVMathLibrary.TVQuaternionNormalize(ref pmat, pmat);
        Rotation = new TV_3DVECTOR(pmat.x, pmat.y, pmat.z);
        */
        /*
        TV_3DVECTOR aret = a.GetRotation();
        TV_3DMATRIX pmat = new TV_3DMATRIX();
        TV_3DMATRIX pinmat = new TV_3DMATRIX();
        float pindet = 0;
        TV_3DMATRIX mat = new TV_3DMATRIX();
        Globals.Engine.TrueVision.TVMathLibrary.TVMatrixRotationYawPitchRoll(ref pmat, aret.y, aret.x, aret.z);
        Globals.Engine.TrueVision.TVMathLibrary.TVMatrixInverse(ref pinmat, ref pindet, pmat);
        Globals.Engine.TrueVision.TVMathLibrary.TVMatrixRotationYawPitchRoll(ref mat, y, x, z);
        TV_3DMATRIX lmat = mat * pinmat;

        TV_3DQUATERNION quad = new TV_3DQUATERNION();
        Globals.Engine.TrueVision.TVMathLibrary.TVConvertMatrixToQuaternion(ref quad, lmat);
        //Globals.Engine.TrueVision.TVMathLibrary.TVEulerAnglesFromMatrix(ref aret, lmat);
        //Globals.Engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref aret, new TV_3DVECTOR(), aret.x, aret.y, aret.z);
        Globals.Engine.TrueVision.TVMathLibrary.TVQuaternionNormalize(ref quad, quad);
        Rotation = new TV_3DVECTOR(quad.x, quad.y, quad.z);
        */
        //Rotation = aret;
        //Rotation = Utilities.GetRotation(ret);
        Rotation = new TV_3DVECTOR(x, y, z);
      }
      else
        Rotation = new TV_3DVECTOR(x, y, z);
    }

    public TV_3DVECTOR GetLocalRotation()
    {
      return Rotation;
    }

    public void SetLocalRotation(float x, float y, float z)
    {
      Rotation = new TV_3DVECTOR(x, y, z);
    }

    public TV_3DVECTOR GetDirection()
    {
      TV_3DVECTOR ret = Utilities.GetDirection(Rotation);
      ActorInfo a = AttachToParent ? Owner.Engine.ActorFactory.Get(ParentID) : null;
      if (a != null)
        ret += a.GetDirection();

      TV_3DVECTOR dir = new TV_3DVECTOR();
      Globals.Engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public void SetDirection(float x, float y, float z)
    {
      TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
      ActorInfo a = AttachToParent ? Owner.Engine.ActorFactory.Get(ParentID) : null;
      if (a != null)
        dir -= a.GetDirection();

      Rotation = Utilities.GetRotation(dir);
    }

    public TV_3DVECTOR GetLocalDirection()
    {
      TV_3DVECTOR ret = Utilities.GetDirection(Rotation);

      TV_3DVECTOR dir = new TV_3DVECTOR();
      Globals.Engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public void SetLocalDirection(float x, float y, float z)
    {
      TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
      Rotation = Utilities.GetRotation(dir);
    }

    public void LookAtPoint(TV_3DVECTOR target, bool preserveZrotation = false)
    {
      float zrot = GetRotation().z;
      TV_3DVECTOR dir = target - GetPosition();
      SetDirection(dir.x, dir.y, dir.z);

      if (preserveZrotation)
      {
        SetRotation(GetRotation().x, GetRotation().y, zrot);
      }
    }

    public void MoveRelative(float front, float up, float right)
    {
      TV_3DVECTOR vec = GetRelativePositionFUR(front, up, right, true);
      SetLocalPosition(vec.x, vec.y, vec.z);
    }

    public void MoveAbsolute(float x, float y, float z)
    {
      TV_3DVECTOR vec = GetLocalPosition() + new TV_3DVECTOR(x, y, z);
      SetLocalPosition(vec.x, vec.y, vec.z);
    }

    #endregion

    public void SetSpawnerEnable(bool value)
    {
      if (SpawnerInfo != null)
        SpawnerInfo.Enabled = value;
    }


    public float GetTrueSpeed()
    {
      float ret = MovementInfo.Speed;
      ActorInfo a = AttachToParent ? Owner.Engine.ActorFactory.Get(ParentID) : null;
      if (a != null)
        ret += a.GetTrueSpeed();

      return ret;
    }

    public void Rotate(float x, float y, float z)
    {
      TV_3DVECTOR vec = GetRotation();
      SetRotation(vec.x + x, vec.y + y, vec.z + z);
    }

    #region Parent, Child and Relatives
    public void AddParent(int actorID)
    {
      if (ParentID >= 0)
        RemoveParent();

      ParentID = actorID;
    }

    public void RemoveParent()
    {
      ParentID = -1;
    }

    public bool HasParent(int actorID, int searchlevel = 99)
    {
      if (searchlevel < 0)
        return false;

      bool ret = false;
      if (ParentID < 0)
        return false;

      ret |= (ParentID == actorID) || (Owner.Engine.ActorFactory.Get(ParentID)?.HasParent(actorID, searchlevel - 1) ?? false);

      return ret;
    }

    public int GetTopParent(int searchlevel = 99)
    {
      if (ParentID < 0 || searchlevel <= 1)
        return ID;
      else
        return Owner.Engine.ActorFactory.Get(ParentID)?.GetTopParent(searchlevel - 1) ?? ID;
    }

    public List<int> GetAllParents(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<int>();

      List<int> ret = new List<int>();
      if (searchlevel > 1)
      {
        if (ParentID >= 0 && !ret.Contains(ParentID))
        {
          ret.Add(ParentID);
          ret.AddRange(Owner.Engine.ActorFactory.Get(ParentID)?.GetAllParents(searchlevel - 1));
        }
      }
      else
      {
        if (ParentID >= 0)
          ret.Add(ParentID);
      }
      return ret;
    }

    public List<int> GetAllChildren(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<int>();

      List<int> ret = new List<int>();

      foreach (int actorID in Owner.Engine.ActorFactory.GetHoldingList())
      {
        ActorInfo p = Owner.Engine.ActorFactory.Get(actorID);
        if (p != null && p.HasParent(ID, searchlevel))
          ret.Add(p.ID);
      }
      return ret;
    }

    public bool HasRelative(int actorID, int searchlevel = 99, List<int> alreadysearched = null)
    {
      if (searchlevel < 0)
        return false;

      if (alreadysearched == null)
        alreadysearched = new List<int>();

      ActorInfo actor = Owner.Engine.ActorFactory.Get(actorID);
      return (actor != null && actor.GetTopParent() == GetTopParent());
    }

    public List<int> GetAllRelatives(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<int>();

      List<int> ret = new List<int>();

      foreach (int actorID in Owner.Engine.ActorFactory.GetHoldingList())
      {
        ActorInfo p = Owner.Engine.ActorFactory.Get(actorID);
        if (p != null && p.GetTopParent() == GetTopParent())
          ret.Add(p.ID);
      }
      return ret;
    }

    #endregion

    #region Event Methods
    public void OnTickEvent(object[] param) { TickEvents?.Invoke(ID); }
    public void OnHitEvent(int victimID) { HitEvents?.Invoke(ID, victimID); }
    public void OnStateChangeEvent(object[] param) { ActorStateChangeEvents?.Invoke(ID); }
    public void OnCreatedEvent(object[] param) { CreatedEvents?.Invoke(ID); }
    public void OnDestroyedEvent(object[] param) { DestroyedEvents?.Invoke(ID); }

    #endregion

    public bool IsOutOfBounds(TV_3DVECTOR minbounds, TV_3DVECTOR maxbounds)
    {
      TV_3DVECTOR pos = GetPosition();
      return (pos.x < minbounds.x)
          || (pos.x > maxbounds.x)
          || (pos.y < minbounds.y)
          || (pos.y > maxbounds.y)
          || (pos.z < minbounds.z)
          || (pos.z > maxbounds.z);
    }

    public bool IsNearlyOutOfBounds(float dx = 1000, float dy = 250, float dz = 1000)
    {
      TV_3DVECTOR pos = GetPosition();
      return (pos.x < Globals.Engine.GameScenarioManager.MinBounds.x + dx)
          || (pos.x > Globals.Engine.GameScenarioManager.MaxBounds.x - dx)
          || (pos.y < Globals.Engine.GameScenarioManager.MinBounds.y + dy)
          || (pos.y > Globals.Engine.GameScenarioManager.MaxBounds.y - dy)
          || (pos.z < Globals.Engine.GameScenarioManager.MinBounds.z + dz)
          || (pos.z > Globals.Engine.GameScenarioManager.MaxBounds.z - dz);
    }

    public float GetWeaponRange()
    {
      float ret = 0;
      foreach (WeaponInfo w in WeaponSystemInfo.Weapons.Values)
      {
        if (ret < w.Range)
          ret = w.Range;
      }
      return ret;
    }

    public bool SelectWeapon(int targetActorID, float delta_angle, float delta_distance, out WeaponInfo weapon, out int burst)
    {
      weapon = null;
      burst = 0;
      WeaponInfo weap = null;
      foreach (string ws in WeaponSystemInfo.AIWeapons)
      {
        TypeInfo.InterpretWeapon(ID, ws, out weap, out burst);

        if (weap != null)
        {
          if ((delta_angle < weap.AngularRange
            && delta_angle > -weap.AngularRange)
            && (delta_distance < weap.Range
            && delta_distance > -weap.Range)
            && weap.CanTarget(ID, targetActorID)
            && (weap.MaxAmmo == -1 || weap.Ammo > 0))
          {
            weapon = weap;
            return true;
          }
        }
      }
      return false;
    }

    public bool IsAggregateMode()
    {
      ActorInfo a = (Globals.Engine.PlayerInfo.Actor != null) ? Globals.Engine.PlayerInfo.Actor : Globals.Engine.GameScenarioManager.SceneCamera;
      float distcheck = TypeInfo.CullDistance * Globals.Engine.Game.PerfCullModifier;

      return (!IsPlayer() && TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetRoughDistance(ID, a.ID) > distcheck);
    }

    public bool IsFarMode()
    {
      ActorInfo a = (Globals.Engine.PlayerInfo.Actor != null) ? Globals.Engine.PlayerInfo.Actor : Globals.Engine.GameScenarioManager.SceneCamera;
      float distcheck = TypeInfo.CullDistance * 0.25f * Globals.Engine.Game.PerfCullModifier;

      return (!IsPlayer() && TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetRoughDistance(ID, a.ID) > distcheck);
    }

    public bool IsPlayer()
    {
      return this == Globals.Engine.PlayerInfo.Actor;
    }

    public bool IsScenePlayer()
    {
      return IsPlayer() || this == Globals.Engine.PlayerInfo.TempActor;
    }

    public float StrengthFrac
    {
      get
      {
        float f = CombatInfo.Strength / TypeInfo.MaxStrength;
        if (f > 1)
          f = 1;
        else if (f < 0)
          f = 0;
        return f;
      }
    }

    public void Kill()
    {
      Owner.Engine.ActorFactory.MakeDead(this);
    }

    public void Destroy()
    {
      using (new PerfElement("actor_process_destroy"))
      {
        //if (CreationState == CreationState.DISPOSED)
        //  return;

        // Parent
        RemoveParent();

        // Destroy Children
        foreach (int i in GetAllChildren(1))
        {
          ActorInfo child = Owner.Engine.ActorFactory.Get(i);

          if (child.TypeInfo is ActorTypes.Groups.AddOn || child.AttachToParent)
            child.Destroy();
          else
            child.RemoveParent();
        }
        
        // Mesh
        if (Mesh != null)
          Mesh.Destroy();
        Mesh = null;

        if (FarMesh != null)
          FarMesh.Destroy();
        FarMesh = null;

        AttachToParent = false;

        // Particle System
        if (ParticleSystem != null)
          ParticleSystem.Destroy();
        ParticleSystem = null;

        // Actions
        CurrentAction = null;
        //ActionManager.ClearQueue(this);

        // Reset components
        Score.Reset();
        CombatInfo.Reset();
        MovementInfo.Reset();
        CycleInfo.Reset();
        RegenerationInfo.Reset();
        ExplosionInfo.Reset();
        WeaponSystemInfo.Reset();
        CollisionInfo.Reset();

        // Events
        OnDestroyedEvent(new object[] { this, ActorState });
        CreatedEvents = null;
        DestroyedEvents = null;
        TickEvents = null;
        HitEvents = null;
        ActorStateChangeEvents = null;

        // Player
        if (IsPlayer())
          Globals.Engine.PlayerInfo.ActorID = -1;
        else if (this == Globals.Engine.PlayerInfo.TempActor)
          Globals.Engine.PlayerInfo.TempActorID = -1;

        // Final dispose
        Faction.UnregisterActor(this);
        Owner.Engine.ActorFactory.Remove(ID);

        // Finally
        CreationState = CreationState.DISPOSED;
      }
    }
  }
}
