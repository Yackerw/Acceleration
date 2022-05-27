using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using Mathj;
using Acceleration;

namespace Acceleration.Projectiles.Saki
{
	class SakiOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 48;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 350;
			Projectile.penetrate = 1;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.Name = "Saki Orb";
			Projectile.DamageType = DamageClass.Magic;
			Projectile.frameCounter = 0;
			Projectile.frame = 0;
		}

		public override void AI()
		{
			// target saki until we grow big, then target player for a short while
			if (Projectile.timeLeft > 150)
			{
				Vector2 targetPos;
				if (Projectile.timeLeft > 225)
				{
					// let us randomly be blue
					if (Projectile.frameCounter == 0)
					{
						Projectile.frame = Main.rand.Next(0, 2);
						if (Projectile.frame == 1)
						{
							Projectile.frame = 2;
						}
						// lol this is not how its meant to be used
						Projectile.frameCounter = 1;
					}
					NPC target = Main.npc[(int)Projectile.ai[0]];
					if (!target.active || target.type != ModContent.NPCType<NPCs.Bosses.Saki>())
					{
						return;
					}
					targetPos = target.position;
				} else
				{
					// turn big
					if (Projectile.frame % 2 == 0)
					{
						Projectile.frame += 1;
					} 
					Player target = Main.player[(int)Projectile.ai[1]];
					if (!target.active)
					{
						return;
					}
					targetPos = target.position;
				}
				// simply accelerate towards our target
				targetPos = targetPos - Projectile.position;
				targetPos.Normalize();
				targetPos *= 0.5f;
				Projectile.velocity += targetPos;
				float projMag = Matht.Magnitude(Projectile.velocity);
				if (projMag > 6.0f)
				{
					Projectile.velocity *= 6.0f / projMag;
				}
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return Projectile.timeLeft > 225 ? false : base.Colliding(projHitbox, targetHitbox);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center, Projectile.frame, 64, Color.White, 0, new Vector2(1, 1), Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				if (Projectile.frame < 2)
				{
					Dust.NewDust(Projectile.position, 48, 48, DustID.GemTopaz, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				}
				else
				{
					Dust.NewDust(Projectile.position, 48, 48, DustID.GemSapphire, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				}
			}
		}
	}
}
