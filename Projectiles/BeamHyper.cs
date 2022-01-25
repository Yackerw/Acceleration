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
	class BeamHyper : ModProjectile
	{

		int fluctuationTimer = 0;

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = 40;
			projectile.height = 40;
			projectile.alpha = 50;
			projectile.timeLeft = 300;
			projectile.penetrate = -1;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.magic = true;
			projectile.light = 0.5f;
		}

		public override void AI()
		{
			fluctuationTimer += 1;
			// packed normal
			Vector2 normal = new Vector2(1, 0).RotatedBy(projectile.ai[1]);
			if (projectile.ai[0] == 0)
			{
				// ai[0] 0 is the only one to exist for a while
				if (projectile.timeLeft < 20)
				{
					projectile.alpha = (int)Matht.Lerp(255.0f, 50.0f, projectile.timeLeft / 20.0f);
				}
				Vector2 spawnPos = projectile.Center;
				// draw a good number of projectiles
				int projToSpawn = ModContent.ProjectileType<BeamHyper>();
				Vector2 spawnIncrement = normal * 40;
				for (int i = 0; i < 40; ++i)
				{
					Vector2 moveAmnt = Collision.TileCollision(spawnPos, spawnIncrement, 40, 40, false, false, 0);
					if (moveAmnt.X != spawnIncrement.X || moveAmnt.Y != spawnIncrement.Y)
					{
						spawnPos += moveAmnt;
						break;
					}
					spawnPos += spawnIncrement;
					Projectile proj = Main.projectile[Projectile.NewProjectile(spawnPos, new Vector2(0, 0), projToSpawn, projectile.damage, projectile.knockBack, projectile.owner, 2 + ((fluctuationTimer + (i * 2)) % 4 <= 1 ? 1 : 0), projectile.ai[1])];
					proj.alpha = projectile.alpha;
					// hack for ordering...
					if (proj.whoAmI < projectile.whoAmI)
					{
						proj.timeLeft = 1;
					}
				}
				Projectile explodeProj = Main.projectile[Projectile.NewProjectile(spawnPos, new Vector2(0, 0), projToSpawn, projectile.damage, projectile.knockBack, projectile.owner, 4 + ((fluctuationTimer) % 4 <= 1 ? 1 : 0), fluctuationTimer)];
				if (explodeProj.whoAmI < projectile.whoAmI)
				{
					explodeProj.timeLeft = 1;
				}
				projectile.frame = (fluctuationTimer % 4) <= 1 ? 0 : 1;
			} else
			{
				if (projectile.timeLeft > 2)
				{
					projectile.timeLeft = 2;
				}
				if (projectile.ai[0] >= 4)
				{
					projectile.alpha = 0;
				}
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 10;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.ai[0] < 4)
			{
				projectile.rotation = projectile.ai[1];
			}
			projectile.frame = (int)projectile.ai[0];
			if (projectile.ai[0] >= 4)
			{
				if (projectile.ai[1] < 280)
				{
					projectile.scale = 4.5f;
				} else
				{
					projectile.scale = Matht.Lerp(4.5f, 2, (projectile.ai[1] - 280.0f) / 20.0f);
				}
				projectile.scale += (projectile.ai[1] % 5) * 0.05f;
				projectile.alpha = 0;
			}
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
			if (projectile.ai[0] < 4)
			{
				AccelerationHelper.DrawSpriteCached(Main.projectileTexture[projectile.type], projectile.Center, (int)projectile.ai[0], 40, new Color(0, 128, 255, 255 - projectile.alpha), projectile.rotation, new Vector2(projectile.scale, projectile.scale), spriteBatch);
			} else
			{
				AccelerationHelper.DrawSprite("Projectiles/beam_impact", projectile.Center, 0, 64, new Color(0, 128, 255, 255), projectile.rotation, new Vector2(projectile.scale, projectile.scale), spriteBatch);
				AccelerationHelper.DrawSprite("Projectiles/beam_impact", projectile.Center, 0, 64, new Color(0, 128, 128, 255), projectile.rotation, new Vector2(projectile.scale, projectile.scale) * 0.75f, spriteBatch);
			}
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
