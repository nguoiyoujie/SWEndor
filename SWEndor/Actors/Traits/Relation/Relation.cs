using SWEndor.Primitives;
using SWEndor.Primitives.Traits;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors.Traits
{
  public class Relation<T> : IRelation<T> where T : ITraitOwner
  {
    public T Parent { get; set; }
    public bool UseParentCoords { get; set; }

    public T PrevSibling { get; set; }
    public T NextSibling { get; set; }

    public T FirstChild { get; set; }
    public T LastChild { get; set; }
    public int NumberOfChildren { get; set; }

    public void Init()
    {
      Parent = default(T);
      UseParentCoords = false;
      PrevSibling = default(T);
      NextSibling = default(T);
      FirstChild = default(T);
      LastChild = default(T);
      NumberOfChildren = 0;
    }

    public void AddChild(T self, T child)
    {
      if (self.Equals(child))
        throw new InvalidOperationException("Adding the same {0} instance as its own child is not allowed.".F(typeof(T).Name));

      IRelation<T> cTrait;
      bool bcT = child.TryGetTrait(out cTrait);
      if (bcT)
        cTrait.Parent = self;

      if (NumberOfChildren == 0)
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
      NumberOfChildren++;
    }

    public void RemoveChild(T self, T child)
    {
      if (child == null)
        return;

      IRelation<T> cTrait;
      bool bcT = child.TryGetTrait(out cTrait);
      if (bcT && self.Equals(cTrait.Parent))
      {
        NumberOfChildren--;
        if (NumberOfChildren < -1)
        { }

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

        cTrait.Parent = default(T);
      }
    }

    public IEnumerable<T> Children
    {
      get
      {
        T child = FirstChild;
        IRelation<T> trt;
        for (int i = 0; i < NumberOfChildren && child != null; i++)
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
      if (Parent != null && Parent.TryGetTrait(out trt))
      {
        ret = trt.GetTopParent(Parent);
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
  }
}
