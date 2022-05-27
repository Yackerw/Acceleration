using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Acceleration;
using Terraria.ID;
using System.IO;
using Mathj;
using System;
using Terraria.Audio;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;

namespace Acceleration.NPCs.Enemies
{
	class RoboBall : ModNPC
	{

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (Main.invasionType == (int)AccelerateWorld.Invasions.Saki)
			{
				return 1.0f;
			}
			return SpawnCondition.Overworld.Chance * 0.1f;
		}
		public override void SetDefaults()
		{
			NPC.lifeMax = 70;
			NPC.damage = 15;
			NPC.defense = 12;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.width = 44;
			NPC.height = 30;
			Main.npcFrameCount[NPC.type] = 1;
			NPC.friendly = false;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
			NPC.ai[0] = 4 * 60;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Robo Ball");
		}

		public override void AI()
		{
			// proper aggro handling!
			Player target = Main.player[NPC.target];
			if (NPC.target < 0 || NPC.target == 255 || target.dead || !target.active)
			{
				NPC.TargetClosest(true);
				target = Main.player[NPC.target];
			}
			// wander away if all players dead...so, do nothing.
			// wander towards player if found
			if (!target.dead)
			{
				NPC.spriteDirection = Math.Sign(target.position.X - NPC.position.X);
				NPC.velocity.X = 1.5f * (float)NPC.spriteDirection;
			}
			// despawn
			if (Matht.Magnitude(NPC.position - target.position) > 2400)
			{
				NPC.active = false;
				return;
			}
			NPC.velocity.Y = 0;
			// hover up if we're too close to the ground
			int outUp;
			int outDown;
			Collision.ExpandVertically((int)NPC.position.X / 16, (int)NPC.position.Y / 16, out outUp, out outDown, 100, 10);
			// didn't reach max capacity?
			if (outDown < (int)((NPC.position.X / 16) + 10))
			{
				outDown *= 16;
				float floorDiff = NPC.position.Y - outDown;
				float targetPosition = -90.0f + (float)Math.Sin((float)NPC.ai[1] / 27.0f) * 15;
				if (floorDiff < targetPosition)
				{
					NPC.velocity.Y = Math.Min(targetPosition - floorDiff, 0.5f);
				} else
				{
					NPC.velocity.Y = Math.Max(targetPosition - floorDiff, -0.5f);
				}
			}
			NPC.ai[0] -= 1;
			NPC.ai[1] += 1;
			if ((int)NPC.ai[0] % 10 == 0)
			{
				NPC.netUpdate = true;
			}
			// start some particles to show charging
			if (NPC.ai[0] <= 80)
			{
				if (Main.rand.NextFloat() < 0.40)
				{
					Dust dust;
					// You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
					Vector2 position = NPC.position - new Vector2(15, 15);
					dust = Main.dust[Terraria.Dust.NewDust(position, 30, 30, 43, 0f, 0f, 0, new Color(255, 255, 0), 0.4069768f)];
				}
			}
			if (NPC.ai[0] <= 0)
			{
				// fire shot
				Vector2 speed = (target.position - NPC.position);
				speed.Normalize();
				speed *= 5.0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(),NPC.position + new Vector2(10 * NPC.spriteDirection, 20), speed, ModContent.ProjectileType<Projectiles.RoboShot>(), 12, 1.0f);
				NPC.ai[0] = 60*4;
				SoundEngine.PlaySound(SoundID.Item43, NPC.position);
			}
			base.AI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(NPC.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(NPC.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(NPC.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
		}

		public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
		{
			base.OnHitByItem(player, item, damage, knockback, crit);
			NPC.ai[0] += 50;
			NPC.netUpdate = true;
		}

		public override bool CheckDead()
		{
			// die particles
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(NPC.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(NPC.position, 20, 20, DustID.Water_BloodMoon, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
			return base.CheckDead();
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 2, 1, 2));
		}
	}
}
