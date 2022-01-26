using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Acceleration;
using Terraria.ID;
using Terraria.Graphics.Shaders;
using System.IO;
using Mathj;
using Microsoft.Xna.Framework.Graphics;
using System;

/*public class RainbowRing : ModProjectile
{
	public override bool CanDamage()
	{
		return false;
	}

	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Rainbow Colored Ring");
	}

	public override void SetDefaults()
	{
		projectile.width = 30;
		projectile.height = 30;
		projectile.alpha = 255;
		projectile.timeLeft = 255;
		projectile.penetrate = -1;
		projectile.hostile = false;
		projectile.tileCollide = false;
		projectile.ignoreWater = true;
	}
}*/

namespace Acceleration.Misc
{
	public class RainbowRing
	{
		int existTime = 0;
		bool hit = false;
		bool synced = false;
		public int owner;
		static public RainbowRing firstRing;
		static public RainbowRing lastRing;
		public RainbowRing previous;
		public RainbowRing next;
		public float rotation;
		public static RainbowRingCallback rrc = new RainbowRingCallback();
		public Vector2 position;
		float scale;
		float alpha;

		public class RainbowRingCallback : SyncCallback
		{
			public override void Callback(BinaryReader reader)
			{
				int owner = reader.ReadByte();
				RainbowRing.Spawn(owner);
				// re-send callback
				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket mp = Acceleration.thisMod.GetPacket();
					mp.Write(reference);
					mp.Write((byte)owner);
					mp.Send(-1, owner);
				}
			}
		}

		static public void Update()
		{
			RainbowRing ring = firstRing;
			while (ring != null)
			{
				ring.AI();
				ring = ring.next;
			}
		}

		static public void UpdateDraw()
		{
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
			RainbowRing ring = firstRing;
			Texture2D tex = ModContent.GetTexture("Acceleration/Misc/RainbowRing");
			while (ring != null)
			{
				AccelerationHelper.DrawSpriteCached(tex, ring.position, (int)(((float)ring.existTime / 90f) * 16f), 141, new Color(255 - (int)ring.alpha, 255 - (int)ring.alpha, 255 - (int)ring.alpha, 255), ring.rotation, new Vector2(ring.scale, ring.scale));
				ring = ring.next;
			}
			Main.spriteBatch.End();
		}

		public static RainbowRing AddRing(Vector2 position, int owner)
		{
			RainbowRing retValue = new RainbowRing();
			retValue.previous = lastRing;
			retValue.next = null;
			if (firstRing == null)
			{
				firstRing = retValue;
			}
			if (lastRing != null)
			{
				lastRing.next = retValue;
			}
			lastRing = retValue;
			retValue.owner = owner;
			retValue.position = position;
			return retValue;
		}

		public static void RemoveRing(RainbowRing ring)
		{
			if (lastRing == ring)
			{
				lastRing = ring.previous;
			}
			if (firstRing == ring)
			{
				firstRing = ring.next;
			}
			if (ring.previous != null)
			{
				ring.previous.next = ring.next;
			}
			if (ring.next != null)
			{
				ring.next.previous = ring.previous;
			}
			// yep, that's it. just remove it from the linked list and orphan it
		}


		public static void Spawn(int owner)
		{
			Player player = Main.player[owner];
			AcceleratePlayer accelPlayer = player.GetModPlayer<AcceleratePlayer>();
			RainbowRing ring = AddRing(player.Center, owner);
			ring.rotation = accelPlayer.dashDirection;
		}

		public void UpdateValues(int owner)
		{
			Player player = Main.player[owner];
			AcceleratePlayer accelPlayer = player.GetModPlayer<AcceleratePlayer>();
			rotation = accelPlayer.dashDirection;
		}

		public void AI()
		{
			// uh oh, we shouldn't exist!!
			if (Main.player[owner] == null || !Main.player[owner].active)
			{
				RemoveRing(this);
				return;
			}
			++existTime;
			if (existTime >= 90)
			{
				RemoveRing(this);
			}
			scale = Matht.Lerp(0.8f, 1.8f, existTime / 90f);
			alpha = (int)(Matht.Lerp(0f, 1f, existTime / 90f) * 255);

			// TODO: iterate over projectiles and find collisions

			// wait 1 frame to sync, because game dumb
			if (existTime == 1)
			{
				UpdateValues(owner);
			}


			//Rectangle currRect = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
			Rectangle currRect = new Rectangle((int)position.X, (int)position.Y, 32, 141);
			// iterate over every projectile and check for collision
			if (!hit)
			{
				for (int i = 0; i < Main.projectile.Length; ++i)
				{
					Projectile currProj = Main.projectile[i];
					// not active, or friendly? no! i don't want that! give me the <hostile projectiles>
					if (!currProj.active || currProj.friendly)
					{
						continue;
					}
					Rectangle projRect = currProj.getRect();// = new Rectangle((int)currProj.position.X, (int)currProj.position.Y, currProj.width, currProj.height);
					//if (Matht.RectangleOnRectangleOverlap(currRect, projRect, npc.rotation, currProj.rotation))
					if (Collision.CheckAABBvAABBCollision(new Vector2(currRect.X, currRect.Y), new Vector2(currRect.Width, currRect.Height), new Vector2(projRect.X, projRect.Y), new Vector2(projRect.Width, projRect.Height))) 
					{
						// increase players hyper
						hit = true;
						AcceleratePlayer ap = Main.player[owner].GetModPlayer<AcceleratePlayer>();
						ap.hyper = Math.Min(ap.hyper + 0.04f, 3.0f);
						Main.PlaySound(Acceleration.RRSound);
						// create "CHARGE" pop-up thing
						//Projectile.NewProjectile(currProj.position, new Vector2(0, 0), mod.ProjectileType("Charge"), 0, 0);
						Charge.AddCharge(currProj.position);
						break;
					}

				}
			}
		}
	}
}