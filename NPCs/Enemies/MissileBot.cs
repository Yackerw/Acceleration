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
	class MissileBot : ModNPC
	{
		float spinValue = 0;
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.Overworld.Chance * 0.1f;
		}

		public override void SetDefaults()
		{
			npc.lifeMax = 50;
			npc.damage = 15;
			npc.defense = 8;
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			npc.noGravity = true;
			npc.width = 74;
			npc.height = 36;
			Main.npcFrameCount[npc.type] = 4;
			npc.friendly = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.ai[0] = 5 * 60;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Missile Bot");
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frame.Y = ((int)spinValue) * frameHeight;
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
			npc.velocity.Y = 0;
			// hover up if we're too close to the ground
			int outUp;
			int outDown;
			Collision.ExpandVertically((int)npc.position.X / 16, (int)npc.position.Y / 16, out outUp, out outDown, 100, 10);
			// didn't reach max capacity?
			/*if (outDown < (int)((npc.position.X / 16) + 10))
			{
				outDown *= 16;
				float floorDiff = npc.position.Y - outDown;
				float targetPosition = -90.0f + (float)Math.Sin((float)npc.ai[1] / 27.0f) * 15;
				if (floorDiff < targetPosition)
				{
					npc.velocity.Y = Math.Min(targetPosition - floorDiff, 0.5f);
				}
				else
				{
					npc.velocity.Y = Math.Max(targetPosition - floorDiff, -0.5f);
				}
			}
			npc.ai[1] += 1;*/
			npc.ai[0] -= 1;
			// speen
			float spinAdd = 0.15f;
			// spin progressively faster as we prepare to take off
			if (npc.ai[1] == 0)
			{
				if (npc.ai[0] <= 50)
				{
					spinAdd += (1.0f - (npc.ai[0] / 50.0f)) * 0.5f;
				}
				if (npc.ai[0] <= 0)
				{

					// blast off
					npc.ai[1] = 1;
					npc.ai[0] = 2 * 60;
					// choose a target position
					// first, how close are we to target? do we need to get closer, or further away?
					Vector2 targDiff = target.position - npc.position;
					float xAmnt = Main.rand.NextFloat(0.1f, 0.9f);
					float yAmnt = 1.0f - xAmnt;
					xAmnt *= 7.0f;
					yAmnt *= 7.0f;
					if (targDiff.X > 0)
					{
						// move right if we're to the left, and vice versa
						npc.ai[2] = xAmnt;
					} else
					{
						npc.ai[2] = -xAmnt;
					}
					// move down if we're too far away
					if (targDiff.Y > 200)
					{
						npc.ai[3] = yAmnt;
					} else
					{
						npc.ai[3] = -yAmnt;
					}
					// rotate...
					npc.rotation = (float)Math.Atan2(npc.ai[3], npc.ai[2]) - (float)Math.PI;
					npc.netUpdate = true;
				}
			}
			if (npc.ai[1] == 1)
			{
				// spin with s p e e d
				spinAdd = 0.65f;
				// move relative to how much time we have left
				npc.velocity = new Vector2(npc.ai[2], npc.ai[3]) * (npc.ai[0] / 120.0f);
				// fire a missile at certain intervals
				if ((int)npc.ai[0] == 90 || (int)npc.ai[0] == 40)
				{
					// no cheap shots to player
					Vector2 diffBetween = npc.position - target.position;
					float magnitude = (diffBetween.X * diffBetween.X) + (diffBetween.Y * diffBetween.Y);
					// 400*400
					if (magnitude < 160000)
					{
						// calculate speed to player
						Vector2 angleToPlayer = (target.position - npc.position);
						int missile = Projectile.NewProjectile(npc.position, new Vector2(0, 0), Acceleration.thisMod.ProjectileType("SuguriMissile"), 25, 2.0f, 255, target.whoAmI);
						Main.projectile[missile].rotation = (float)Math.Atan2(angleToPlayer.Y, angleToPlayer.X);
					}
				}
				if (npc.ai[0] <= 0)
				{
					npc.ai[1] = 0;
					npc.ai[0] = 4 * 60;
				}
			}

			spinValue += spinAdd;
			spinValue %= 4;
			base.AI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(npc.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(npc.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(npc.position, 20, 20, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
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
	}
}
