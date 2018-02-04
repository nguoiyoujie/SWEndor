using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Explosion2ATI : ExplosionGroup
  {
    private static Explosion2ATI _instance;
    public static Explosion2ATI Instance()
    {
      if (_instance == null) { _instance = new Explosion2ATI(); }
      return _instance;
    }

    List<int> texanimframes = new List<int>();

    private Explosion2ATI() : base("Explosion2")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 1;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 5;
      AnimationCyclePeriod = 1;

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
        SourceMesh.CreateBox(40, 40, 0.01f);
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

