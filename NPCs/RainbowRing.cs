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

namespace Acceleration.NPCs
{
	public class RainbowRing : ModNPC
	{
		int existTime = 0;
		bool hit = false;
		bool synced = false;
		public static RainbowRingCallback rrc = new RainbowRingCallback();

		public class RainbowRingCallback : SyncCallback
		{
			public override void Callback(BinaryReader reader)
			{
				int owner = reader.ReadByte();
				RainbowRing.Spawn(owner);
			}
		}


		public static void Spawn(int owner)
		{
			Player player = Main.player[owner];
			AcceleratePlayer accelPlayer = player.GetModPlayer<AcceleratePlayer>();
			Vector2 newPos = new Vector2(12, -96);
			int newRing = NPC.NewNPC((int)player.position.X + (int)newPos.X, (int)player.position.Y - (int)newPos.Y, Acceleration.thisMod.NPCType("RainbowRing"), 0, owner);
			RainbowRing ring = (RainbowRing)Main.npc[newRing].modNPC;
			ring.npc.rotation = accelPlayer.dashDirection;
		}

		public void UpdateValues(int owner)
		{
			Player player = Main.player[owner];
			AcceleratePlayer accelPlayer = player.GetModPlayer<AcceleratePlayer>();
			npc.rotation = accelPlayer.dashDirection;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rainbow Colored Ring");
		}

		public override void SetDefaults()
		{
			npc.lifeMax = 1100;
			npc.damage = 0;
			npc.defense = 90;
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			npc.noGravity = true;
			npc.width = 45;
			npc.height = 96;
			Main.npcFrameCount[npc.type] = 16;
			npc.scale = 0.8f;
		}

		public override bool UsesPartyHat()
		{
			return false;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return false;
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frame.Y = (int)(((float)existTime / 90f) * 16f) * frameHeight;
		}

		public override void AI()
		{
			// uh oh, we shouldn't exist!!
			if (Main.player[(int)npc.ai[0]] == null || !Main.player[(int)npc.ai[0]].active)
			{
				npc.life = 0;
				npc.realLife = 0;
			}
			++existTime;
			if (existTime >= 90)
			{
				npc.realLife = 0;
				npc.life = 0;
			}
			npc.scale = Matht.Lerp(0.8f, 1.8f, existTime / 90f);
			npc.alpha = (int)(Matht.Lerp(0f, 1f, existTime / 90f) * 255);

			// TODO: iterate over projectiles and find collisions

			// wait 1 frame to sync, because game dumb
			if (existTime == 1)
			{
				UpdateValues((int)npc.ai[0]);
			}


			//Rectangle currRect = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
			Rectangle currRect = npc.getRect();
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
						AcceleratePlayer ap = Main.player[(int)npc.ai[0]].GetModPlayer<AcceleratePlayer>();
						ap.hyper = Math.Min(ap.hyper + 0.04f, 3.0f);
						Main.PlaySound(Acceleration.RRSound);
						// create "CHARGE" pop-up thing
						Projectile.NewProjectile(currProj.position, new Vector2(0, 0), mod.ProjectileType("Charge"), 0, 0);
						break;
					}

				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
			return base.PreDraw(spriteBatch, drawColor);
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			spriteBatch.End();
			spriteBatch.Begin();
			base.PostDraw(spriteBatch, drawColor);
		}
	}
	
	/*public class RainbowRing : ModProjectile
	{
		int existTime = 0;
		public AcceleratePlayer creator;
		bool hit = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rainbow Colored Ring");
			Main.projFrames[projectile.type] = 16;
		}

		public override void SetDefaults()
		{
			projectile.damage = 0;
			projectile.aiStyle = -1;
			projectile.width = 1;
			projectile.height = 1;
			projectile.scale = 0.8f;
		}

		/*public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			// this isn't working
			return false;
			if (hit || projectile.damage <= 0) return false;
			// increase players hyper
			creator.hyperGauge += 0.04f;
			hit = true;
			Main.PlaySound(AccelerationMod.RRSound);
			return false;
			// play sound effect
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return false;
			if (hit || item.damage <= 0) return false;
			// increase players hyper
			creator.hyperGauge += 0.04f;
			hit = true;
			Main.PlaySound(AccelerationMod.RRSound);
			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frame.Y = (int)(((float)existTime / 90f) * 16f) * frameHeight;
			//npc.frame.Width = 45;
			//npc.frame.Height = 141;
			//npc.frame.Y = (existTime / 100 * 16) * 45;
		}

		public override void AI()
		{
			++existTime;
			if (existTime >= 90)
			{
				npc.realLife = 0;
				npc.life = 0;
			}
			npc.scale = Matht.Lerp(0.8f, 1.8f, existTime / 90f);
			npc.alpha = (int)(Matht.Lerp(0f, 1f, existTime / 90f) * 255);
		}*/
		/*public override void AI()
		{
			++existTime;
			if (existTime >= 90)
			{
				projectile.Kill();
			}
			projectile.scale = Matht.Lerp(0.8f, 1.8f, existTime / 90f);
			projectile.alpha = (int)(Matht.Lerp(0f, 1f, existTime / 90f) * 255);
			projectile.frame = (int)(((float)existTime / 90f) * 16f);
		}
	}*/
}