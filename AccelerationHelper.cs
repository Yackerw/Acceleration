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

		static public void DrawSpriteRect(Texture2D texture, Vector2 position, Rectangle rect, Color color, float rotation, Vector2 scale, SpriteBatch batch = null, SpriteEffects spriteEffects = SpriteEffects.None)
		{
			if (batch == null)
			{
				batch = Main.spriteBatch;
			}
			batch.Draw(texture,
				position - Main.screenPosition,
				rect,
				color,
				rotation,
				new Vector2(rect.Width / 2, rect.Height / 2),
				scale,
				spriteEffects,
				0);
		}

		static public float GetMouseRotation(Player player)
		{
			return (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
		}
	}
}
