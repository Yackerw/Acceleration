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
using Acceleration;
using Terraria.GameContent;
using Acceleration.Items.Weapons.Melee;

namespace Acceleration.Projectiles
{
    class BeamSwordProjectile : ModProjectile
    {
        int animTimer;
        Vector2[] blurPos = new Vector2[8];
        float[] blurRot = new float[8];
        float maxExistTime = 30;

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.alpha = 0;
            Projectile.timeLeft = 30;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.light = 0.3f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            AcceleratePlayer ap = Main.player[Projectile.owner].GetModPlayer<AcceleratePlayer>();
            ap.hyper += 0.05f;
            if (ap.hyper > 3.0f)
            {
                ap.hyper = 3.0f;
            }
            // grant some invuln frames
            if (ap.Player.HeldItem.type == ModContent.ItemType<BeamSword>())
            {
                BeamSword bm = (BeamSword)ap.Player.HeldItem.ModItem;
                if (bm.swingInvulnDelay <= 0)
                {
                    ap.Player.immune = true;
                    ap.Player.immuneTime = 90;
                    for (int i = 0; i < ap.Player.hurtCooldowns.Length; ++i)
                    {
                        ap.Player.hurtCooldowns[i] = 90;
                    }
                    // put a delay
                    bm.swingInvulnDelay = 300;
                }
            }
        }

        public override void AI()
        {
            // adjust our existence time
            if (Projectile.timeLeft == 30)
			{
                int existTime = (int)Projectile.ai[0] >> 2;
                maxExistTime = existTime;
                Projectile.timeLeft = existTime;
                Projectile.ai[0] -= (existTime << 2);
			}
            // stick to the player
            Vector2 playerPos = Main.player[Projectile.owner].Center;
            if (Projectile.ai[1] > Math.PI / 2 || Projectile.ai[1] < -Math.PI / 2)
			{
                Projectile.spriteDirection = -1;
			}
            switch (Projectile.ai[0])
			{
                case 0:
                    float sinLerp = Matht.SmootherStep(0, 1, Projectile.timeLeft / maxExistTime);
                    // simple smoothstep animation
                    Projectile.rotation = Matht.Lerp(80 * Matht.Deg2Rad * Projectile.spriteDirection, -20 * Matht.Deg2Rad * Projectile.spriteDirection, sinLerp) + Projectile.ai[1];
                    break;
                case 1:
                    sinLerp = Matht.SmootherStep(1, 0, Projectile.timeLeft / maxExistTime);
                    // simple smoothstep animation
                    Projectile.rotation = Matht.Lerp(80 * Matht.Deg2Rad * Projectile.spriteDirection, -40 * Matht.Deg2Rad * Projectile.spriteDirection, sinLerp) + Projectile.ai[1];
                    break;

                case 2:
                    sinLerp = Matht.SmootherStep(1, 0, Projectile.timeLeft / maxExistTime);
                    sinLerp = Matht.SmootherStep(1, 0, sinLerp);
                    Projectile.rotation = Matht.Lerp(80 * Matht.Deg2Rad * Projectile.spriteDirection, -80 * Matht.Deg2Rad * Projectile.spriteDirection, sinLerp) + Projectile.ai[1];
                    break;

            }
            // adjust the base position based on our rotation so it matches our swing
            Vector2 rotPos = new Vector2(17, -17).RotatedBy(Projectile.rotation + (Projectile.spriteDirection == -1 ? Math.PI / 2 : 0));
            // and one more offset to follow the swing
            Projectile.position = playerPos + rotPos + new Vector2(17, 17/*Matht.Lerp(40 * Projectile.spriteDirection, 0, sinLerp)*/).RotatedBy(Projectile.rotation + (Projectile.spriteDirection == -1 ? 0 : (float)-Math.PI / 2));
            ++animTimer;
            // set up our motion blur
            for (int i = blurPos.Length - 2; i >= 0; --i)
			{
                blurPos[i + 1] = blurPos[i];
                blurRot[i + 1] = blurRot[i];
			}
            blurPos[0] = Projectile.position - Main.player[Projectile.owner].Center;
            blurRot[0] = Projectile.rotation;
            // create hurty Projectiles for our swing
            Vector2 spawnPos = Projectile.position;
            Vector2 spawnNorm = new Vector2(11, 0).RotatedBy(Projectile.rotation + (Projectile.spriteDirection == -1 ? (float)Math.PI / 4 : (float)-Math.PI / 4));
            spawnPos -= spawnNorm * 2.0f;
            float hitRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? -(float)Math.PI / 4 : (float)Math.PI / 4);
            for (int i = 0; i < 5; ++i)
			{
                Item aa = Main.player[Projectile.owner].HeldItem;
				Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_ItemUse(aa), spawnPos, new Vector2(2f * Projectile.spriteDirection, 0), ModContent.ProjectileType<SwordHitbox>(), Projectile.damage, Projectile.knockBack, Projectile.owner, hitRot, 1);
                spawnPos += spawnNorm;
			}
        }

		public override bool PreDraw(ref Color lightColor)
		{
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            AccelerationHelper.DrawSpriteCached(TextureAssets.Projectile[Projectile.type].Value, Projectile.position, 0, 48, lightColor, Projectile.rotation, new Vector2(1,Projectile.spriteDirection), Main.spriteBatch);
            return false;
		}

		public override void PostDraw(Color lightColor)
		{
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            // draw emissive
            AccelerationHelper.DrawSprite("Acceleration/Items/Weapons/Melee/BeamSword_Emiss", Projectile.position, 0, 48, Color.White, Projectile.rotation, new Vector2(1, Projectile.spriteDirection), Main.spriteBatch);
            // draw motion blur
            Vector2 ownerPos = Main.player[Projectile.owner].Center;
            Texture2D tex;
            tex = ModContent.Request<Texture2D>("Acceleration/Items/Weapons/Melee/BeamSword_Blur").Value;
            Color drawColor = new Color(255, 255, 255);
            int iteratorSize = blurPos.Length > (maxExistTime - Projectile.timeLeft) ? (int)maxExistTime - Projectile.timeLeft : blurPos.Length;
            for (int i = 1; i < iteratorSize; ++i)
			{
                AccelerationHelper.DrawSpriteCached(tex, blurPos[i] + ownerPos, 0, 48, drawColor, blurRot[i], new Vector2(1, Projectile.spriteDirection), Main.spriteBatch);
                drawColor.R -= 35;
                drawColor.G -= 35;
                drawColor.B -= 35;
			}
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
        }
	}
}