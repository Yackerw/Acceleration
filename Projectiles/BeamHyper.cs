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
using Acceleration.NPCs;

namespace Acceleration.Projectiles
{
	class BeamHyper : ModProjectile
	{

		int fluctuationTimer = 0;

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			Main.projFrames[Projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.alpha = 50;
			Projectile.timeLeft = 300;
			Projectile.penetrate = -1;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.light = 0.5f;
		}

		public override void AI()
		{
			fluctuationTimer += 1;
			// packed normal
			Vector2 normal = new Vector2(1, 0).RotatedBy(Projectile.ai[1]);
			if (Projectile.ai[0] == 0)
			{

				// ai[0] 0 is the only one to exist for a while
			} else
			{
				if (Projectile.ai[0] >= 4)
				{
					Projectile.alpha = 0;
				}
			}
			if (Projectile.timeLeft < 20 && Projectile.ai[0] < 4)
			{
				Projectile.alpha = (int)Matht.Lerp(255.0f, 50.0f, Projectile.timeLeft / 20.0f);
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[Projectile.owner] = 10;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (Projectile.ai[0] < 4)
			{
				Projectile.rotation = Projectile.ai[1];
			}
			// set up scale for explodey
			if (Projectile.ai[0] >= 4)
			{
				if (fluctuationTimer < 280)
				{
					Projectile.scale = 4.5f;
				} else
				{
					Projectile.scale = Matht.Lerp(4.5f, 2, (fluctuationTimer - 280.0f) / 20.0f);
				}
				Projectile.scale += (fluctuationTimer % 5) * 0.05f;
				Projectile.alpha = 0;
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
			// render main sprite
			if (Projectile.ai[0] < 4)
			{
				int sprite = (int)Projectile.ai[0];
				Projectile.frame = (fluctuationTimer % 4) <= 1 ? 0 : 1;
				if (Projectile.ai[0] == 3)
				{
					sprite -= Projectile.frame;
				} else
				{
					sprite += Projectile.frame;
				}
				AccelerationHelper.DrawSpriteCached(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center, sprite, 40, new Color(0, 128, 255, 255 - Projectile.alpha), Projectile.rotation, new Vector2(Projectile.scale, Projectile.scale), Main.spriteBatch);
			} else
			{
				// explodey sprite
				AccelerationHelper.DrawSprite("Acceleration/Projectiles/beam_impact", Projectile.Center, 0, 64, new Color(0, 128, 255, 255), Projectile.rotation, new Vector2(Projectile.scale, Projectile.scale), Main.spriteBatch);
				AccelerationHelper.DrawSprite("Acceleration/Projectiles/beam_impact", Projectile.Center, 0, 64, new Color(0, 128, 128, 255), Projectile.rotation, new Vector2(Projectile.scale, Projectile.scale) * 0.75f, Main.spriteBatch);
			}
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
