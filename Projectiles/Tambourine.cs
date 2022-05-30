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
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.alpha = 0;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.light = 0.3f;
        }
        public override void AI()
        {
            base.AI();
            // face our momentum
            Projectile.rotation += 15*Mathj.Matht.Deg2Rad;
            Vector2 tamspeed = Projectile.velocity;
            tamspeed.Normalize();
            if (Projectile.ai[0] == 0 && (Projectile.velocity.X * Projectile.velocity.X) + (Projectile.velocity.Y * Projectile.velocity.Y) > 16f) //tambo slows down over time 
            {
                Projectile.velocity -= 0.2f * tamspeed;
            }
            if (Projectile.ai[0] == 0 && (Projectile.velocity.X * Projectile.velocity.X) + (Projectile.velocity.Y * Projectile.velocity.Y) < 16f) //slow down the slow down for some juicy boss dps 
            {
                Projectile.velocity -= 0.05f * tamspeed;
            }
            if (Projectile.ai[0] == 1) //tambo return 
            {
                Projectile.velocity -= -0.1f * tamspeed;
                Projectile.tileCollide = true;
            }
            if (Vector2.Dot(Projectile.velocity, tamspeed) < 0) //dot product multiplies 
            {
                Projectile.ai[0] = 1;
            }
            for (int i = 0; i < 7; i++) //blur shit wooooo also its 7 because + 1 will be 8 at 7 
            {
                rotations[i] = rotations[i + 1];
                rotpos[i] = rotpos[i + 1];
            }
            rotations[7] = Projectile.rotation;
            rotpos[7] = Projectile.Center;
        }
        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
            for (int i = 0; i < 8; i++)
            {
                AccelerationHelper.DrawSprite("Acceleration/Projectiles/Saki/Tambourine_Emiss", rotpos[i], 0, 48, new Color(255 - ((8 - i) * 35), 255 - ((8 - i) * 35), 255 - ((8 - i) * 35)), rotations[i], new Vector2(1,1), Main.spriteBatch);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
        }
        public override void Kill(int timeLeft)
        {
            Vector2 position = Projectile.position - new Vector2(15, 15);
            for (int i = 0; i < 20; ++i)
            {
                Dust.NewDust(Projectile.position, 48, 48, DustID.GemTopaz, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
                Dust.NewDust(Projectile.position, 48, 48, DustID.GemSapphire, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
            }
        }
    }
} 