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
	class SakiPellet : ModProjectile
	{
		bool rotSetup = false;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 250;
			Projectile.penetrate = 1;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.Name = "Saki Pellet";
			Projectile.DamageType = DamageClass.Magic;
			Projectile.frameCounter = 0;
			Projectile.frame = 0;
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			// set it to blue if it should be
			if (Projectile.ai[1] > 0)
			{
				Projectile.frame = 1;
			}
			if (!rotSetup)
			{
				Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
				rotSetup = true;
			}
			// go slowly at first
			if (Projectile.timeLeft > 200) {
				Projectile.velocity = new Vector2(0, 2).RotatedBy(Projectile.rotation);
			} else if (Projectile.timeLeft > 190)
			{
				// rotate a tiny bit and add speed
				if (Projectile.ai[1] == 1)
				{
					Projectile.rotation -= 1 * Matht.Deg2Rad;
				} else
				{
					Projectile.rotation += 1 * Matht.Deg2Rad;
				}
				Projectile.velocity = new Vector2(0, MathHelper.Lerp(7, 2, (Projectile.timeLeft - 190) / 10.0f));
			} else
			{
				// speed
				Projectile.velocity = new Vector2(0, 7).RotatedBy(Projectile.rotation);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center, Projectile.frame, 16, Color.White, Projectile.rotation + (90 * Matht.Deg2Rad), new Vector2(1, 1), Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 10; ++i)
			{
				if (Projectile.frame < 1)
				{
					Dust.NewDust(Projectile.position, 32, 32, DustID.GemTopaz, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				}
				else
				{
					Dust.NewDust(Projectile.position, 32, 32, DustID.GemSapphire, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				}
			}
		}
	}
}
