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

namespace Acceleration.Projectiles.Weapons.Ranged
{
	class FriendlyMaracca : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.Name = "Maracca";
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = -1;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			base.OnHitNPC(target, damage, knockback, crit);
			AcceleratePlayer ap = Main.player[Projectile.owner].GetModPlayer<AcceleratePlayer>();
			ap.hyper += 0.035f;
			if (ap.hyper > 3.0f)
			{
				ap.hyper = 3.0f;
			}
			if (Projectile.timeLeft > 4)
			{
				Projectile.timeLeft = 4;
			}
		}

		public override void AI()
		{
			if (Projectile.timeLeft > 3)
			{
				// just apply light gravity
				Projectile.velocity.Y += 0.25f;
				Projectile.velocity.Y = Math.Min(Projectile.velocity.Y, 7.0f);
				if (Projectile.timeLeft % 6 == 0)
				{
					if (Projectile.ai[0] == 0)
					{
						Projectile.frame = (Projectile.frame + 1) % 6;
					}
					else
					{
						Projectile.frame = (Projectile.frame - 1) % 6;
						if (Projectile.frame < 0)
						{
							Projectile.frame = 5;
						}
					}
				}
			}
			else if (Projectile.owner == Main.myPlayer)
			{
				// become BOM
				Projectile.tileCollide = false;
				Projectile.alpha = 255;
				Projectile.position = Projectile.Center;
				Projectile.width = 128;
				Projectile.height = 128;
				Projectile.Center = Projectile.position;
			}
			if (Projectile.timeLeft == 3)
			{
				for (int i = 0; i < 10; ++i)
				{
					Dust.NewDust(Projectile.Center, 64, 32, DustID.GemTopaz, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
					Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 61);
				}
				SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			}
		}

		public override void PostDraw(Color lightColor)
		{
			base.PostDraw(lightColor);
			if (Projectile.timeLeft > 3)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
				AccelerationHelper.DrawSprite("Acceleration/Projectiles/Saki/Maracca_Emiss", Projectile.Center, Projectile.frame, 32, Color.White, 0, new Vector2(1, 1), Main.spriteBatch);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin();
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.timeLeft > 4)
			{
				Projectile.timeLeft = 4;
			}
			return false;
		}
	}
}
