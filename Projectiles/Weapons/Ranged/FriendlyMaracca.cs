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

namespace Acceleration.Projectiles.Weapons.Ranged
{
	class FriendlyMaracca : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			projectile.width = 64;
			projectile.height = 32;
			//projectile.alpha = 50;
			projectile.timeLeft = 600;
			projectile.penetrate = 1;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.Name = "Maracca";
			projectile.aiStyle = -1;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			base.OnHitNPC(target, damage, knockback, crit);
			AcceleratePlayer ap = Main.player[projectile.owner].GetModPlayer<AcceleratePlayer>();
			ap.hyper += 0.035f;
			if (ap.hyper > 3.0f)
			{
				ap.hyper = 3.0f;
			}
		}

		public override void AI()
		{
			// just apply light gravity
			projectile.velocity.Y += 0.12f;
			projectile.velocity.Y = Math.Min(projectile.velocity.Y, 8.0f);
			if (projectile.timeLeft % 6 == 0)
			{
				if (projectile.velocity.X > 0)
				{
					projectile.frame = (projectile.frame + 1) % 6;
				}
				else
				{
					projectile.frame = (projectile.frame - 1) % 6;
					if (projectile.frame < 0)
					{
						projectile.frame = 5;
					}
				}
			}
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			base.PostDraw(spriteBatch, lightColor);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
			AccelerationHelper.DrawSprite("Projectiles/Saki/Maracca_Emiss", projectile.Center, projectile.frame, 32, Color.White, 0, new Vector2(1, 1), spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin();
		}
		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, 64, 32, DustID.TopazBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Gore.NewGore(projectile.position, Vector2.Zero, 61);
			}
			Main.PlaySound(SoundID.Item14, projectile.position);
		}
	}
}
