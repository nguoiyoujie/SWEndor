using System.Windows.Forms;
using SWEndor.Game.Core;

namespace SWEndor.Game.UI.Forms.UIControls
{
  public partial class ucStatObjects : UserControl
  {
    public ucStatObjects()
    {
      InitializeComponent();
    }

    public void Update(Engine engine)
    {
      lblActorTypes.Text = engine.ActorTypeFactory.Count.ToString();
      lblActors.Text = engine.ActorFactory.Count.ToString();

      lblExplosionTypes.Text = engine.ExplosionTypeFactory.Count.ToString();
      lblExplosions.Text = engine.ExplosionFactory.Count.ToString();

      lblProjectileTypes.Text = engine.ProjectileTypeFactory.Count.ToString();
      lblProjectiles.Text = engine.ProjectileFactory.Count.ToString();

      lblSquad.Text = engine.SquadronFactory.Count.ToString();


      lblActorsPool_Plan.Text = engine.ActorFactory.PoolPlanCount.ToString();
      lblActorsPool_Prep.Text = engine.ActorFactory.PoolPrepCount.ToString();
      lblActorsPool_Pool.Text = engine.ActorFactory.PoolMainCount.ToString();
      lblActorsPool_Dead.Text = engine.ActorFactory.PoolDeadCount.ToString();
      lblActorsPool_TPlan.Text = engine.ActorFactory.TempPoolPlanCount.ToString();
      lblActorsPool_TPrep.Text = engine.ActorFactory.TempPoolRedoCount.ToString();
      lblActorsPool_TDead.Text = engine.ActorFactory.TempPoolDeadCount.ToString();

      lblExplPool_Plan.Text = engine.ExplosionFactory.PoolPlanCount.ToString();
      lblExplPool_Prep.Text = engine.ExplosionFactory.PoolPrepCount.ToString();
      lblExplPool_Pool.Text = engine.ExplosionFactory.PoolMainCount.ToString();
      lblExplPool_Dead.Text = engine.ExplosionFactory.PoolDeadCount.ToString();
      lblExplPool_TPlan.Text = engine.ExplosionFactory.TempPoolPlanCount.ToString();
      lblExplPool_TPrep.Text = engine.ExplosionFactory.TempPoolRedoCount.ToString();
      lblExplPool_TDead.Text = engine.ExplosionFactory.TempPoolDeadCount.ToString();

      lblProjPool_Plan.Text = engine.ProjectileFactory.PoolPlanCount.ToString();
      lblProjPool_Prep.Text = engine.ProjectileFactory.PoolPrepCount.ToString();
      lblProjPool_Pool.Text = engine.ProjectileFactory.PoolMainCount.ToString();
      lblProjPool_Dead.Text = engine.ProjectileFactory.PoolDeadCount.ToString();
      lblProjPool_TPlan.Text = engine.ProjectileFactory.TempPoolPlanCount.ToString();
      lblProjPool_TPrep.Text = engine.ProjectileFactory.TempPoolRedoCount.ToString();
      lblProjPool_TDead.Text = engine.ProjectileFactory.TempPoolDeadCount.ToString();
    }
  }
}
