using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
  public interface ITransform : ITrait
  {
    void Init(ActorTypeInfo type, ActorCreationInfo acinfo);

    TV_3DMATRIX MatrixRotation { get; }
    TV_3DMATRIX MatrixTranslation { get; }
    TV_3DMATRIX MatrixScale { get; }
    TV_3DMATRIX GetWorldMatrix<A>(A self, float time) where A : class, ITraitOwner;

    float Scale { get; set; }
    TV_3DVECTOR Position { get; set; }
    TV_3DVECTOR PrevPosition { get; }
    TV_3DVECTOR Direction { get; set; }
    TV_3DVECTOR PrevDirection { get; }
    TV_3DVECTOR Rotation { get; set; }
    TV_3DVECTOR PrevRotation { get; }


    TV_3DVECTOR GetGlobalPosition<A>(A self, float time) where A : class, ITraitOwner;
    TV_3DVECTOR GetPrevGlobalPosition<A>(A self, float time) where A : class, ITraitOwner;

    TV_3DVECTOR GetGlobalRotation<A>(A self) where A : class, ITraitOwner;
    TV_3DVECTOR GetPrevGlobalRotation<A>(A self) where A : class, ITraitOwner;

    TV_3DVECTOR GetGlobalDirection<A>(A self) where A : class, ITraitOwner;
    TV_3DVECTOR GetPrevGlobalDirection<A>(A self) where A : class, ITraitOwner;

    //TV_3DVECTOR GetLocalDirection();
    //ITransform SetLocalDirection(TV_3DVECTOR newDir);
    TV_3DVECTOR GetRelativePositionFUR<A>(A self, float time, float forward, float up = 0, float right = 0, bool local = false) where A : class, ITraitOwner;
    TV_3DVECTOR GetRelativePositionXYZ<A>(A self, float time, float x, float y, float z, bool local = false) where A : class, ITraitOwner;
    ITransform LookAt(TV_3DVECTOR target, bool preserveZrotation = false);
  }
}
