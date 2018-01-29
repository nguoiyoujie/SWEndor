using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Tower01_RuinsATI : DebrisGroup
  {
    private static Tower01_RuinsATI _instance;
    public static Tower01_RuinsATI Instance()
    {
      if (_instance == null) { _instance = new Tower01_RuinsATI(); }
      return _instance;
    }

    private Tower01_RuinsATI() : base("Turbolaser Tower 01 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_01_destroyed.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = false;
      ainfo.EnableExplosions = false;
    }
  }
}

