using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ExplosionWaveATI : ExplosionGroup
  {
    private static ExplosionWaveATI _instance;
    public static ExplosionWaveATI Instance()
    {
      if (_instance == null) { _instance = new ExplosionWaveATI(); }
      return _instance;
    }

    private ExplosionWaveATI() : base("Explosion Wave")
    {
      // Combat
      OnTimedLife = true;
      TimedLife = 2f;
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 0;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

          string texname = @"wave.jpg";
          string texpath = Path.Combine(Globals.ShaderPath, texname);
        int tex = -1;
        if (Engine.Instance().TVGlobals.GetTex(texname) == 0)
        {
          int texS = Engine.Instance().TVTextureFactory.LoadTexture(texpath);
          int texA = Engine.Instance().TVTextureFactory.LoadAlphaTexture(texpath);
          tex = Engine.Instance().TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }
        else
        {
          tex = Engine.Instance().TVGlobals.GetTex(texname);
        }

        SourceMesh.CreateBox(100, 0.001f, 100f);
        SourceMesh.SetTexture(tex);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.NORMAL)
      {
        ainfo.Scale += new TV_3DVECTOR(100, 0, 100) * Game.Instance().TimeSinceRender;
      }
    }
  }
}

