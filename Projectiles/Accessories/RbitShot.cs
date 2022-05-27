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

namespace Acceleration.Projectiles.Accessories
{
	class RbitShot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 250;
			Projectile.penetrate = 3;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.Name = "Saki Pellet";
			Projectile.DamageType = DamageClass.Summon;
			Projectile.frameCounter = 0;
			Projectile.frame = 0;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			AcceleratePlayer ap = Main.player[Projectile.owner].GetModPlayer<AcceleratePlayer>();
			ap.hyper += 0.02f;
			if (ap.hyper > 3.0f)
			{
				ap.hyper = 3.0f;
			}
			target.immune[Projectile.owner] = 10;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center, 0, 32, Color.White, (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X), new Vector2(1, 1), Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(Projectile.position, 32, 32, DustID.GemEmerald, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}
