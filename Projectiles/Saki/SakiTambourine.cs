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

namespace Acceleration.Projectiles.Saki
{
	class SakiTambourine : ModProjectile
	{
		Vector2[] rotpos = new Vector2[8];
		float[] rotations = new float[8];

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 48;
			projectile.height = 48;
			//projectile.alpha = 50;
			projectile.timeLeft = 600;
			projectile.penetrate = -1;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.Name = "Tambourine";
		}

		public override void AI()
		{
			// simply rotate a bit
			projectile.rotation += 3 * Matht.Deg2Rad;

			for (int i = 0; i < 7; i++) //blur shit wooooo also its 7 because + 1 will be 8 at 7 
			{
				rotations[i] = rotations[i + 1];
				rotpos[i] = rotpos[i + 1];
			}
			rotations[7] = projectile.rotation;
			rotpos[7] = projectile.Center;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			base.PostDraw(spriteBatch, lightColor);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
			for (int i = 0; i < 8; i++)
			{
				AccelerationHelper.DrawSprite("Projectiles/Tambourine_Emiss", rotpos[i], 0, 48, new Color(255 - ((8 - i) * 35), 255 - ((8 - i) * 35), 255 - ((8 - i) * 35)), rotations[i], new Vector2(1, 1), spriteBatch);
			}
			spriteBatch.End();
			spriteBatch.Begin();
		}
		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(projectile.position, 48, 48, DustID.TopazBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(projectile.position, 48, 48, DustID.SapphireBolt, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}
