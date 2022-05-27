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
using Terraria.Audio;
using Mathj;
using Acceleration.NPCs;

namespace Acceleration.Projectiles.Saki
{
	class SakiMaracca : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 32;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 600;
			Projectile.penetrate = 1;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.Name = "Maracca";
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			// just apply light gravity
			Projectile.velocity.Y += 0.25f;
			Projectile.velocity.Y = Math.Min(Projectile.velocity.Y, 7.0f);
			if (Projectile.timeLeft % 6 == 0)
			{
				if (Projectile.ai[0] == 0)
				{
					Projectile.frame = (Projectile.frame + 1) % 6;
				} else
				{
					Projectile.frame = (Projectile.frame - 1) % 6;
					if (Projectile.frame < 0)
					{
						Projectile.frame = 5;
					}
				}
			}
		}

		public override void PostDraw(Color lightColor)
		{
			base.PostDraw(lightColor);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
			AccelerationHelper.DrawSprite("Acceleration/Projectiles/Saki/Maracca_Emiss", Projectile.Center, Projectile.frame, 32, Color.White, 0, new Vector2(1, 1), Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin();
		}
		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(Projectile.position, 64, 32, DustID.GemTopaz, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, 61);
			}
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
		}
	}
}
