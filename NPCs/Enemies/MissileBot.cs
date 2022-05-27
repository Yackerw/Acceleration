using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Acceleration;
using Terraria.ID;
using System.IO;
using Mathj;
using System;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;

namespace Acceleration.NPCs.Enemies
{
	class MissileBot : ModNPC
	{
		float spinValue = 0;
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
			NPC.lifeMax = 50;
			NPC.damage = 15;
			NPC.defense = 8;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.width = 74;
			NPC.height = 36;
			Main.npcFrameCount[NPC.type] = 4;
			NPC.friendly = false;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
			NPC.ai[0] = 5 * 60;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Missile Bot");
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = ((int)spinValue) * frameHeight;
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
			NPC.velocity.Y = 0;
			Vector2 targDiff = target.position - NPC.position;
			// despawn if too far away
			if (Matht.Magnitude(targDiff) > 2400)
			{
				NPC.active = false;
				return;
			}
			NPC.ai[0] -= 1;
			// speen
			float spinAdd = 0.15f;
			// spin progressively faster as we prepare to take off
			if (NPC.ai[1] == 0)
			{
				if (NPC.ai[0] <= 50)
				{
					spinAdd += (1.0f - (NPC.ai[0] / 50.0f)) * 0.5f;
				}
				if (NPC.ai[0] <= 0)
				{

					// blast off
					NPC.ai[1] = 1;
					NPC.ai[0] = 2 * 60;
					// choose a target position
					// first, how close are we to target? do we need to get closer, or further away?
					float xAmnt = Main.rand.NextFloat(0.1f, 0.9f);
					float yAmnt = 1.0f - xAmnt;
					xAmnt *= 7.0f;
					yAmnt *= 7.0f;
					if (targDiff.X > 0)
					{
						// move right if we're to the left, and vice versa
						NPC.ai[2] = xAmnt;
					} else
					{
						NPC.ai[2] = -xAmnt;
					}
					// move down if we're too far away
					if (targDiff.Y > 200)
					{
						NPC.ai[3] = yAmnt;
					} else
					{
						NPC.ai[3] = -yAmnt;
					}
					// rotate...
					NPC.rotation = (float)Math.Atan2(NPC.ai[3], NPC.ai[2]) - (float)Math.PI;
					NPC.netUpdate = true;
				}
			}
			if (NPC.ai[1] == 1)
			{
				// spin with s p e e d
				spinAdd = 0.65f;
				// move relative to how much time we have left
				NPC.velocity = new Vector2(NPC.ai[2], NPC.ai[3]) * (NPC.ai[0] / 120.0f);
				// fire a missile at certain intervals
				if ((int)NPC.ai[0] == 90 || (int)NPC.ai[0] == 40)
				{
					// no cheap shots to player
					Vector2 diffBetween = NPC.position - target.position;
					float magnitude = (diffBetween.X * diffBetween.X) + (diffBetween.Y * diffBetween.Y);
					// 400*400
					if (magnitude < 160000)
					{
						// calculate speed to player
						Vector2 angleToPlayer = (target.position - NPC.position);
						int missile = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, new Vector2(0, 0), ModContent.ProjectileType<Projectiles.SuguriMissile>(), 15, 2.0f, 255, target.whoAmI);
						Main.projectile[missile].rotation = (float)Math.Atan2(angleToPlayer.Y, angleToPlayer.X);
					}
				}
				if (NPC.ai[0] <= 0)
				{
					NPC.ai[1] = 0;
					NPC.ai[0] = 4 * 60;
				}
			}

			spinValue += spinAdd;
			spinValue %= 4;
			base.AI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(NPC.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(NPC.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(NPC.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
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
