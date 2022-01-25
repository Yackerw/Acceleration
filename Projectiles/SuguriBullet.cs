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
	class SuguriBullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.timeLeft = 600;
			projectile.penetrate = 1;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.ranged = true;
		}

		public override void AI()
		{
			base.AI();
			// face our momentum
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
			// dust
			Color drawColor = GetColorDraw();
			Vector2 dustSpeed = projectile.velocity - new Vector2(10, 0).RotatedBy(projectile.rotation);
			Vector2 randomPositioning = new Vector2(Main.rand.NextFloat(-2.999f, 2.999f), Main.rand.NextFloat(-2.999f, 2.999f));
			if (projectile.frame == 1)
			{
				Dust.NewDustPerfect(projectile.position - (0.5f * projectile.velocity), mod.DustType("Bullet_Trail_White"), dustSpeed, 0, drawColor);
			} else
			{
				Dust.NewDustPerfect(projectile.position - (0.5f * projectile.velocity), mod.DustType("Bullet_Trail_White"), dustSpeed, 0, Color.Yellow);
			}
		}

		Color GetColorDraw()
		{
			Color drawColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			projectile.frame = 1;
			switch (projectile.ai[0])
			{

				case ProjectileID.MeteorShot:
					drawColor = new Color(125, 75, 75);
					break;

				case ProjectileID.CrystalBullet:
					drawColor = new Color(133, 140, 178);
					break;

				case ProjectileID.CursedBullet:
					drawColor = new Color(180, 252, 0);
					break;

				case ProjectileID.ChlorophyteBullet:
					drawColor = new Color(148, 196, 67);
					break;

				case ProjectileID.IchorBullet:
					drawColor = new Color(254, 203, 79);
					break;

				case ProjectileID.VenomBullet:
					drawColor = new Color(186, 129, 194);
					break;

				case ProjectileID.NanoBullet:
					drawColor = new Color(0, 255, 255);
					break;

				case ProjectileID.GoldenBullet:
					drawColor = new Color(234, 208, 138);
					break;

				case ProjectileID.MoonlordBullet:
					drawColor = new Color(28, 222, 152);
					break;

				default:
					projectile.frame = 0;
					break;
			}
			return drawColor;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			Color drawColor = GetColorDraw();
			for (int i = 0; i < 10; ++i)
			{
				if (projectile.frame == 0)
				{
					Dust.NewDust(projectile.position, 14, 14, DustID.Amber, projectile.oldVelocity.X, projectile.oldVelocity.Y);
				} else
				{
					// spawn a more colorless dust
					Dust.NewDust(projectile.position, 14, 14, DustID.Web, projectile.oldVelocity.X, projectile.oldVelocity.Y, 0, drawColor);
				}
			}
			base.Kill(timeLeft);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			switch (projectile.ai[0])
			{
				case ProjectileID.VenomBullet:
					target.AddBuff(BuffID.Venom, 600);
					break;
				case ProjectileID.CursedBullet:
					target.AddBuff(BuffID.CursedInferno, 7*60);
					break;
				case ProjectileID.NanoBullet:
					target.AddBuff(BuffID.Confused, Main.rand.Next(60, 180));
					break;
				case ProjectileID.IchorBullet:
					target.AddBuff(BuffID.Ichor, 600);
					break;
			}
			AcceleratePlayer ap = Main.player[projectile.owner].GetModPlayer<AcceleratePlayer>();
			ap.hyper += 0.015f;
			if (ap.hyper > 3.0f)
			{
				ap.hyper = 3.0f;
			}
			base.OnHitNPC(target, damage, knockback, crit);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.ai[0] != ProjectileID.MeteorShot || projectile.penetrate <= 1)
			{
				return base.OnTileCollide(oldVelocity);
			} else
			{
				--projectile.penetrate;
				// mirror our speed...
				if (projectile.velocity.X != oldVelocity.X)
				{
					projectile.velocity.X = -oldVelocity.X;
				} else
				{
					projectile.velocity.Y = -oldVelocity.Y;
				}
				return false;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Color drawColor = GetColorDraw();
			drawColor = drawColor.MultiplyRGB(lightColor);

			Main.spriteBatch.Draw(Main.projectileTexture[projectile.type],
				projectile.Center - Main.screenPosition,
				new Rectangle(0, 12 * projectile.frame, 24, 12),
				drawColor,
				projectile.rotation,
				new Vector2(12, 6),
				1.0f,
				SpriteEffects.None,
				0f);


			return false;
		}
	}
}
