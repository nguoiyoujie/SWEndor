using System.Windows.Forms;
using SWEndor.Core;
using Primrose.Primitives;

namespace SWEndor.UI.Forms.UIControls
{
  public partial class ucPoolCache : UserControl
  {
    public ucPoolCache()
    {
      InitializeComponent();
    }

    public void Update(Engine engine)
    {
      lblActorDistanceCache.Text = Models.DistanceModel.CacheCount.ToString();

      lblScopedPool.Text = ScopedManager<Actors.ActorInfo>.PoolCount.ToString();
      lblSquadPool.Text = engine.SquadronFactory.PoolCount.ToString();

      lblActionHuntPool.Text = AI.Actions.Hunt._pool.Count.ToString();
      lblActionHunt.Text = AI.Actions.Hunt._count.ToString();

      lblActionAttackPool.Text = AI.Actions.AttackActor._pool.Count.ToString();
      lblActionAttack.Text = AI.Actions.AttackActor._count.ToString();

      lblActionAvoidRotatePool.Text = AI.Actions.AvoidCollisionRotate._pool.Count.ToString();
      lblActionAvoidRotate.Text = AI.Actions.AvoidCollisionRotate._count.ToString();

      lblActionAvoidWaitPool.Text = AI.Actions.AvoidCollisionWait._pool.Count.ToString();
      lblActionAvoidWait.Text = AI.Actions.AvoidCollisionWait._count.ToString();

      lblActionEvadePool.Text = AI.Actions.Evade._pool.Count.ToString();
      lblActionEvade.Text = AI.Actions.Evade._count.ToString();
      
    }
  }
}
