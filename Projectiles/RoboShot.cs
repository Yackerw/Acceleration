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
	class RoboShot : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.light = 0.6f;
		}

		public override void AI()
		{
			base.AI();
			if (Main.rand.NextFloat() < 0.15)
			{
				Dust dust;
				// You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
				Vector2 position = Projectile.position - new Vector2(15, 15);
				dust = Main.dust[Terraria.Dust.NewDust(position, 30, 30, 43, 0f, 0f, 0, new Color(255, 255, 0), 0.4069768f)];
			}

		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				Dust dust = Main.dust[Terraria.Dust.NewDust(position, 30, 30, 43, 0f, 0f, 0, new Color(255, 255, 0), 0.4069768f)];
			}
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return true;
		}

		public override void PostDraw(Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin();
			base.PostDraw(lightColor);
		}
	}
}