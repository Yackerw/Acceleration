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
using Mathj;
using Acceleration;

namespace Acceleration.Projectiles.Saki
{
	class SakiOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = 48;
			projectile.height = 48;
			//projectile.alpha = 50;
			projectile.timeLeft = 350;
			projectile.penetrate = 1;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.Name = "Saki Orb";
			projectile.magic = true;
			projectile.frameCounter = 0;
			projectile.frame = 0;
		}

		public override void AI()
		{
			// target saki until we grow big, then target player for a short while
			if (projectile.timeLeft > 150)
			{
				Vector2 targetPos;
				if (projectile.timeLeft > 225)
				{
					// let us randomly be blue
					if (projectile.frameCounter == 0)
					{
						projectile.frame = Main.rand.Next(0, 2);
						if (projectile.frame == 1)
						{
							projectile.frame = 2;
						}
						// lol this is not how its meant to be used
						projectile.frameCounter = 1;
					}
					NPC target = Main.npc[(int)projectile.ai[0]];
					if (!target.active || target.type != ModContent.NPCType<NPCs.Bosses.Saki>())
					{
						return;
					}
					targetPos = target.position;
				} else
				{
					// turn big
					if (projectile.frame % 2 == 0)
					{
						projectile.frame += 1;
					} 
					Player target = Main.player[(int)projectile.ai[1]];
					if (!target.active)
					{
						return;
					}
					targetPos = target.position;
				}
				// simply accelerate towards our target
				targetPos = targetPos - projectile.position;
				targetPos.Normalize();
				targetPos *= 0.5f;
				projectile.velocity += targetPos;
				float projMag = Matht.Magnitude(projectile.velocity);
				if (projMag > 6.0f)
				{
					projectile.velocity *= 6.0f / projMag;
				}
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return projectile.timeLeft > 225 ? false : base.Colliding(projHitbox, targetHitbox);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(Main.projectileTexture[projectile.type], projectile.Center, projectile.frame, 64, Color.White, 0, new Vector2(1, 1), spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				if (projectile.frame < 2)
				{
					Dust.NewDust(projectile.position, 48, 48, DustID.TopazBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				}
				else
				{
					Dust.NewDust(projectile.position, 48, 48, DustID.SapphireBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				}
			}
		}
	}
}
