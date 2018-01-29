//#####################################################################//
//                     TrueVision8 Shader Script                       //
//#####################################################################//
//                                                                     //
//  Common Shader for Flames					       //
//								       //
// You have to define the used textures before creating the Shader     //
// At the left, the texture filename (can be in a PAK file)            //
// At the right, the texture name that will be used in the engine      //
//								       //	
//#####################################################################//

TEXTURES 21
shaders\expl01.jpg > Explosion01
shaders\expl02.jpg > Explosion02
shaders\expl03.jpg > Explosion03
shaders\expl04.jpg > Explosion04
shaders\expl05.jpg > Explosion05
shaders\expl06.jpg > Explosion06
shaders\expl07.jpg > Explosion07
shaders\expl08.jpg > Explosion08
shaders\expl09.jpg > Explosion09
shaders\expl10.jpg > Explosion10
shaders\expl11.jpg > Explosion11
shaders\expl12.jpg > Explosion12
shaders\expl13.jpg > Explosion13
shaders\expl14.jpg > Explosion14
shaders\expl15.jpg > Explosion15
shaders\expl16.jpg > Explosion16
shaders\elk1.jpg > Electrico1
shaders\elk2.jpg > Electrico2
shaders\elk3.jpg > Electrico3
shaders\elk4.jpg > Electrico4
shaders\deathstar.jpg > DeathStar1

//#####################################
// Shaders Declarations. 
//#####################################


//
// Explosion effect
//
SHADER Explosion
{
   Layer map1
   {    

       //
       // Texture Animation :
       // Syntax : anim [FramePerSecond] [Tex1] [Tex2] ... [TexN] [0]
       //

         anim 16 Explosion01 Explosion02 Explosion03 Explosion04 Explosion05 Explosion06 Explosion07 Explosion08 Explosion09 Explosion10 Explosion11 Explosion12 Explosion13 Explosion14 Explosion15 Explosion16 [0]

       // Blender functions
       // Two ways : Blend [NO|ADD|ALPHA|COLOR|LIGHTMAP]
       //            BlendFunc [ZERO|ONE|ALPHA|INVALPHA|COLOR|INVCOLOR|DESTCOLOR] [ONE|ZERO|SRCCOLOR|ALPHA|INVALPHA|COLOR|INVCOLOR]
       //                             src blend                                    dest blend
      
	 blendFunc ONE ONE
         depthFunc NOWRITE

    }
}   


SHADER Electrico
{
   Layer map1
   {    

       //
       // Texture Animation :
       // Syntax : anim [FramePerSecond] [Tex1] [Tex2] ... [TexN] [0]
       //

         anim 8 Electrico1 Electrico2 Electrico3 Electrico4 [0]

       // Blender functions
       // Two ways : Blend [NO|ADD|ALPHA|COLOR|LIGHTMAP]
       //            BlendFunc [ZERO|ONE|ALPHA|INVALPHA|COLOR|INVCOLOR|DESTCOLOR] [ONE|ZERO|SRCCOLOR|ALPHA|INVALPHA|COLOR|INVCOLOR]
       //                             src blend                                    dest blend
      
	 blendFunc ONE ONE
         depthFunc NOWRITE

    }
}   

