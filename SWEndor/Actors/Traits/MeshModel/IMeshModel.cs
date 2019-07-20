using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
  public interface IMeshModel : ITrait, IDisposableTrait
  {
    void Init(int id, ActorTypeInfo atype);
    BoundingBox GetBoundingBox(bool uselocal);
    BoundingSphere GetBoundingSphere(bool uselocal);
    int GetVertexCount();
    TV_3DVECTOR GetVertex(int vertexID);
    void SetTexture(int iTexture);
    void Render(bool renderfar);
    void Update(ActorInfo actor); // should replace ActorInfo with a generic ITraitOwner?
  }
}
