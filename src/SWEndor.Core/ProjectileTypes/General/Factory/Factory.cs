using SWEndor.Core;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using System;
using System.IO;
using Primrose;

namespace SWEndor.ProjectileTypes
{
  public partial class ProjectileTypeInfo
  {
    internal class Factory : Registry<ProjectileTypeInfo>
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      public void Register(ProjectileTypeInfo atype)
      {
        Put(atype.ID, atype);
        Engine.Screen2D.LoadingTextLines.Add(string.Format("{0} loaded!", atype.Name));
      }
      
      public void Load()
      {
        foreach (string fp in Directory.GetFiles(Globals.ProjectileTypeINIDirectory, Globals.INIExt, SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "ProjectileType", f);
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

