using System.Windows.Forms;
using SWEndor.Game.Core;
using SWEndor.Game.AI.Actions;
using Primrose.Primitives.Factories;
using System.Collections.Generic;
using Primrose.Primitives.ValueTypes;
using MTV3D65;
using SWEndor.Game.Sound;
using SWEndor.Game.AI.Squads;

namespace SWEndor.Game.UI.Forms.UIControls
{
  public partial class ucPoolCache : UserControl
  {
    private class PairCollection<T, U> : List<Pair<T, U>>
    {
      public void Add(T t, U u)
      {
        Pair<T, U> item = new Pair<T, U>(t, u);
        Add(item);
      }
    }

    public ucPoolCache()
    {
      InitializeComponent();
    }

    public void Init(Engine engine)
    {
      engine.SquadronFactory.Return(engine.SquadronFactory.Create());
      AttackActor.GetOrCreate(0).Return();
      AvoidCollisionRotate.GetOrCreate(default(TV_3DVECTOR), default(TV_3DVECTOR)).Return();
      AvoidCollisionWait.GetOrCreate().Return();
      CustomAction.GetOrCreate(null).Return();
      Delete.GetOrCreate().Return();
      EnableSpawn.GetOrCreate(false).Return();
      Evade.GetOrCreate().Return();
      FollowActor.GetOrCreate(0).Return();
      ForcedMove.GetOrCreate(default(TV_3DVECTOR), 0).Return();
      Hunt.GetOrCreate().Return();
      HyperspaceIn.GetOrCreate(default(TV_3DVECTOR)).Return();
      HyperspaceOut.GetOrCreate().Return();
      Idle.GetOrCreate().Return();
      Lock.GetOrCreate().Return();
      AI.Actions.Move.GetOrCreate(default(TV_3DVECTOR), 0).Return();
      PlaySound.GetOrCreate(null, false).Return();
      ProjectileAttackActor.GetOrCreate(null).Return();
      Rotate.GetOrCreate(default(TV_3DVECTOR), 0).Return();
      SelfDestruct.GetOrCreate().Return();
      SetGameStateB.GetOrCreate(null, Enabled).Return();
      SetMood.GetOrCreate(default(MoodState), false).Return();
      Wait.GetOrCreate().Return();

      PairCollection<string, IPool> pools = new PairCollection<string, IPool>
      {
        { "Squadron", ObjectPool<Squadron>.GetStaticPool() },
        { "Actions.AttackActor", ObjectPool<AttackActor>.GetStaticPool()},
        { "Actions.AvoidCollisionRotate", ObjectPool<AvoidCollisionRotate>.GetStaticPool()},
        { "Actions.AvoidCollisionWait", ObjectPool<AvoidCollisionWait>.GetStaticPool()},
        { "Actions.CustomAction", ObjectPool<CustomAction>.GetStaticPool()},
        { "Actions.Delete", ObjectPool<Delete>.GetStaticPool()},
        { "Actions.EnableSpawn", ObjectPool<EnableSpawn>.GetStaticPool()},
        { "Actions.Evade", ObjectPool<Evade>.GetStaticPool()},
        { "Actions.FollowActor", ObjectPool<FollowActor>.GetStaticPool()},
        { "Actions.ForcedMove", ObjectPool<ForcedMove>.GetStaticPool()},
        { "Actions.Hunt", ObjectPool<Hunt>.GetStaticPool()},
        { "Actions.HyperspaceIn", ObjectPool<HyperspaceIn>.GetStaticPool()},
        { "Actions.HyperspaceOut", ObjectPool<HyperspaceOut>.GetStaticPool()},
        { "Actions.Idle", ObjectPool<Idle>.GetStaticPool()},
        { "Actions.Lock", ObjectPool<Lock>.GetStaticPool()},
        { "Actions.Move", ObjectPool<Move>.GetStaticPool()},
        { "Actions.PlaySound", ObjectPool<PlaySound>.GetStaticPool()},
        { "Actions.ProjectileAttackActor", ObjectPool<ProjectileAttackActor>.GetStaticPool()},
        { "Actions.Rotate", ObjectPool<Rotate>.GetStaticPool()},
        { "Actions.SelfDestruct", ObjectPool<SelfDestruct>.GetStaticPool()},
        { "Actions.SetGameStateB", ObjectPool<SetGameStateB>.GetStaticPool()},
        { "Actions.SetMood", ObjectPool<SetMood>.GetStaticPool()},
        { "Actions.Wait",  ObjectPool<Wait>.GetStaticPool()}
      };

      int y = 80;
      foreach (Pair<string, IPool> kvp in pools)
      {
        Label txt = new Label();
        txt.Location = new System.Drawing.Point(44, y);
        txt.Size = new System.Drawing.Size(180, 15);
        txt.Text = kvp.t;
        //44, 232, 276

        Label count1 = new Label();
        count1.Location = new System.Drawing.Point(lblPool.Location.X, y);
        count1.Size = new System.Drawing.Size(lblPool.Size.Width, 15);
        count1.Text = "0";

        Label count2 = new Label();
        count2.Location = new System.Drawing.Point(lblInUse.Location.X, y);
        count2.Size = new System.Drawing.Size(lblInUse.Size.Width, 15);
        count2.Text = "0";

        Controls.Add(txt);
        Controls.Add(count1);
        Controls.Add(count2);
        counters.Add(new Pair<Label, Label>(count1, count2), kvp.u);

        y += 15;
      }
    }

    Dictionary<Pair<Label, Label>, IPool> counters = new Dictionary<Pair<Label, Label>, IPool>();

    public void Update(Engine engine)
    {
      lblActorDistanceCache.Text = Models.DistanceModel.CacheCount.ToString();
      lblActorDistancePool.Text = Models.DistanceModel.PoolCount.ToString();
      lblActorDistanceInUse.Text = Models.DistanceModel.PoolUncollectedCount.ToString();

      foreach (KeyValuePair<Pair<Label, Label>, IPool> kvp in counters)
      {
        kvp.Key.t.Text = kvp.Value.Count.ToString();
        kvp.Key.u.Text = kvp.Value.UncollectedCount.ToString();
      }

    }
  }
}
