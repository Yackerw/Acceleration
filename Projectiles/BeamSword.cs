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
using Mathj;

namespace Acceleration.Projectiles
{
    class BeamSword : ModProjectile
    {
        int animTimer;
        Vector2[] blurPos = new Vector2[8];
        float[] blurRot = new float[8];
        float maxExistTime = 30;

        public override void SetDefaults()
        {
            //projectile.width = 48;
            //projectile.height = 10;
            projectile.alpha = 0;
            projectile.timeLeft = 30;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.light = 0.3f;
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
            AcceleratePlayer ap = Main.player[projectile.owner].GetModPlayer<AcceleratePlayer>();
            ap.hyper += 0.05f;
		}
		public override void AI()
        {
            // adjust our existence time
            if (projectile.timeLeft == 30)
			{
                int existTime = (int)projectile.ai[0] >> 2;
                maxExistTime = existTime;
                projectile.timeLeft = existTime;
                projectile.ai[0] -= (existTime << 2);
			}
            // stick to the player
            Vector2 playerPos = Main.player[projectile.owner].Center;
            if (projectile.ai[1] > Math.PI / 2 || projectile.ai[1] < -Math.PI / 2)
			{
                projectile.spriteDirection = -1;
			}
            switch (projectile.ai[0])
			{
                case 0:
                    float sinLerp = Matht.SmootherStep(0, 1, projectile.timeLeft / maxExistTime);
                    // simple smoothstep animation
                    projectile.rotation = Matht.Lerp(80 * Matht.Deg2Rad * projectile.spriteDirection, -20 * Matht.Deg2Rad * projectile.spriteDirection, sinLerp) + projectile.ai[1];
                    break;
                case 1:
                    sinLerp = Matht.SmootherStep(1, 0, projectile.timeLeft / maxExistTime);
                    // simple smoothstep animation
                    projectile.rotation = Matht.Lerp(80 * Matht.Deg2Rad * projectile.spriteDirection, -40 * Matht.Deg2Rad * projectile.spriteDirection, sinLerp) + projectile.ai[1];
                    break;

                case 2:
                    sinLerp = Matht.SmootherStep(1, 0, projectile.timeLeft / maxExistTime);
                    sinLerp = Matht.SmootherStep(1, 0, sinLerp);
                    projectile.rotation = Matht.Lerp(80 * Matht.Deg2Rad * projectile.spriteDirection, -80 * Matht.Deg2Rad * projectile.spriteDirection, sinLerp) + projectile.ai[1];
                    break;

            }
            // adjust the base position based on our rotation so it matches our swing
            Vector2 rotPos = new Vector2(17, -17).RotatedBy(projectile.rotation + (projectile.spriteDirection == -1 ? Math.PI / 2 : 0));
            // and one more offset to follow the swing
            projectile.position = playerPos + rotPos + new Vector2(17, 17/*Matht.Lerp(40 * projectile.spriteDirection, 0, sinLerp)*/).RotatedBy(projectile.rotation + (projectile.spriteDirection == -1 ? 0 : (float)-Math.PI / 2));
            ++animTimer;
            // set up our motion blur
            for (int i = blurPos.Length - 2; i >= 0; --i)
			{
                blurPos[i + 1] = blurPos[i];
                blurRot[i + 1] = blurRot[i];
			}
            blurPos[0] = projectile.position - Main.player[projectile.owner].Center;
            blurRot[0] = projectile.rotation;
            // create hurty projectiles for our swing
            Vector2 spawnPos = projectile.position;
            Vector2 spawnNorm = new Vector2(11, 0).RotatedBy(projectile.rotation + (projectile.spriteDirection == -1 ? (float)Math.PI / 4 : (float)-Math.PI / 4));
            spawnPos -= spawnNorm * 2.0f;
            float hitRot = projectile.rotation + (projectile.spriteDirection == -1 ? -(float)Math.PI / 4 : (float)Math.PI / 4);
            for (int i = 0; i < 5; ++i)
			{
                Projectile.NewProjectile(spawnPos, new Vector2(0, 0), ModContent.ProjectileType<SwordHitbox>(), projectile.damage, projectile.knockBack, projectile.owner, hitRot);
                spawnPos += spawnNorm;
			}
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            AccelerationHelper.DrawSpriteCached(Main.projectileTexture[projectile.type], projectile.position, 0, 48, lightColor, projectile.rotation, new Vector2(1,projectile.spriteDirection), spriteBatch);
            return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            // draw emissive
            AccelerationHelper.DrawSprite("Items/Weapons/Melee/BeamSword_Emiss", projectile.position, 0, 48, Color.White, projectile.rotation, new Vector2(1, projectile.spriteDirection), spriteBatch);
            // draw motion blur
            Vector2 ownerPos = Main.player[projectile.owner].Center;
            Texture2D tex;
            tex = Acceleration.thisMod.GetTexture("Items/Weapons/Melee/BeamSword_Blur");
            Color drawColor = new Color(255, 255, 255);
            int iteratorSize = blurPos.Length > (maxExistTime - projectile.timeLeft) ? (int)maxExistTime - projectile.timeLeft : blurPos.Length;
            for (int i = 1; i < iteratorSize; ++i)
			{
                AccelerationHelper.DrawSpriteCached(tex, blurPos[i] + ownerPos, 0, 48, drawColor, blurRot[i], new Vector2(1, projectile.spriteDirection), spriteBatch);
                drawColor.R -= 35;
                drawColor.G -= 35;
                drawColor.B -= 35;
			}
            spriteBatch.End();
            spriteBatch.Begin();
        }
	}
}