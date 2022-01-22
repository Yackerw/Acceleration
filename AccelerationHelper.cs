using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace Acceleration
{
	class AccelerationHelper
	{
		static public void DrawSprite(string texture, Vector2 position, int frame, int spriteHeight, Color color, float rotation, float scale = 1.0f)
		{
			Texture2D tex = Acceleration.thisMod.GetTexture(texture);
			Main.spriteBatch.Draw(tex,
				position - Main.screenPosition,
				new Rectangle(0, frame * spriteHeight, 0, spriteHeight),
				color,
				rotation,
				new Vector2(tex.Width / 2, spriteHeight / 2),
				scale,
				SpriteEffects.None,
				0);
		}
	}
}
