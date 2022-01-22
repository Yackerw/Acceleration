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
	class MissileCarrier : ModNPC
	{
		const int waitTimer = 0;
		public const int launchersAlive = 1;
		public const int state = 2;

		int launcherBit0;
		int launcherBit1;
		int launcherBit2;
		int launcherBit3;

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return 1.0f;
		}

		public override void SetDefaults()
		{
			npc.lifeMax = 250;
			npc.damage = 15;
			npc.defense = 8;
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			npc.noGravity = true;
			npc.width = 84;
			npc.height = 40;
			Main.npcFrameCount[npc.type] = 1;
			npc.friendly = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Missile Carrier");
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

			// code for when the launchers are still alive
			if (npc.ai[launchersAlive] >= 0.1f)
			{
				npc.ai[waitTimer] = 2 * 60;
				Vector2 posDiff = target.position - npc.position;
				// try to match players height
				if (posDiff.Y > 500)
				{
					npc.velocity.Y = Math.Min(npc.velocity.Y + 0.1f, 10f);
				} else if (posDiff.Y < -500)
				{
					npc.velocity.Y = Math.Max(npc.velocity.Y - 0.1f, -10f);
				} else
				{
					if (npc.velocity.Y > 0)
					{
						npc.velocity.Y = Math.Max(0, npc.velocity.Y - 0.1f);
					} else
					{
						npc.velocity.Y = Math.Min(0, npc.velocity.Y + 0.1f);
					}
				}

				// hover up if we're too close to the ground
				int outUp;
				int outDown;

				Collision.ExpandVertically((int)npc.position.X / 16, (int)npc.position.Y / 16, out outUp, out outDown, 100, 15);
				// minor leeway
				if (outDown < (npc.position.Y / 16) + 13)
				{
					npc.position.Y -= 4.0f;
					if (npc.velocity.Y > 0)
					{
						npc.velocity.Y = 0;
					}
				}
				// try to stay a certain distance away from target
				if (posDiff.X > 0)
				{
					npc.spriteDirection = 1;
					if (posDiff.X > 500)
					{
						npc.velocity.X = Math.Min(npc.velocity.X + 0.1f, 5.0f);
					} else if (posDiff.X < 300)
					{
						npc.velocity.X = Math.Max(npc.velocity.X - 0.1f, -5.0f);
					} else
					{
						if (npc.velocity.X > 0)
						{
							npc.velocity.X = Math.Max(npc.velocity.X - 0.4f, 0f);
						} else
						{
							npc.velocity.X = Math.Min(npc.velocity.X + 0.4f, 0f);
						}
					}
				} else
				{
					npc.spriteDirection = -1;
					if (posDiff.X > -300)
					{
						npc.velocity.X = Math.Min(npc.velocity.X + 0.1f, 5.0f);
					}
					else if (posDiff.X < -500)
					{
						npc.velocity.X = Math.Max(npc.velocity.X - 0.1f, -5.0f);
					}
					else
					{
						if (npc.velocity.X > 0)
						{
							npc.velocity.X = Math.Max(npc.velocity.X - 0.4f, 0f);
						}
						else
						{
							npc.velocity.X = Math.Min(npc.velocity.X + 0.4f, 0f);
						}
					}
				}
			} else
			{
				switch (npc.ai[state])
				{
					case 0:
						// vibrate up and down
						if ((((int)npc.ai[waitTimer]) & 1) == 1)
						{
							npc.position.Y += 2;
						}
						else
						{
							npc.position.Y -= 2;
						}
						if (npc.ai[waitTimer] <= 0)
						{
							npc.ai[state] = 1;
							npc.ai[waitTimer] = 160;
						}
						npc.velocity = new Vector2(0, 0);
						break;
					case 1:
						// launch at the player roughly
						float dir = npc.rotation;
						if (npc.direction == -1)
						{
							dir -= (float)Math.PI;
						}
						Vector2 posDiff = target.position - npc.position;
						float angBetween = (float)Math.Atan2(posDiff.Y, posDiff.X);
						angBetween = Matht.AngleBetween(dir * Matht.Rad2Deg, angBetween * Matht.Rad2Deg) * Matht.Deg2Rad;
						angBetween = MathHelper.Clamp(angBetween, -3 * Matht.Deg2Rad, 3 * Matht.Deg2Rad);
						dir += angBetween;
						// speeeed
						float speed = Matht.Lerp(0, 6.0f, npc.ai[waitTimer] / 160.0f);
						npc.velocity = new Vector2(speed, 0).RotatedBy(dir);
						if (npc.direction == -1)
						{
							dir += (float)Math.PI;
						}
						npc.rotation = dir;
						// done being fast
						if (npc.ai[waitTimer] <= 0)
						{
							npc.velocity = new Vector2(0, 0);
							npc.ai[state] = 2;
							npc.ai[waitTimer] = 180;
						}
						break;

					case 2:
						// wait a bit and fire some projectiles
						if ((npc.ai[waitTimer] > 110.1f && npc.ai[waitTimer] <= 111.0f) || npc.ai[waitTimer] > 30.1f && npc.ai[waitTimer] <= 31.0f) {
							Vector2 plDiff = target.position - npc.position;
							plDiff.Normalize();
							plDiff *= 5.0f;
							// proper offset depending on which way we're facing
							Vector2 shotOffset = new Vector2(npc.direction == 1 ? 50 : -50, 0).RotatedBy(npc.rotation);
							Projectile.NewProjectile(npc.Center + shotOffset, plDiff, Acceleration.thisMod.ProjectileType("RoboShot"), 22, 1.0f);
							Main.PlaySound(SoundID.Item43, npc.position);
						}
						if (npc.ai[waitTimer] <= 0)
						{
							// start all over again...
							npc.ai[state] = 0;
							npc.ai[waitTimer] = 120;
						}
						break;
				}
				npc.ai[waitTimer] -= 1;
			}


			base.AI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(npc.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(npc.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(npc.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
		}

		public override bool CheckDead()
		{
			// die particles
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(npc.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(npc.position, 80, 50, DustID.Water_BloodMoon, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
			return base.CheckDead();
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			if (npc.ai[launchersAlive] > 0.1f)
			{
				return false;
			}
			return base.CanBeHitByItem(player, item);
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			//Main.NewText(npc.ai[launchersAlive]);
			if (npc.ai[launchersAlive] > 0.1f)
			{
				return false;
			}
			return base.CanBeHitByProjectile(projectile);
		}

		public override int SpawnNPC(int tileX, int tileY)
		{
			int retValue = base.SpawnNPC(tileX, tileY);
			// spawn our GOONS too
			launcherBit0 = NPC.NewNPC(tileX, tileY, mod.NPCType("MissileCarrierPiece"), 0, 0, 0, 0, retValue);
			// top front
			launcherBit1 = NPC.NewNPC(tileX, tileY, mod.NPCType("MissileCarrierPiece"), 0, 0, 0, 1, retValue);
			// bottom back
			launcherBit2 = NPC.NewNPC(tileX, tileY, mod.NPCType("MissileCarrierPiece"), 0, 1, 0, 0, retValue);
			// bottom front
			launcherBit3 = NPC.NewNPC(tileX, tileY, mod.NPCType("MissileCarrierPiece"), 0, 1, 0, 1, retValue);
			Main.npc[retValue].ai[launchersAlive] = 4;
			return retValue;
		}
	}
}
