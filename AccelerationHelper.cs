using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
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
			tex = ModContent.Request<Texture2D>(texture).Value;
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

		static public int FindClosestNPC(Vector2 position, float threshold)
		{
			int closest = -1;
			float closestDist = threshold;
			for (int i = 0; i < Main.npc.Length; ++i)
			{
				NPC target = Main.npc[i];
				if (!target.active || target.friendly)
				{
					continue;
				}
				// get distance and compare it to our current distance
				float currDist = Mathj.Matht.Magnitude(position - target.position);
				if (currDist < closestDist)
				{
					closest = i;
					closestDist = currDist;
				}
			}
			return closest;
		}

		public static Projectile[] SummonLaser(int projType, IEntitySource source, float angle, Vector2 spawnPos, int damage, int knockback, int scale, int owner, bool spawnAll50)
		{
			Vector2 spawnIncrement = new Vector2(scale, 0).RotatedBy(angle);
			Projectile[] retValue = new Projectile[50];
			retValue[0] = Main.projectile[Projectile.NewProjectile(source, spawnPos, new Vector2(0, 0), projType, damage, knockback, owner, 0, angle)];
			int i;
			bool continueMovement = true;
			for (i = 1; i < 49; ++i)
			{
				if (continueMovement)
				{
					Vector2 moveAmnt = Collision.TileCollision(spawnPos, spawnIncrement, scale, scale, false, false, 0);
					spawnPos += moveAmnt;
					if (moveAmnt.X != spawnIncrement.X || moveAmnt.Y != spawnIncrement.Y)
					{
						continueMovement = false;
						if (!spawnAll50)
							break;
					}
				}
				Projectile proj = Main.projectile[Projectile.NewProjectile(source, spawnPos, new Vector2(0, 0), projType, damage, knockback, owner, 2 + ((i * 2) % 4 <= 1 ? 1 : 0), angle)];
				// hack for ordering...
				if (proj.whoAmI < retValue[0].whoAmI)
				{
					proj.timeLeft -= 1;
				}
				retValue[i] = proj;
			}
			retValue[i] = Main.projectile[Projectile.NewProjectile(source, spawnPos, new Vector2(0, 0), projType, damage, knockback, owner, 4, angle)];
			return retValue;
		}

		public static void UpdateLaser(Projectile[] laserProjs, Vector2 position, float angle, int scale)
		{
			Vector2 spawnIncrement = new Vector2(scale, 0).RotatedBy(angle);
			bool continueMovement = true;
			for (int i = 0; i < 50; ++i)
			{
				// reached the end, break
				if (laserProjs[i] == null)
				{
					break;
				}

				// reposition them
				laserProjs[i].position = position;
				laserProjs[i].ai[1] = angle;
				// find next position
				if (continueMovement)
				{
					Vector2 moveAmnt = Collision.TileCollision(position, spawnIncrement, scale, scale, false, false, 0);
					position += moveAmnt;
					if (moveAmnt.X != spawnIncrement.X || moveAmnt.Y != spawnIncrement.Y)
					{
						continueMovement = false;
					}
				}
			}
		}
	}
}
