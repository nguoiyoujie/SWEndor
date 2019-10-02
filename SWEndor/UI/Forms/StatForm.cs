using SWEndor.Core;
using SWEndor.Primitives;
using System;
using System.Windows.Forms;

namespace SWEndor.UI.Forms
{
  public partial class StatForm : Form
  {
    internal StatForm(Engine engine)
    {
      Engine = engine;
      InitializeComponent();
      TopMost = true;
      tmTick.Start();
    }

    private readonly Engine Engine;

    private void tmTick_Tick(object sender, EventArgs e)
    {
      lblFPS.Text = Engine.Game.CurrentFPS.ToString();
      lblGameTime.Text = Engine.Game.GameTime.ToString();
      lblRenderTime.Text = Engine.Game.TimeSinceRender.ToString();

      lblActorTypes.Text = Engine.ActorTypeFactory.Count.ToString();
      lblActors.Text = Engine.ActorFactory.Count.ToString();
      lblExplosionTypes.Text = Engine.ExplosionTypeFactory.Count.ToString();
      lblExplosions.Text = Engine.ExplosionFactory.Count.ToString();
      lblDynPiece.Text = Sound.SoundManager.Piece.Factory.Count.ToString();

      lblActorDistanceCache.Text = Actors.ActorDistanceInfo.CacheCount.ToString();

      lblScopedPool.Text = ScopedManager<Actors.ActorInfo>.PoolCount.ToString();
      lblSquad.Text = Engine.SquadronFactory.Count.ToString();
      lblSquadPool.Text = Engine.SquadronFactory.PoolCount.ToString();

      lblActorsPool_Plan.Text = Engine.ActorFactory.PoolPlanCount.ToString();
      lblActorsPool_Prep.Text = Engine.ActorFactory.PoolPrepCount.ToString();
      lblActorsPool_Main.Text = Engine.ActorFactory.PoolMainCount.ToString();
      lblActorsPool_Dead.Text = Engine.ActorFactory.PoolDeadCount.ToString();
      lblActorsPool_TPlan.Text = Engine.ActorFactory.TempPoolPlanCount.ToString();
      lblActorsPool_TPrep.Text = Engine.ActorFactory.TempPoolRedoCount.ToString();
      lblActorsPool_TDead.Text = Engine.ActorFactory.TempPoolDeadCount.ToString();

      lblExplPool_Plan.Text = Engine.ExplosionFactory.PoolPlanCount.ToString();
      lblExplPool_Prep.Text = Engine.ExplosionFactory.PoolPrepCount.ToString();
      lblExplPool_Main.Text = Engine.ExplosionFactory.PoolMainCount.ToString();
      lblExplPool_Dead.Text = Engine.ExplosionFactory.PoolDeadCount.ToString();
      lblExplPool_TPlan.Text = Engine.ExplosionFactory.TempPoolPlanCount.ToString();
      lblExplPool_TPrep.Text = Engine.ExplosionFactory.TempPoolRedoCount.ToString();
      lblExplPool_TDead.Text = Engine.ExplosionFactory.TempPoolDeadCount.ToString();
    }
  }
}
