using Primrose.Primitives;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    /// <summary>
    /// Adds a child to this object
    /// </summary>
    /// <param name="a">The child object</param>
    public void AddChild(ActorInfo a)
    {
      using (ScopeCounters.Acquire(Scope))
      using (ScopeCounters.Acquire(a.Scope))
        Relation.AddChild(this, a);
    }

    /// <summary>
    /// Removes a child from this object
    /// </summary>
    /// <param name="a">The child object</param>
    public void RemoveChild(ActorInfo a)
    {
      using (ScopeCounters.Acquire(Scope))
      using (ScopeCounters.Acquire(a.Scope))
        Relation.RemoveChild(this, a);
    }

    internal Models.RelationModel.ChildEnumerable Children { get { return Relation.Children; } }

    /// <summary>The parent of this object</summary>
    public ActorInfo Parent { get { return Relation.Parent; } internal set { Relation.Parent = value; } }

    /// <summary>The top-level parent of this object</summary>
    public ActorInfo TopParent
    {
      get
      {
        using (ScopeCounters.Acquire(Scope))
          return Relation.GetTopParent(this);
      }
    }

    internal Models.RelationModel.ChildEnumerable Siblings { get { return Relation.Siblings; } }

    /// <summary>The parent of this object when considering coordinate positions</summary>
    public ActorInfo ParentForCoords { get { return Relation.ParentForCoords; } }

    /// <summary>Whether the global position of this object is relative to its parent</summary>
    public bool UseParentCoords { get { return Relation.UseParentCoords; } set { Relation.UseParentCoords = value; } }
  }
}
