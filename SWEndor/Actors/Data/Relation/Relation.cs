using SWEndor.Primitives;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public struct Relation
  {
    public ActorInfo Parent { get; set; }
    public bool UseParentCoords { get; set; }
    LinkedList<ActorInfo> list;

    public void Init()
    {
      Parent = null;
      UseParentCoords = false;
      if (list == null)
        list = new LinkedList<ActorInfo>();
      else
        list.Clear();
    }

    public void AddChild(ActorInfo self, ActorInfo child)
    {
      if (self.Equals(child))
        throw new InvalidOperationException("Adding the same ActorInfo instance as its own child is not allowed.");

      child.Relation.Parent = self;

      if (!list.Contains(child))
        list.AddLast(child);
    }

    public void RemoveChild(ActorInfo self, ActorInfo child)
    {
      if (child == null)
        return;

      if (self.Equals(child.Relation.Parent))
        child.Relation.Parent = null;

      list.Remove(child);
    }

    public IEnumerable<ActorInfo> Children
    {
      get
      {
        LinkedListNode<ActorInfo> node = list.First;
        while (node != null)
        {
          yield return node.Value;
          node = node.Next;
        }
      }
    }

    public ActorInfo GetTopParent(ActorInfo self)
    {
      ActorInfo ret;
      if (Parent != null && !Parent.Equals(self))
      {
        ret = Parent.Relation.GetTopParent(Parent); // Stack overflow?
        return ret ?? self;
      }
      else
        return self;
    }

    public IEnumerable<ActorInfo> Siblings { get { return Parent.Relation.Children; } }
  }
}
