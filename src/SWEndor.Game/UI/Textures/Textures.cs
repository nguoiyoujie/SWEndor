using MTV3D65;
using Primrose.Primitives.ValueTypes;
using System.IO;

namespace SWEndor.Game.UI
{
  public class Textures
  {
    public Textures(Screen2D owner)
    {
      Texture_Target_fighter = owner.Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"targets\tgt_sm.png"), nameof(Texture_Target_fighter), Texture_Target_size.x, Texture_Target_size.y, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
      Texture_Target_leader = owner.Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"targets\tgt_lead.png"), nameof(Texture_Target_leader), Texture_Target_size.x, Texture_Target_size.y, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
      Texture_Target_hardpoint = owner.Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"targets\tgt_gun.png"), nameof(Texture_Target_hardpoint), Texture_Target_size.x, Texture_Target_size.y, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
      Texture_Target_projectile = owner.Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"targets\tgt_misl.png"), nameof(Texture_Target_projectile), Texture_Target_size.x, Texture_Target_size.y, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
      Texture_Target_ship_top_left = owner.Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"targets\tgt_lg_00.png"), nameof(Texture_Target_ship_top_left), Texture_Target_size.x, Texture_Target_size.y, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
      Texture_Target_ship_top_right = owner.Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"targets\tgt_lg_01.png"), nameof(Texture_Target_ship_top_right), Texture_Target_size.x, Texture_Target_size.y, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
      Texture_Target_ship_bottom_left = owner.Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"targets\tgt_lg_10.png"), nameof(Texture_Target_ship_bottom_left), Texture_Target_size.x, Texture_Target_size.y, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);
      Texture_Target_ship_bottom_right = owner.Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"targets\tgt_lg_11.png"), nameof(Texture_Target_ship_bottom_right), Texture_Target_size.x, Texture_Target_size.y, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL);

    }

    internal readonly int2 Texture_Target_size = new int2(16, 16);

    internal readonly int Texture_Target_fighter;
    internal readonly int Texture_Target_leader;
    internal readonly int Texture_Target_hardpoint;
    internal readonly int Texture_Target_projectile;
    internal readonly int Texture_Target_ship_top_left;
    internal readonly int Texture_Target_ship_top_right;
    internal readonly int Texture_Target_ship_bottom_left;
    internal readonly int Texture_Target_ship_bottom_right;
  }
}
