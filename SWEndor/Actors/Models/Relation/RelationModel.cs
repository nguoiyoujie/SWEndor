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
      private LinkedList<ActorInfo> list;
      public ChildEnumerable Children;

      public void Init()
      {
        Parent = null;
        UseParentCoords = false;
        if (list == null)
        {
          list = new LinkedList<ActorInfo>();
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
        foreach (ActorInfo c in Children)//ToArray()) // use new list as members are deleted from the IEnumerable
        {
          if (c.TypeInfo is ActorTypes.Groups.AddOn || c.Relation.UseParentCoords)
            c.Destroy();
          else
            self.RemoveChild(c);
        }

        UseParentCoords = false;
      }

      public void AddChild(ActorInfo self, ActorInfo child)
      {
        if (self == child)
          throw new InvalidOperationException("Adding the same ActorInfo instance as its own child is not allowed.");

        child.Relation.Parent = self;

        if (list != null && !list.Contains(child))
          list.AddLast(child);
      }

      public void RemoveChild(ActorInfo self, ActorInfo child)
      {
        if (child == null)
          return;

        if (self == child.Relation.Parent)
          child.Relation.Parent = null;

        list?.Remove(child);
      }

      public ActorInfo GetTopParent(ActorInfo self)
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

      public ChildEnumerable Siblings { get { return Parent.Relation.Children; } }

      public struct ChildEnumerable
      {
        readonly LinkedList<ActorInfo> L;
        public ChildEnumerable(LinkedList<ActorInfo> list) { L = list; }
        public LinkedList<ActorInfo>.Enumerator GetEnumerator() { return L.GetEnumerator(); } // new ChildEnumerator(L); }
        //public ChildEnumerator GetEnumerator() { return new ChildEnumerator(L); }
      }

      /*
      public struct ChildEnumerator : IEnumerator<ActorInfo>
      {
        readonly LinkedList<ActorInfo> L;
        LinkedListNode<ActorInfo> current;
        public ChildEnumerator(LinkedList<ActorInfo> list) { L = list; current = null; }

        public void Reset() { current = null; }
        public bool MoveNext() { return (current = (current == null) ? L?.First : current?.Next) != null; }
        public ActorInfo Current { get { return current?.Value; } }
        object System.Collections.IEnumerator.Current { get { return Current; } }
        public void Dispose() { }
      }
      */
    }

    public void AddChild(ActorInfo a) { Relation.AddChild(this, a); }
    public void RemoveChild(ActorInfo a) { Relation.RemoveChild(this, a); }
    public RelationModel.ChildEnumerable Children { get { return Relation.Children; } }
    public ActorInfo Parent { get { return Relation.Parent; } }
    public ActorInfo TopParent { get { return Relation.GetTopParent(this); } }
    public RelationModel.ChildEnumerable Siblings { get { return Relation.Siblings; } }
    public ActorInfo ParentForCoords { get { return Relation.ParentForCoords; } }
    public bool UseParentCoords { get { return Relation.UseParentCoords; } set { Relation.UseParentCoords = value; } }
  }
}
