using SWEndor.Core;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using SWEndor.ProjectileTypes.Instances;
using System;
using System.IO;

namespace SWEndor.ProjectileTypes
{
  public partial class ProjectileTypeInfo
  {
    internal class Factory : Registry<ProjectileTypeInfo>
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      public void RegisterBase()
      {
        // lasers
        Register(new RedLaserATI(this));
        Register(new GreenLaserATI(this));
        Register(new Green2LaserATI(this));
        Register(new GreenAntiShipLaserATI(this));
        Register(new GreenLaserAdvancedATI(this));
        Register(new YellowLaserATI(this));
        Register(new Yellow2LaserATI(this));
        Register(new SmallIonLaserATI(this));
        Register(new BigIonLaserATI(this));
        Register(new DeathStarLaserATI(this));
      }

      public void Register(ProjectileTypeInfo atype)
      {
        if (Contains(atype.ID))
        {
          atype = GetX(atype.ID);
        }
        else
        {
          Add(atype.ID, atype);
        }
        //atype.SaveToINI(atype.ID);
        Engine.Screen2D.LoadingTextLines.Add(string.Format("{0} loaded!", atype.Name));
      }

      
      public void Load()
      {
        foreach (string fp in Directory.GetFiles(Globals.ProjectileTypeINIDirectory, Globals.INIExt, SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          if (Contains(f))
            throw new InvalidOperationException(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INITWICE_ERROR).F(f));
          ProjectileTypeInfo t = new ProjectileTypeInfo(this, f, f);
          t.LoadFromINI(f, fp);
          Register(t);
        }
      }
      

      public void Initialise()
      {
        foreach (ProjectileTypeInfo atype in list.Values)
          atype.Init();
      }

      public new ProjectileTypeInfo Get(string id)
      {
        ProjectileTypeInfo ret = base.Get(id);
        if (ret == null)
          throw new Exception(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INVALID_ERROR).F(id));

        return ret;
      }
    }
  }
}

