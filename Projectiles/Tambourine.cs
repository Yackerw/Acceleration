using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID; 
//hydro's stink stain 
namespace Acceleration.Projectiles
{
	class Tambourine : ModProjectile
    {
        Vector2[] rotpos = new Vector2[8];
        float[] rotations = new float[8]; 

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.alpha = 0;
            projectile.timeLeft = 360;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.light = 0.3f;
        }
        public override void AI()
        {
            base.AI();
            // face our momentum
            projectile.rotation += 15*Mathj.Matht.Deg2Rad;
            Vector2 tamspeed = projectile.velocity;
            tamspeed.Normalize();
            if (projectile.ai[0] == 0 && (projectile.velocity.X * projectile.velocity.X) + (projectile.velocity.Y * projectile.velocity.Y) > 16f) //tambo slows down over time 
            {
                projectile.velocity -= 0.2f * tamspeed;
            }
            if (projectile.ai[0] == 0 && (projectile.velocity.X * projectile.velocity.X) + (projectile.velocity.Y * projectile.velocity.Y) < 16f) //slow down the slow down for some juicy boss dps 
            {
                projectile.velocity -= 0.05f * tamspeed;
            }
            if (projectile.ai[0] == 1) //tambo return 
            {
                projectile.velocity -= -0.1f * tamspeed;
                projectile.tileCollide = true;
            }
            if (Vector2.Dot(projectile.velocity, tamspeed) < 0) //dot product multiplies 
            {
                projectile.ai[0] = 1;
            }
            for (int i = 0; i < 7; i++) //blur shit wooooo also its 7 because + 1 will be 8 at 7 
            {
                rotations[i] = rotations[i + 1];
                rotpos[i] = rotpos[i + 1];
            }
            rotations[7] = projectile.rotation;
            rotpos[7] = projectile.Center;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            base.PostDraw(spriteBatch, lightColor);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
            for (int i = 0; i < 8; i++)
            {
                AccelerationHelper.DrawSprite("Projectiles/Tambourine_Emiss", rotpos[i], 0, 48, new Color(255 - ((8 - i) * 35), 255 - ((8 - i) * 35), 255 - ((8 - i) * 35)), rotations[i], new Vector2(1,1), spriteBatch);
            }
            spriteBatch.End();
            spriteBatch.Begin();
        }
        public override void Kill(int timeLeft)
        {
            Vector2 position = projectile.position - new Vector2(15, 15);
            for (int i = 0; i < 20; ++i)
            {
                Dust.NewDust(projectile.position, 48, 48, DustID.TopazBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
                Dust.NewDust(projectile.position, 48, 48, DustID.SapphireBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
            }
        }
    }
} 