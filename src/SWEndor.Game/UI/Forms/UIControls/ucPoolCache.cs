using System.Windows.Forms;
using SWEndor.Game.Core;
using SWEndor.Game.AI.Actions;
using Primrose.Primitives.Factories;
using System.Collections.Generic;
using Primrose.Primitives.ValueTypes;
using Primrose.Primitives.Extensions;
using MTV3D65;
using SWEndor.Game.Sound;

namespace SWEndor.Game.UI.Forms.UIControls
{
  public partial class ucPoolCache : UserControl
  {
    public ucPoolCache()
    {
      InitializeComponent();
    }

    public void Init()
    {
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

      IPool[] pools = new IPool[]
      {
        ObjectPool<AttackActor>.GetStaticPool(),
        ObjectPool<AvoidCollisionRotate>.GetStaticPool(),
        ObjectPool<AvoidCollisionWait>.GetStaticPool(),
        ObjectPool<CustomAction>.GetStaticPool(),
        ObjectPool<Delete>.GetStaticPool(),
        ObjectPool<EnableSpawn>.GetStaticPool(),
        ObjectPool<Evade>.GetStaticPool(),
        ObjectPool<FollowActor>.GetStaticPool(),
        ObjectPool<ForcedMove>.GetStaticPool(),
        ObjectPool<Hunt>.GetStaticPool(),
        ObjectPool<HyperspaceIn>.GetStaticPool(),
        ObjectPool<HyperspaceOut>.GetStaticPool(),
        ObjectPool<Idle>.GetStaticPool(),
        ObjectPool<Lock>.GetStaticPool(),
        ObjectPool<Move>.GetStaticPool(),
        ObjectPool<PlaySound>.GetStaticPool(),
        ObjectPool<ProjectileAttackActor>.GetStaticPool(),
        ObjectPool<Rotate>.GetStaticPool(),
        ObjectPool<SelfDestruct>.GetStaticPool(),
        ObjectPool<SetGameStateB>.GetStaticPool(),
        ObjectPool<SetMood>.GetStaticPool(),
        ObjectPool<Wait>.GetStaticPool(),
      };

      int y = 100;
      foreach (IPool p in pools)
      {
        Label txt = new Label();
        txt.Location = new System.Drawing.Point(44, y);
        txt.Size = new System.Drawing.Size(180, 15);
        txt.Text = "Actions.{0}".F(p.GetType().GetGenericArguments()[0].Name);
        //44, 232, 276

        Label count1 = new Label();
        count1.Location = new System.Drawing.Point(230, y);
        count1.Size = new System.Drawing.Size(40, 15);
        count1.Text = "0";

        Label count2 = new Label();
        count2.Location = new System.Drawing.Point(276, y);
        count2.Size = new System.Drawing.Size(40, 15);
        count2.Text = "0";

        Controls.Add(txt);
        Controls.Add(count1);
        Controls.Add(count2);
        counters.Add(new Pair<Label, Label>(count1, count2), p);

        y += 15;
      }
    }

    Dictionary<Pair<Label, Label>, IPool> counters = new Dictionary<Pair<Label, Label>, IPool>();

    public void Update(Engine engine)
    {
      lblActorDistanceCache.Text = Models.DistanceModel.CacheCount.ToString();
      lblSquadPool.Text = engine.SquadronFactory.PoolCount.ToString();

      foreach (KeyValuePair<Pair<Label, Label>, IPool> kvp in counters)
      {
        kvp.Key.t.Text = kvp.Value.Count.ToString();
        kvp.Key.u.Text = kvp.Value.UncollectedCount.ToString();
      }

    }
  }
}
