using MTV3D65;
using System.IO;

namespace SWEndor.Actors.Types
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

      EnableDistanceCull = false;


      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        string texname = Path.Combine("explosion", "wave", @"tex0000.jpg");
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int tex = LoadAlphaTexture(texname, texpath);

        SourceMesh.CreateBox(100, 0.001f, 100f);
        SourceMesh.SetTexture(tex);
        SourceMesh.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);

        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.NORMAL)
        ainfo.Scale += new TV_3DVECTOR(100, 0, 100) * Game.Instance().TimeSinceRender;
    }
  }
}

