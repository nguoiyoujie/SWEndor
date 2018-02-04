using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ExplosionATI : ExplosionGroup
  {
    private static ExplosionATI _instance;
    public static ExplosionATI Instance()
    {
      if (_instance == null) { _instance = new ExplosionATI(); }
      return _instance;
    }

    List<int> texanimframes = new List<int>();

    private ExplosionATI() : base("Explosion")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 0.5f;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 2;
      AnimationCyclePeriod = 0.5f;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        // 8 textures (not including 0).
        for (int i = 1; i <= 8; i++)
        {
          string texname = string.Format(@"expl{0:0}.jpg", i);
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
        SourceMesh.CreateBox(10, 10, 0.001f);
        SourceMesh.SetTexture(texanimframes[0]);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        TV_3DVECTOR pos = PlayerInfo.Instance().Camera.GetWorldPosition(new TV_3DVECTOR(0, 0, -1000));
        ainfo.LookAtPoint(pos);

        int k = texanimframes.Count - 1 - (int)(ainfo.TimedLife / AnimationCyclePeriod * texanimframes.Count);
        if (k >= 0 && k < texanimframes.Count)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}

