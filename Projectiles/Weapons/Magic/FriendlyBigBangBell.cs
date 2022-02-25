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

namespace Acceleration.Projectiles.Weapons.Magic
{
	class FriendlyBigBangBell : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 280;
			projectile.height = 280;
			//projectile.alpha = 50;
			projectile.timeLeft = 650;
			projectile.penetrate = -1;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.Name = "Big Bang Bell";
			projectile.magic = true;
			projectile.frameCounter = 0;
			projectile.frame = 0;
		}

		public override void AI()
		{
			// rotate a little bit
			projectile.rotation += 1.5f * Matht.Deg2Rad;
			if (projectile.timeLeft > 150)
			{
				if (projectile.timeLeft > 550)
				{
					// increase in scale, then idle for a short while
					projectile.scale = (1.0f - Math.Max((projectile.timeLeft - 600.0f) / 50.0f, 0.0f)) * 2.0f;
					projectile.ai[0] = AccelerationHelper.FindClosestNPC(projectile.Center, 600);
				}
				else
				{
					if (projectile.ai[0] == -1 || !Main.npc[(int)projectile.ai[0]].active)
					{
						projectile.ai[0] = AccelerationHelper.FindClosestNPC(projectile.Center, 600);
						if (projectile.ai[0] == -1)
						{
							return;
						}
					}
					NPC target = Main.npc[(int)projectile.ai[0]];
					if (!target.active)
					{
						return;
					}
					Vector2 targetPos = target.position;
					// simply accelerate towards our target
					targetPos = targetPos - projectile.position;
					targetPos.Normalize();
					targetPos *= 0.15f;
					projectile.velocity += targetPos;
					float projMag = Matht.Magnitude(projectile.velocity);
					if (projMag > 1.25f)
					{
						projectile.velocity *= 4.0f / projMag;
					}
				}
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return projectile.timeLeft > 550 ? false : base.Colliding(projHitbox, targetHitbox);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(Main.projectileTexture[projectile.type], projectile.Center, 0, 160, Color.White, projectile.rotation, new Vector2(projectile.scale, projectile.scale), spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 40; ++i)
			{
				Dust.NewDust(projectile.position, 280, 280, DustID.TopazBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(projectile.position, 280, 280, DustID.SapphireBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}
