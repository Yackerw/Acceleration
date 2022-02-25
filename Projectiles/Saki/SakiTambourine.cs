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
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			base.PostDraw(spriteBatch, lightColor);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
			AccelerationHelper.DrawSprite("Projectiles/Tambourine_Emiss", projectile.Center, 0, 48, Color.White, projectile.rotation, new Vector2(1, 1), spriteBatch);
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
