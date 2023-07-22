using MTV3D65;

namespace SWEndor.Game.Models
{
  /// <summary>
  /// Represents a transformable object
  /// </summary>
  public interface ITransformable
  {
    /// <summary>The local position of the object</summary>
    TV_3DVECTOR Position { get; }

    /// <summary>Gets the world transformation matrix of the object</summary>
    TV_3DMATRIX GetWorldMatrix();

    /// <summary>Gets the previous world transformation matrix of the object</summary>
    TV_3DMATRIX GetPrevWorldMatrix();

    /// <summary>Extracts the world position from the object's transformation matrix</summary>
    TV_3DVECTOR GetGlobalPosition();

    /// <summary>Extracts the previous world position from the object's transformation matrix</summary>
    TV_3DVECTOR GetPrevGlobalPosition();

    /// <summary>Gets the relative offset from an object</summary>
    TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, bool uselocal = false);

    /// <summary>Gets the relative offset from an object</summary>
    TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool local = false);
  }
}