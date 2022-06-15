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

namespace Acceleration.Projectiles.Weapons.Ranged.PreHardMode
{
	class FriendlyBugMine : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 300;
			Projectile.penetrate = 1;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.Name = "Bit Projectile";
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			// just rotate a bit
			Projectile.rotation += 0.05f;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			base.OnHitNPC(target, damage, knockback, crit);
			AcceleratePlayer ap = Main.player[Projectile.owner].GetModPlayer<AcceleratePlayer>();
			ap.hyper += 0.015f;
			if (ap.hyper > 3.0f)
			{
				ap.hyper = 3.0f;
			}
			if (Projectile.timeLeft > 4)
			{
				Projectile.timeLeft = 4;
			}
		}

		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(Projectile.position, 32, 32, DustID.Stone, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}
