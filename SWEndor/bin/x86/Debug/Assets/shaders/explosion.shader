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
expl01.jpg > Explosion01
expl02.jpg > Explosion02
expl03.jpg > Explosion03
expl04.jpg > Explosion04
expl05.jpg > Explosion05
expl06.jpg > Explosion06
expl07.jpg > Explosion07
expl08.jpg > Explosion08
expl09.jpg > Explosion09
expl10.jpg > Explosion10
expl11.jpg > Explosion11
expl12.jpg > Explosion12
expl13.jpg > Explosion13
expl14.jpg > Explosion14
expl15.jpg > Explosion15
expl16.jpg > Explosion16
elk1.jpg > Electrico1
elk2.jpg > Electrico2
elk3.jpg > Electrico3
elk4.jpg > Electrico4
deathstar.jpg > DeathStar1

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

