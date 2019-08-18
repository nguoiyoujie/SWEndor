using SWEndor.Actors;
using SWEndor.Actors.Traits;
using SWEndor.Primitives;
using SWEndor.Primitives.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SWEndor.Actors.ActorInfo;
using SWEndor;

namespace SWEndorTest
{
  public class DummyTestActor : ITraitOwner
  {
    private readonly TraitCollection Traits;

    public DummyTestActor()
    {
      Traits = new TraitCollection(this);
    }

    public Engine Engine { get { return Globals.Engine; } }
    public int ID { get { return 0; } }
    public string Name { get { return "Dummy Actor"; } }
    public ITraitOwner Owner { get { return null; } }

    public T Trait<T>() where T : ITrait { return Traits.Get<T>(); }
    public bool TryGetTrait<T>(out T trait) where T : ITrait { return Traits.TryGet(out trait); }
    public T TraitOrDefault<T>() where T : ITrait { return Traits.GetOrDefault<T>(); }
    public IEnumerable<T> TraitsImplementing<T>() where T : ITrait { return Traits.TraitsImplementing<T>(); }
    public T AddTrait<T>(T trait) where T : ITrait { return Traits.Add(trait); }

    public void Dispose() { }
    public bool Disposed { get { return false; } }

    /*
    public class UnitTestActor : ITraitOwner
    {
      // Identifiers
      private string _name = "Test Actor";
      private string sidebar_name = "";

      public string Name { get { return _name; } }
      public int ID { get; private set; }
      public string Key { get { return "{0} {1}".F(_name, ID); } }

      public override string ToString() { return Key; }

      // Checks
      public bool EnteredCombatZone = false;

      // AI
      public ActionInfo CurrentAction = null;
      public bool CanEvade = true;
      public bool CanRetaliate = true;
      public int HuntWeight = 1;

      // Traits
      public IStateModel StateModel { get; private set; }
      public IMeshModel MeshModel { get; private set; }
      public IHealth Health { get; private set; }
      public DyingTimer DyingTimer { get; private set; }
      public ITransform Transform { get; private set; }
      public IRelation<UnitTestActor> Relation { get; private set; }

      // 
      //private TraitCollection Traits { get { return Engine.Traits; } }
      private readonly TraitCollection Traits;
      //public List<Action<ActorInfo>> PostFrameActions = new List<Action<ActorInfo>>();

      private UnitTestActor(string name, int id)
      {
        ID = id;

        Traits = new TraitCollection(this);
        InitializeTraits(acinfo);
      }

      public void Destroy()
      {
        using (var scope = ScopedManager<UnitTestActor>.Scope(this))
        {
          if (StateModel.CreationState == CreationState.DISPOSING
            || StateModel.CreationState == CreationState.DISPOSED)
            return;

          StateModel.SetDisposing();

          // Parent
          Relation.Parent?.RemoveChild(this);

          // Destroy Children
          foreach (ActorInfo c in new List<ActorInfo>(Children)) // use new list as members are deleted from the IEnumerable
          {
            if (c.TypeInfo is ActorTypes.Groups.AddOn || c.Relation.UseParentCoords)
              c.Destroy();
            else
              RemoveChild(c);
          }

          Relation.UseParentCoords = false;

          // Actions
          CurrentAction = null;

          // Dispose Traits
          foreach (IDisposableTrait t in TraitsImplementing<IDisposableTrait>())
          {
            t.Dispose();
            Engine.TraitPoolCollection.ReturnTrait(t);
          }

          StateModel.SetDisposed();
          Traits.State = TraitCollectionState.DISPOSED;
          Traits.Clear();

          // Final dispose
          Engine.ActorFactory.Remove(ID);

          Log.Write(Log.DEBUG, LogType.ACTOR_DISPOSED, this);
        }
      }

      public T Trait<T>() where T : ITrait { return Traits.Get<T>(); }

      public bool TryGetTrait<T>(out T trait) where T : ITrait { return Traits.TryGet(out trait); }

      public T TraitOrDefault<T>() where T : ITrait { return Traits.GetOrDefault<T>(); }

      public IEnumerable<T> TraitsImplementing<T>() where T : ITrait { return Traits.TraitsImplementing<T>(); }

      public T AddTrait<T>(T trait) where T : ITrait { return Traits.Add(trait); }

      public void Dispose()
      {
        Destroy();
      }

      public bool Planned { get { return StateModel != null && StateModel.CreationState == CreationState.PLANNED; } }

      public bool Active { get { return StateModel != null && StateModel.CreationState == CreationState.ACTIVE; } }

      public bool Disposed { get { return StateModel == null || StateModel.CreationState == CreationState.DISPOSED; } }

    }
    */
  }
}
