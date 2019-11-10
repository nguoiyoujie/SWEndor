using MTV3D65;

namespace SWEndor.Models
{
  /// <summary>
  /// Represents a mesh object
  /// </summary>
  public interface IMeshObject
  {
    int GetVertexCount();
    TV_3DVECTOR GetVertex(int vertID);
    float Scale { get; }
    TV_3DVECTOR MaxDimensions { get; }
    TV_3DVECTOR MinDimensions { get; }
  }
}