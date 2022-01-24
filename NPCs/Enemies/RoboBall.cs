using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Acceleration;
using Terraria.ID;
using System.IO;
using Mathj;
using System;
using Terraria.Audio;

namespace Acceleration.NPCs.Enemies
{
	class RoboBall : ModNPC
	{

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.Overworld.Chance * 0.1f;
		}
		public override void SetDefaults()
		{
			npc.lifeMax = 70;
			npc.damage = 15;
			npc.defense = 12;
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			npc.noGravity = true;
			npc.width = 44;
			npc.height = 30;
			Main.npcFrameCount[npc.type] = 1;
			npc.friendly = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.ai[0] = 4 * 60;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Robo Ball");
		}

		public override void AI()
		{
			// proper aggro handling!
			Player target = Main.player[npc.target];
			if (npc.target < 0 || npc.target == 255 || target.dead || !target.active)
			{
				npc.TargetClosest(true);
				target = Main.player[npc.target];
			}
			// wander away if all players dead...so, do nothing.
			// wander towards player if found
			if (!target.dead)
			{
				npc.spriteDirection = Math.Sign(target.position.X - npc.position.X);
				npc.velocity.X = 1.5f * (float)npc.spriteDirection;
			}
			npc.velocity.Y = 0;
			// hover up if we're too close to the ground
			int outUp;
			int outDown;
			Collision.ExpandVertically((int)npc.position.X / 16, (int)npc.position.Y / 16, out outUp, out outDown, 100, 10);
			// didn't reach max capacity?
			if (outDown < (int)((npc.position.X / 16) + 10))
			{
				outDown *= 16;
				float floorDiff = npc.position.Y - outDown;
				float targetPosition = -90.0f + (float)Math.Sin((float)npc.ai[1] / 27.0f) * 15;
				if (floorDiff < targetPosition)
				{
					npc.velocity.Y = Math.Min(targetPosition - floorDiff, 0.5f);
				} else
				{
					npc.velocity.Y = Math.Max(targetPosition - floorDiff, -0.5f);
				}
			}
			npc.ai[0] -= 1;
			npc.ai[1] += 1;
			if ((int)npc.ai[0] % 10 == 0)
			{
				npc.netUpdate = true;
			}
			// start some particles to show charging
			if (npc.ai[0] <= 80)
			{
				if (Main.rand.NextFloat() < 0.40)
				{
					Dust dust;
					// You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
					Vector2 position = npc.position - new Vector2(15, 15);
					dust = Main.dust[Terraria.Dust.NewDust(position, 30, 30, 43, 0f, 0f, 0, new Color(255, 255, 0), 0.4069768f)];
				}
			}
			if (npc.ai[0] <= 0)
			{
				// fire shot
				Vector2 speed = (target.position - npc.position);
				speed.Normalize();
				speed *= 5.0f;
				Projectile.NewProjectile(npc.position + new Vector2(10 * npc.spriteDirection, 20), speed, Acceleration.thisMod.ProjectileType("RoboShot"), 22, 1.0f);
				npc.ai[0] = 60*4;
				Main.PlaySound(SoundID.Item43, npc.position);
			}
			base.AI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(npc.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(npc.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(npc.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
		}

		public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
		{
			base.OnHitByItem(player, item, damage, knockback, crit);
			npc.ai[0] += 50;
			npc.netUpdate = true;
		}

		public override bool CheckDead()
		{
			// die particles
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(npc.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(npc.position, 20, 20, DustID.Water_BloodMoon, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
			return base.CheckDead();
		}
		public override void NPCLoot()
		{
			int choice = Main.rand.Next(0, 10);
			if (choice < 6)
			{
				if (choice < 3)
				{
					// drop 2
					Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 2);
				}
				else
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 1);
				}
			}
		}
	}
}
