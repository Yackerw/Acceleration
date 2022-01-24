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
		static public void DrawSpriteCached(Texture2D texture, Vector2 position, int frame, int spriteHeight, Color color, float rotation, Vector2 scale, SpriteBatch batch = null)
		{
			if (batch == null)
			{
				batch = Main.spriteBatch;
			}
			batch.Draw(texture,
				position - Main.screenPosition,
				new Rectangle(0, frame * spriteHeight, texture.Width, spriteHeight),
				color,
				rotation,
				new Vector2(texture.Width / 2, spriteHeight / 2),
				scale,
				SpriteEffects.None,
				0);
		}

		static public void DrawSprite(string texture, Vector2 position, int frame, int spriteHeight, Color color, float rotation, Vector2 scale, SpriteBatch batch = null)
		{
			Texture2D tex;
			tex = Acceleration.thisMod.GetTexture(texture);
			DrawSpriteCached(tex, position, frame, spriteHeight, color, rotation, scale, batch);
		}
	}
}
