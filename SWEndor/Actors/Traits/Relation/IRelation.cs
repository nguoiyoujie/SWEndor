using SWEndor.Primitives.Traits;
using System.Collections.Generic;

namespace SWEndor.Actors.Traits
{
  public interface IRelation<T> : ITrait where T : ITraitOwner
  {
    void Init(); 

    T Parent { get; set; }
    bool UseParentCoords { get; set; }

    T PrevSibling { get; set; }
    T NextSibling { get; set; }

    T FirstChild { get; set; }
    T LastChild { get; set; }
    int NumberOfChildren { get; }

    void AddChild(T self, T child);
    void RemoveChild(T self, T child);

    T GetTopParent(T self);
    IEnumerable<T> Children { get; }
    IEnumerable<T> Siblings { get; }
  }
}
