using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ElectroATI : ActorTypeInfo
  {
    private static ElectroATI _instance;
    public static ElectroATI Instance()
    {
      if (_instance == null) { _instance = new ElectroATI(); }
      return _instance;
    }

    List<int> texanimframes = new List<int>();

    private ElectroATI() : base("Electro")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 0.5f;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 5;
      AnimationCyclePeriod = 0.5f;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 4 textures (not including 0).
        for (int i = 1; i <= 4; i++)
        {
          string texname = string.Format(@"elk{0:0}.jpg", i);
          string texpath = Path.Combine(Globals.ShaderPath, texname);
          if (Engine.Instance().TVGlobals.GetTex(texname) == 0)
          {
            int texS = Engine.Instance().TVTextureFactory.LoadTexture(texpath);
            int texA = Engine.Instance().TVTextureFactory.LoadAlphaTexture(texpath);
            texanimframes.Add(Engine.Instance().TVTextureFactory.AddAlphaChannel(texS, texA, texname));
          }
          else
          {
            texanimframes.Add(Engine.Instance().TVGlobals.GetTex(texname));
          }
        }
        SourceMesh.CreateBox(75, 75, 0.001f);
        SourceMesh.SetTexture(texanimframes[0]);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Generate States
      if (!ainfo.IsStateFDefined("CyclesRemaining"))
        ainfo.SetStateF("CyclesRemaining", 99);
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.DYING && (!ainfo.IsStateFDefined("CyclesRemaining") || ainfo.GetStateF("CyclesRemaining") > 0))
      {
        ainfo.ActorState = ActorState.NORMAL;
        ainfo.TimedLife = TimedLife;

        if (ainfo.IsStateFDefined("CyclesRemaining"))
          ainfo.SetStateF("CyclesRemaining", ainfo.GetStateF("CyclesRemaining") - 1);
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        TV_3DVECTOR p = PlayerInfo.Instance().Camera.GetWorldPosition(new TV_3DVECTOR(0, 0, -1000));
        ainfo.LookAtPoint(p);

        List<ActorInfo> elep = ainfo.GetAllParents(1);
        if (elep.Count > 0)
        {
          if (elep[0].CreationState != CreationState.DISPOSED)
          {
            TV_3DVECTOR pos = elep[0].GetPosition();
            ainfo.SetLocalPosition(pos.x, pos.y, pos.z);
          }
          else
          {
            ainfo.RemoveParent(elep[0]);
          } 
        }
        else
        {
          ainfo.ActorState = ActorState.DEAD;
        }

        int k = texanimframes.Count - 1 - (int)(ainfo.TimedLife / AnimationCyclePeriod * texanimframes.Count);
        if (k >= 0 && k < texanimframes.Count)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}

