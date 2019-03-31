using MTV3D65;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class TransportATI : WarshipGroup
  {
    private static TransportATI _instance;
    public static TransportATI Instance()
    {
      if (_instance == null) { _instance = new TransportATI(); }
      return _instance;
    }

    private TransportATI() : base("Transport")
    {
      MaxStrength = 250.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 40.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 3f;

      Score_perStrength = 8;
      Score_DestroyBonus = 3000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", new TV_3DVECTOR(0, 0, -150), 500.0f, true) };
      AddOns = new AddOnInfo[]
      {
        new AddOnInfo("Transport Turbolaser Tower", new TV_3DVECTOR(0, 70, 200), new TV_3DVECTOR(-90, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 86, -150));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 86, 2000));
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      if (ainfo.ActorState == ActorState.DEAD)
      {
        // special debris
        if (!ainfo.IsAggregateMode() && !Game.Instance().IsLowFPS())
        {
          float px = 0;
          float py = 0;
          float pz = 0;
          float dx = 0;
          float dy = 0;
          float dz = 0;
          int imax = Engine.Instance().Random.Next(0, 15);
          for (int i = 0; i < imax; i++)
          {
            px = Engine.Instance().Random.Next(-6000, 6000) / 100f;
            py = Engine.Instance().Random.Next(-1000, 3500) / 100f;
            pz = Engine.Instance().Random.Next(7500, 25500) / 100f;
            dx = Engine.Instance().Random.Next(-1000, 1000) / 100f;
            dy = Engine.Instance().Random.Next(-2000, 2000) / 100f;
            dz = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorTypeInfo[] types = new ActorTypeInfo[] {
                                                            Transport_Box1ATI.Instance(),
                                                            Transport_Box2ATI.Instance(),
                                                            Transport_Box3ATI.Instance(),
                                                            Transport_Box4ATI.Instance()
                                                          };

            ActorCreationInfo acinfo = new ActorCreationInfo(types[Engine.Instance().Random.Next(0, types.Length)]);
            acinfo.Position = ainfo.GetRelativePositionFUR(px, py, pz);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + dx, ainfo.Rotation.y + dy, ainfo.Rotation.z + dz);

            acinfo.InitialSpeed = ainfo.MovementInfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);

          }
        }
      }
    }
  }
}

