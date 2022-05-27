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
using Acceleration.Dusts;
using Terraria.GameContent;

namespace Acceleration.Projectiles
{
	class SuguriBullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 20;
		}
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.timeLeft = 600;
			Projectile.penetrate = 1;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Ranged;
		}

		public override void AI()
		{
			base.AI();
			// face our momentum
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			// dust
			Color drawColor = GetColorDraw();
			Vector2 dustSpeed = Projectile.velocity - new Vector2(10, 0).RotatedBy(Projectile.rotation);
			Dust.NewDustPerfect(Projectile.Center - (0.5f * Projectile.velocity), ModContent.DustType<Bullet_Trail_White>(), dustSpeed, 0, drawColor);
		}

		Color GetColorDraw()
		{
			Color drawColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			switch (Projectile.ai[0])
			{

				case ProjectileID.MeteorShot:
					drawColor = new Color(0xF0, 0, 0x38);
					Projectile.frame = 1;
					break;

				case ProjectileID.CrystalBullet:
					drawColor = new Color(0, 0xA0, 0xE0);
					Projectile.frame = 3;
					break;

				case ProjectileID.CursedBullet:
					drawColor = new Color(0xC0, 255, 0);
					Projectile.frame = 4;
					break;

				case ProjectileID.ChlorophyteBullet:
					drawColor = new Color(0x40, 0xA0, 0);
					Projectile.frame = 5;
					break;

				case ProjectileID.BulletHighVelocity:
					drawColor = new Color(0xFF, 0xE0, 0x40);
					Projectile.frame = 6;
					break;

				case ProjectileID.IchorBullet:
					drawColor = new Color(255, 255, 0x80);
					Projectile.frame = 7;
					break;

				case ProjectileID.VenomBullet:
					drawColor = new Color(0xB8, 0x80, 0xC0);
					Projectile.frame = 8;
					break;

				case ProjectileID.PartyBullet:
					drawColor = new Color(0xFF, 0x80, 0xFF);
					Projectile.frame = 9;
					break;

				case ProjectileID.NanoBullet:
					drawColor = new Color(0, 255, 255);
					Projectile.frame = 10;
					break;

				case ProjectileID.ExplosiveBullet:
					drawColor = new Color(0xFF, 0, 0);
					Projectile.frame = 11;
					break;

				case ProjectileID.GoldenBullet:
					drawColor = new Color(0xD0, 0xA0, 00);
					Projectile.frame = 12;
					break;

				case ProjectileID.MoonlordBullet:
					drawColor = new Color(0xC0, 255, 0xE0);
					Projectile.frame = 13;
					break;

					// TODO: nanako bullet, heat bullet

				default:
					Projectile.frame = 0;
					drawColor = new Color(0xC0, 0, 0);
					break;
			}
			return drawColor;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			Color drawColor = GetColorDraw();
			for (int i = 0; i < 10; ++i)
			{
				if (Projectile.frame == 0)
				{
					Dust.NewDust(Projectile.position, 14, 14, DustID.GemAmber, Projectile.oldVelocity.X, Projectile.oldVelocity.Y);
				} else
				{
					// spawn a more colorless dust
					Dust.NewDust(Projectile.position, 14, 14, DustID.Web, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 0, drawColor);
				}
			}
			base.Kill(timeLeft);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			switch (Projectile.ai[0])
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
			AcceleratePlayer ap = Main.player[Projectile.owner].GetModPlayer<AcceleratePlayer>();
			ap.hyper += 0.015f;
			if (ap.hyper > 3.0f)
			{
				ap.hyper = 3.0f;
			}
			base.OnHitNPC(target, damage, knockback, crit);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.ai[0] != ProjectileID.MeteorShot || Projectile.penetrate <= 1)
			{
				return base.OnTileCollide(oldVelocity);
			} else
			{
				--Projectile.penetrate;
				// mirror our speed...
				if (Projectile.velocity.X != oldVelocity.X)
				{
					Projectile.velocity.X = -oldVelocity.X;
				} else
				{
					Projectile.velocity.Y = -oldVelocity.Y;
				}
				return false;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value,
				Projectile.Center - Main.screenPosition,
				new Rectangle(0, 16 * Projectile.frame, 32, 16),
				lightColor,
				Projectile.rotation,
				new Vector2(12, 6),
				1.0f,
				SpriteEffects.None,
				0f);


			return false;
		}
	}
}
