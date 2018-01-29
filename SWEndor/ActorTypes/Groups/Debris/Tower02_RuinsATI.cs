using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Tower02_RuinsATI : DebrisGroup
  {
    private static Tower02_RuinsATI _instance;
    public static Tower02_RuinsATI Instance()
    {
      if (_instance == null) { _instance = new Tower02_RuinsATI(); }
      return _instance;
    }

    private Tower02_RuinsATI() : base("Turbolaser Tower 02 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_02_destroyed.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = false;
      ainfo.EnableExplosions = false;
    }
  }
}

