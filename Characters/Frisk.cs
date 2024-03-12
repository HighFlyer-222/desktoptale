﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class Frisk : Character
    {
        public Frisk(CharacterCreationContext characterCreationContext) : base(characterCreationContext) {}
    
        public override void LoadContent(ContentManager contentManager)
        {
            OrientedAnimatedSprite idleSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Up"), 4,    
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Down"), 4,    
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Left"), 2
            );
            idleSprite.Loop = false;
            idleSprite.Framerate = 0;
            IdleSprite = idleSprite;

            OrientedAnimatedSprite walkSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Up"), 4,    
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Down"), 4,    
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Left"), 2
            );
            walkSprite.Loop = true;
            walkSprite.Framerate = 5;
            walkSprite.StartFrame = 1;
            WalkSprite = walkSprite;

            OrientedAnimatedSprite runSprite = new OrientedAnimatedSprite(
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Up"), 4,    
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Down"), 4,    
                contentManager.Load<Texture2D>("Included/Frisk/Spr_Frisk_Walk_Left"), 2
            );
            runSprite.Loop = true;
            runSprite.Framerate = 10;
            runSprite.StartFrame = 1;
            RunSprite = runSprite;
        }
    }
}