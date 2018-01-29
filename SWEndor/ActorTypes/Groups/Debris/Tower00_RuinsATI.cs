using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Tower00_RuinsATI : DebrisGroup
  {
    private static Tower00_RuinsATI _instance;
    public static Tower00_RuinsATI Instance()
    {
      if (_instance == null) { _instance = new Tower00_RuinsATI(); }
      return _instance;
    }

    private Tower00_RuinsATI() : base("Turbolaser Tower 00 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_00_destroyed.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = false;
      ainfo.EnableExplosions = false;
    }
  }
}

