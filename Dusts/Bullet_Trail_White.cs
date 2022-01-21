using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Acceleration.Dusts
{
	public class Bullet_Trail_White : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 16, 16);
			//If our texture had 2 different dust on top of each other (a 30x60 pixel image), we might do this:
			//dust.frame = new Rectangle(0, Main.rand.Next(2) * 30, 30, 30);
		}

		public override bool Update(Dust dust)
		{
			if (dust.firstFrame)
			{
				dust.firstFrame = false;
				return false;
			}
			dust.position += dust.velocity;
			dust.frame.Y += 16;
			if (dust.frame.Y > 64)
			{
				dust.active = false;
			}
			return false;
		}
	}
}