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

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 48;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.Name = "Tambourine";
		}

		public override void AI()
		{
			// simply rotate a bit
			Projectile.rotation += 3 * Matht.Deg2Rad;
		}

		public override void PostDraw(Color lightColor)
		{
			base.PostDraw(lightColor);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
			AccelerationHelper.DrawSprite("Acceleration/Projectiles/Saki/Tambourine_Emiss", Projectile.Center, 0, 48, Color.White, Projectile.rotation, new Vector2(1, 1), Main.spriteBatch);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin();
		}
		public override void Kill(int timeLeft)
		{
			Vector2 position = Projectile.position - new Vector2(15, 15);
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(Projectile.position, 48, 48, DustID.GemTopaz, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(Projectile.position, 48, 48, DustID.GemSapphire, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
		}
	}
}
