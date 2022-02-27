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

namespace Acceleration.Projectiles.Accessories
{
	class RbitShot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 26;
			//projectile.alpha = 50;
			projectile.timeLeft = 250;
			projectile.penetrate = 3;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.Name = "Saki Pellet";
			projectile.magic = true;
			projectile.frameCounter = 0;
			projectile.frame = 0;
			projectile.aiStyle = -1;
			projectile.friendly = true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			AcceleratePlayer ap = Main.player[projectile.owner].GetModPlayer<AcceleratePlayer>();
			ap.hyper += 0.02f;
			if (ap.hyper > 3.0f)
			{
				ap.hyper = 3.0f;
			}
			target.immune[projectile.owner] = 10;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(Main.projectileTexture[projectile.type], projectile.Center, 0, 32, Color.White, (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X), new Vector2(1, 1), spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, 32, 32, DustID.EmeraldBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}
