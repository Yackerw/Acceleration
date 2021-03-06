using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Mathj;
using Acceleration.NPCs;

namespace Acceleration.Projectiles
{
	class ChargeBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 56;
			Projectile.height = 32;
			Projectile.alpha = 0;
			Projectile.timeLeft = 600;
			Projectile.penetrate = 3;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.light = 0.5f;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			AcceleratePlayer ap = Main.player[Projectile.owner].GetModPlayer<AcceleratePlayer>();
			ap.hyper += 0.06f;
			if (ap.hyper > 3.0f)
			{
				ap.hyper = 3.0f;
			}
		}

		public override void AI()
		{
			base.AI();
			// face our momentum
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(Projectile.position, 14, 14, DustID.GemSapphire, Projectile.oldVelocity.X, Projectile.oldVelocity.Y);
			}
			base.Kill(timeLeft);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center, 0, 24, Color.White, Projectile.rotation, new Vector2(1,1), Main.spriteBatch);
			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin();
			base.PostDraw(lightColor);
		}
	}
}
