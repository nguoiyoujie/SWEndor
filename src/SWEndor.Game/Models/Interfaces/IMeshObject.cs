using MTV3D65;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.Models
{
  /// <summary>
  /// Represents a mesh object
  /// </summary>
  public interface IMeshObject
  {
    int GetVertexCount();
    TV_3DVECTOR GetVertex(int vertID);
    float3 Scale { get; }
    TV_3DVECTOR MaxDimensions { get; }
    TV_3DVECTOR MinDimensions { get; }
  }
}