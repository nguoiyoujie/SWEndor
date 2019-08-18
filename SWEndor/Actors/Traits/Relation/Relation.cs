using SWEndor.Primitives;
using SWEndor.Primitives.Traits;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors.Traits
{
  public class Relation<T> : IRelation<T> where T : class, ITraitOwner
  {
    public T Parent { get; set; }
    public bool UseParentCoords { get; set; }
    LinkedList<T> list = new LinkedList<T>();

    public void Init()
    {
      Parent = null;
      UseParentCoords = false;
      list.Clear();
    }

    public void AddChild(T self, T child, IRelation<T> childrel)
    {
      if (self.Equals(child))
        throw new InvalidOperationException("Adding the same {0} instance as its own child is not allowed.".F(typeof(T).Name));

      childrel.Parent = self;

      if (!list.Contains(child))
        list.AddLast(child);
    }

    public void RemoveChild(T self, T child, IRelation<T> childrel)
    {
      if (child == null)
        return;

      if (self.Equals(childrel.Parent))
        childrel.Parent = null;

      list.Remove(child);
    }

    public IEnumerable<T> Children
    {
      get
      {
        LinkedListNode<T> node = list.First;
        while (node != null)
        {
          yield return node.Value;
          node = node.Next;
        }
      }
    }

    public T GetTopParent(T self)
    {
      T ret = default(T);
      IRelation<T> trt;
      if (Parent != null && !Parent.Equals(self) && Parent.TryGetTrait(out trt))
      {
        ret = trt.GetTopParent(Parent); // Stack overflow?
        if (ret != null)
          return ret;
        else return self;
      }
      else
        return self;
    }

    public IEnumerable<T> Siblings
    {
      get
      {
        IRelation<T> trt;
        if (Parent.TryGetTrait(out trt))
          return trt.Children;
        else
          return new T[0];
      }
    }
    /*
    public T PrevSibling { get; set; }
    public T NextSibling { get; set; }

    public T FirstChild { get; set; }
    public T LastChild { get; set; }
    //public int NumberOfChildren { get; set; }

    public void Init()
    {
      Parent = null;
      UseParentCoords = false;
      PrevSibling = null;
      NextSibling = null;
      FirstChild = null;
      LastChild = null;
      //NumberOfChildren = 0;
    }

    public void AddChild(T self, T child)
    {
      if (self.Equals(child))
        throw new InvalidOperationException("Adding the same {0} instance as its own child is not allowed.".F(typeof(T).Name));

      IRelation<T> cTrait;
      bool bcT = child.TryGetTrait(out cTrait);
      if (bcT)
        cTrait.Parent = self;

      if (FirstChild == null) //NumberOfChildren == 0)
      {
        FirstChild = child;
      }
      else
      {
        IRelation<T> lTrait;
        if (LastChild.TryGetTrait(out lTrait))
          lTrait.NextSibling = child;

        if (bcT)
          cTrait.PrevSibling = LastChild;
      }
      LastChild = child;
      //NumberOfChildren++;
    }

    public void RemoveChild(T self, T child)
    {
      if (child == null)
        return;

      IRelation<T> cTrait;
      bool bcT = child.TryGetTrait(out cTrait);
      if (bcT && self.Equals(cTrait.Parent))
      {
        //NumberOfChildren--;
        //if (NumberOfChildren < -1)
        //{ }
        cTrait.Parent = default(T);

        if (child.Equals(FirstChild))
        {
          FirstChild = cTrait.NextSibling;
        }
        else if (child.Equals(LastChild))
        {
          LastChild = cTrait.PrevSibling;
        }
        else
        {
          IRelation<T> pTrait;
          IRelation<T> nTrait;
          if (cTrait.PrevSibling != null && cTrait.PrevSibling.TryGetTrait(out pTrait))
            pTrait.NextSibling = cTrait.NextSibling;

          if (cTrait.NextSibling != null && cTrait.NextSibling.TryGetTrait(out nTrait))
            nTrait.PrevSibling = cTrait.PrevSibling;
        }
      }
    }

    public IEnumerable<T> Children
    {
      get
      {
        T child = FirstChild;
        IRelation<T> trt;
        //for (int i = 0; i < NumberOfChildren && child != null; i++)
        while (child != null)
        {
          yield return child;
          if (child.TryGetTrait(out trt))
            child = trt.NextSibling;
          else
            break;
        }
      }
    }

    public T GetTopParent(T self)
    {
      T ret = default(T);
      IRelation<T> trt;
      if (Parent != null && !Parent.Equals(self) && Parent.TryGetTrait(out trt))
      {
        ret = trt.GetTopParent(Parent); // Stack overflow?
        if (ret != null)
          return ret;
        else return self;
      }
      else
        return self;
    }

    public IEnumerable<T> Siblings
    {
      get
      {
        IRelation<T> trt;
        if (Parent.TryGetTrait(out trt))
          return trt.Children;
        else
          return new T[0];
      }
    }
    */
  }
}
