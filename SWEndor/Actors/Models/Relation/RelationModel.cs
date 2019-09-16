using SWEndor.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public struct RelationModel
    {
      public ActorInfo Parent { get; private set; }
      public ActorInfo ParentForCoords { get { return UseParentCoords ? Parent : null; } }
      public bool UseParentCoords { get; set; }
      private List<ActorInfo> list;
      public ChildEnumerable Children;

      public void Init()
      {
        Parent = null;
        UseParentCoords = false;
        if (list == null)
        {
          list = new List<ActorInfo>();
          Children = new ChildEnumerable(list);
        }
        else
          list.Clear();
      }

      public void Dispose(ActorInfo self)
      {
        // unlink from parent
        Parent?.RemoveChild(self);

        // Destroy Children
        foreach (ActorInfo c in list.ToArray()) // use new list as members are deleted from the IEnumerable
        {
          using (ScopeCounterManager.Acquire(c.Scope))
            if (c.TypeInfo is ActorTypes.Groups.AddOn || c.Relation.UseParentCoords)
              c.Destroy();
            else
              self.RemoveChild(c);
        }

        UseParentCoords = false;
      }

      internal void AddChild(ActorInfo self, ActorInfo child)
      {
        if (self == child)
          throw new InvalidOperationException("Adding the same ActorInfo instance as its own child is not allowed.");

        child.Relation.Parent = self;

        if (list != null && !list.Contains(child))
          list.Add(child);
      }

      internal void RemoveChild(ActorInfo self, ActorInfo child)
      {
        if (child == null)
          return;

        if (self == child.Relation.Parent)
          child.Relation.Parent = null;

        list?.Remove(child);
      }

      internal ActorInfo GetTopParent(ActorInfo self)
      {
        ActorInfo ret;
        if (Parent != null && Parent != self)
        {
          ret = Parent.Relation.GetTopParent(Parent); // Stack overflow?
          return ret ?? self;
        }
        else
          return self;
      }

      internal ChildEnumerable Siblings { get { return Parent.Relation.Children; } }

      public struct ChildEnumerable
      {
        readonly List<ActorInfo> L;
        public ChildEnumerable(List<ActorInfo> list) { L = list; }
        public ChildEnumerator GetEnumerator() { return new ChildEnumerator(L); }
      }
      
      public struct ChildEnumerator
      {
        readonly List<ActorInfo> L;
        int current;
        public ChildEnumerator(List<ActorInfo> list) { L = list; current = -1; }

        public void Reset() { current = -1; }
        public bool MoveNext() { return ++current < L.Count; }
        public ActorInfo Current { get { return L[current]; } }
        public void Dispose() { }
      }
    }

    public void AddChild(ActorInfo a)
    {
      using (ScopeCounterManager.Acquire(Scope))
      using (ScopeCounterManager.Acquire(a.Scope))
        Relation.AddChild(this, a);
    }

    public void RemoveChild(ActorInfo a)
    {
      using (ScopeCounterManager.Acquire(Scope))
      using (ScopeCounterManager.Acquire(a.Scope))
        Relation.RemoveChild(this, a);
    }

    public RelationModel.ChildEnumerable Children { get { return Relation.Children; } }
    public ActorInfo Parent { get { return Relation.Parent; } }
    public ActorInfo TopParent
    {
      get
      {
        using (ScopeCounterManager.Acquire(Scope))
          return Relation.GetTopParent(this);
      }
    }
    public RelationModel.ChildEnumerable Siblings { get { return Relation.Siblings; } }
    public ActorInfo ParentForCoords { get { return Relation.ParentForCoords; } }
    public bool UseParentCoords { get { return Relation.UseParentCoords; } set { Relation.UseParentCoords = value; } }
  }
}
