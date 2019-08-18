using SWEndor.Primitives.Traits;
using System.Collections.Generic;

namespace SWEndor.Actors.Traits
{
  public interface IRelation<T> : ITrait where T : class, ITraitOwner
  {
    void Init(); 

    T Parent { get; set; }
    bool UseParentCoords { get; set; }

    void AddChild(T self, T child, IRelation<T> childrel);
    void RemoveChild(T self, T child, IRelation<T> childrel);

    T GetTopParent(T self);
    IEnumerable<T> Children { get; }
    IEnumerable<T> Siblings { get; }
  }
}
