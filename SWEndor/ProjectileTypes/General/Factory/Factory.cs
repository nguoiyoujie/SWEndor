/*
namespace SWEndor.ProjectileTypes
{
  public partial class ProjectileTypeInfo
  {
    public class Factory : Registry<ProjectileTypeInfo>
    {
      //private static Dictionary<string, ActorTypeInfo> list = new Dictionary<string, ActorTypeInfo>();

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

        // torps
        Register(new MissileATI(this));
        Register(new TorpedoATI(this));
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
        foreach (string fp in Directory.GetFiles(Globals.ActorTypeINIDirectory, "*.ini", SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          if (Contains(f))
            throw new InvalidOperationException(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INITWICE_ERROR).F(f));
          ActorTypeInfo t = new ActorTypeInfo(this, f, f);
          t.LoadFromINI(f);
        }
      }
      

      public void Initialise()
      {
        foreach (ProjectileTypeInfo atype in list.Values)
          atype.Init();
      }

      public new ProjectileTypeInfo Get(string id)
      {
        ActorTypeInfo ret = base.Get(id);
        if (ret == null)
          throw new Exception(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INVALID_ERROR).F(id));

        return ret;
      }
    }
  }
}
*/
