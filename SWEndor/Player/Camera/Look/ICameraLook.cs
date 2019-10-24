using MTV3D65;
using SWEndor.Core;

namespace SWEndor.Player
{
  public interface ICameraLook
  {
    TV_3DVECTOR GetPosition(Engine engine);
    void Update(Engine engine, TVCamera cam, float rotz);
  }
}
