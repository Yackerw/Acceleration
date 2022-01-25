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
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 38;
			projectile.height = 12;
			projectile.alpha = 0;
			projectile.timeLeft = 600;
			projectile.penetrate = 1;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.magic = true;
			projectile.light = 0.5f;
		}

		public override void AI()
		{
			base.AI();
			// face our momentum
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(projectile.position, 14, 14, DustID.RubyBolt, projectile.oldVelocity.X, projectile.oldVelocity.Y);
			}
			base.Kill(timeLeft);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSprite("Projectiles/HeatBeam", projectile.Center, 0, 16, Color.White, projectile.rotation, new Vector2(1,1), spriteBatch);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin();
			base.PostDraw(spriteBatch, lightColor);
		}
	}
}
