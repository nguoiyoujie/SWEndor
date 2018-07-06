using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ExplosionLgATI : ExplosionGroup
  {
    private static ExplosionLgATI _instance;
    public static ExplosionLgATI Instance()
    {
      if (_instance == null) { _instance = new ExplosionLgATI(); }
      return _instance;
    }

    List<int> texanimframes = new List<int>();

    private ExplosionLgATI() : base("ExplosionLg")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      RadarSize = 20;
      AnimationCyclePeriod = 1;

      EnableDistanceCull = false;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 16 textures (including 00).
        for (int i = 0; i <= 15; i++)
        {
          string texname = string.Format(@"expl{0:00}.jpg", i);
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
        SourceMesh.CreateBox(1000, 1000, 0.01f);
        SourceMesh.SetTexture(texanimframes[0]);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      if (!ainfo.IsPlayer() && PlayerInfo.Instance().Actor != null && !GameScenarioManager.Instance().IsCutsceneMode)
      {
        float dist = ActorDistanceInfo.GetDistance(PlayerInfo.Instance().Actor, ainfo, 3001);
        if (dist < 3000)
        {
          if (PlayerInfo.Instance().exp_navevol < 1 - dist / 3000)
            PlayerInfo.Instance().exp_navevol = 1 - dist / 3000;
        }
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        TV_3DVECTOR pos = PlayerInfo.Instance().Camera.GetWorldPosition(new TV_3DVECTOR(0,0,-1000));
        ainfo.LookAtPoint(pos);

        int k = texanimframes.Count - 1 - (int)(ainfo.TimedLife / AnimationCyclePeriod * texanimframes.Count);
        if (k >= 0 && k < texanimframes.Count)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}

