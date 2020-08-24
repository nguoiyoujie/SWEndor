using SWEndor.Game.ActorTypes.Instances;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using System;
using System.IO;
using Primrose;

namespace SWEndor.Game.ActorTypes
{
  public partial class ActorTypeInfo
  {
    public class Factory : Registry<ActorTypeInfo>
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      public void RegisterBase()
      {
        // Special
        Register(new Hyperspace(this));

        // Hangars
        Register(new HangarBayATI(this));
        Register(new PlayerSpawnerATI(this));

        // Actortypes that is not yet replaced
        Register(new ExecutorBridgeATI(this));
      }

      public void Register(ActorTypeInfo atype)
      {
        Put(atype.ID, atype);
        Engine?.Screen2D.LoadingTextLines.Add(string.Format("{0} loaded!", atype.Name));
      }

      public void Load()
      {
        foreach (string fp in Directory.GetFiles(Globals.ActorTypeINIDirectory, Globals.INIExt, SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.ASSET_LOADING), "ActorType", f);
          if (Contains(f))
            throw new InvalidOperationException(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INITWICE_ERROR).F(f));
          ActorTypeInfo t = new ActorTypeInfo(this, f, f);
          t.LoadFromINI(f, fp);
          Register(t);
        }
      }

      public void Initialise()
      {
        foreach (ActorTypeInfo atype in list.Values)
          atype.Init();
      }

      public new ActorTypeInfo Get(string id)
      {
        ActorTypeInfo ret = base.Get(id);
        if (ret == null)
          throw new Exception(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INVALID_ERROR).F(id));

        return ret;
      }
    }
  }
}
