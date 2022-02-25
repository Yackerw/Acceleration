﻿using System;
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
using Acceleration;

namespace Acceleration.Projectiles.Saki
{
	class SakiPellet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 26;
			//projectile.alpha = 50;
			projectile.timeLeft = 250;
			projectile.penetrate = 1;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.Name = "Saki Pellet";
			projectile.magic = true;
			projectile.frameCounter = 0;
			projectile.frame = 0;
			projectile.aiStyle = -1;
		}

		public override void AI()
		{
			// set it to blue if it should be
			if (projectile.ai[1] > 0)
			{
				projectile.frame = 1;
			}
			// go slowly at first
			if (projectile.timeLeft > 200) {
				projectile.velocity = new Vector2(0, 2).RotatedBy(projectile.rotation);
			} else if (projectile.timeLeft > 190)
			{
				// rotate a tiny bit and add speed
				if (projectile.ai[1] == 1)
				{
					projectile.rotation -= 1 * Matht.Deg2Rad;
				} else
				{
					projectile.rotation += 1 * Matht.Deg2Rad;
				}
				projectile.velocity = new Vector2(0, MathHelper.Lerp(7, 2, (projectile.timeLeft - 190) / 10.0f));
			} else
			{
				// speed
				projectile.velocity = new Vector2(0, 7).RotatedBy(projectile.rotation);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			AccelerationHelper.DrawSpriteCached(Main.projectileTexture[projectile.type], projectile.Center, projectile.frame, 32, Color.White, projectile.rotation + (90 * Matht.Deg2Rad), new Vector2(1, 1), spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 10; ++i)
			{
				if (projectile.frame < 1)
				{
					Dust.NewDust(projectile.position, 32, 32, DustID.TopazBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				}
				else
				{
					Dust.NewDust(projectile.position, 32, 32, DustID.SapphireBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				}
			}
		}
	}
}
