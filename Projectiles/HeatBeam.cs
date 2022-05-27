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
using Acceleration.NPCs;

namespace Acceleration.Projectiles
{
	class HeatBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.height = 12;
			Projectile.alpha = 0;
			Projectile.timeLeft = 600;
			Projectile.penetrate = 1;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.light = 0.5f;
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
				Dust.NewDust(Projectile.position, 14, 14, DustID.GemRuby, Projectile.oldVelocity.X, Projectile.oldVelocity.Y);
			}
			base.Kill(timeLeft);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSprite("Acceleration/Projectiles/HeatBeam", Projectile.Center, 0, 16, Color.White, Projectile.rotation, new Vector2(1,1), Main.spriteBatch);
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
