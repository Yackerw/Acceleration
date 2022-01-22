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
using Mathj;
using Acceleration.NPCs;

namespace Acceleration.Projectiles
{
	class SuguriMissile : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 74;
			projectile.height = 42;
			//projectile.alpha = 50;
			projectile.timeLeft = 600;
			projectile.penetrate = -1;
			projectile.hostile = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.Name = "Missile";
		}

		public override void AI()
		{
			base.AI();
			int timeForCalc = Math.Max(0, projectile.timeLeft - 535);
			if (timeForCalc < 50)
			{
				float speed = (1.0f - ((float)timeForCalc / 50.0f)) * 9.0f;
				// rotate ever so slightly to face player
				Player target = Main.player[(int)projectile.ai[0]];
				if (target != null && target.active)
				{
					Vector2 playerDiff = target.position - projectile.position;
					float newRotation = (float)Math.Atan2(playerDiff.Y, playerDiff.X);
					float rotation = Matht.AngleBetween(projectile.rotation * Matht.Rad2Deg, newRotation * Matht.Rad2Deg) * Matht.Deg2Rad;
					rotation = Math.Min(Math.Max(-0.015f, rotation), 0.015f);
					projectile.rotation += rotation;
					projectile.velocity = new Vector2(speed * (float)Math.Cos(projectile.rotation), speed * (float)Math.Sin(projectile.rotation));
				}
			}

			// You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
			//Vector2 rotatedPos = new Vector2(((float)Math.Cos(projectile.rotation) * -39.0f) - ((float)Math.Sin(projectile.rotation) * -10.0f),
			//	((float)Math.Cos(projectile.rotation) * -10.0f) + ((float)Math.Sin(projectile.rotation) * -39.0f));
			Vector2 rotatedPos = new Vector2((float)Math.Cos(projectile.rotation) * -37, (float)Math.Sin(projectile.rotation) * -37);
			Vector2 position = projectile.Center + rotatedPos;
			Terraria.Dust.NewDust(position, 10, 10, 269, -projectile.velocity.X, -projectile.velocity.Y);
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(projectile.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(projectile.position, 20, 20, DustID.Water_BloodMoon, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}