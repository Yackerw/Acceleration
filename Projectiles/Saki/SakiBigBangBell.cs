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
	class SakiBigBangBell : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 280;
			Projectile.height = 280;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 650;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.Name = "Big Bang Bell";
			Projectile.DamageType = DamageClass.Magic;
			Projectile.frameCounter = 0;
			Projectile.frame = 0;
		}

		public override void AI()
		{
			// rotate a little bit
			Projectile.rotation += 1.5f * Matht.Deg2Rad;
			if (Projectile.timeLeft > 150)
			{
				if (Projectile.timeLeft > 550)
				{
					// increase in scale, then idle for a short while
					Projectile.scale = (1.0f - Math.Max((Projectile.timeLeft - 600.0f) / 50.0f, 0.0f)) * 2.0f;
				}
				else
				{
					Player target = Main.player[(int)Projectile.ai[1]];
					if (!target.active)
					{
						return;
					}
					Vector2 targetPos = target.Center;
					// simply accelerate towards our target
					targetPos = targetPos - Projectile.Center;
					targetPos.Normalize();
					targetPos *= 0.15f;
					Projectile.velocity += targetPos;
					float projMag = Matht.Magnitude(Projectile.velocity);
					if (projMag > 4.0f)
					{
						Projectile.velocity *= 4.0f / projMag;
					}
				}
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return Projectile.timeLeft > 550 ? false : base.Colliding(projHitbox, targetHitbox);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center, 0, 160, Color.White, Projectile.rotation, new Vector2(Projectile.scale, Projectile.scale), Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 40; ++i)
			{
				Dust.NewDust(Projectile.position, 280, 280, DustID.GemTopaz, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(Projectile.position, 280, 280, DustID.GemSapphire, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}
