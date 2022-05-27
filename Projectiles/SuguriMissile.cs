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
			Main.projFrames[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.width = 58;
			Projectile.height = 28;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.Name = "Missile";
		}

		public override void AI()
		{
			base.AI();
			++Projectile.frameCounter;
			if (Projectile.frameCounter >= 2)
			{
				Projectile.frame += 1;
				Projectile.frame %= 2;
			}
			int timeForCalc = Math.Max(0, Projectile.timeLeft - 535);
			if (timeForCalc < 50)
			{
				float speed = (1.0f - ((float)timeForCalc / 50.0f)) * 9.0f;
				// rotate ever so slightly to face player
				Player target = Main.player[(int)Projectile.ai[0]];
				if (target != null && target.active)
				{
					Vector2 playerDiff = target.position - Projectile.position;
					float newRotation = (float)Math.Atan2(playerDiff.Y, playerDiff.X);
					float rotation = Matht.AngleBetween(Projectile.rotation * Matht.Rad2Deg, newRotation * Matht.Rad2Deg) * Matht.Deg2Rad;
					rotation = Math.Min(Math.Max(-0.015f, rotation), 0.015f);
					Projectile.rotation += rotation;
					Projectile.velocity = new Vector2(speed * (float)Math.Cos(Projectile.rotation), speed * (float)Math.Sin(Projectile.rotation));
				}
			}

			// You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
			//Vector2 rotatedPos = new Vector2(((float)Math.Cos(Projectile.rotation) * -39.0f) - ((float)Math.Sin(Projectile.rotation) * -10.0f),
			//	((float)Math.Cos(Projectile.rotation) * -10.0f) + ((float)Math.Sin(Projectile.rotation) * -39.0f));
			Vector2 rotatedPos = new Vector2((float)Math.Cos(Projectile.rotation) * -37, (float)Math.Sin(Projectile.rotation) * -37);
			Vector2 position = Projectile.Center + rotatedPos;
			Terraria.Dust.NewDust(position, 10, 10, 269, -Projectile.velocity.X, -Projectile.velocity.Y);
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(Projectile.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(Projectile.position, 20, 20, DustID.Water_BloodMoon, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}