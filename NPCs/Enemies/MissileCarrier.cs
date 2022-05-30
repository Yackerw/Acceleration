using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Acceleration;
using Terraria.ID;
using System.IO;
using Mathj;
using System;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

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
			if (Main.invasionType == (int)AccelerateWorld.Invasions.Saki)
			{
				return 1.0f;
			}
			return 0;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 250;
			NPC.damage = 15;
			NPC.defense = 8;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.width = 84;
			NPC.height = 40;
			Main.npcFrameCount[NPC.type] = 1;
			NPC.friendly = false;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Missile Carrier");
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

			Vector2 posDiff = target.position - NPC.position;

			if (Matht.Magnitude(posDiff) > 2400)
			{
				// despawn when off screen
				NPC.active = false;
				return;
			}

			// code for when the launchers are still alive
			if (NPC.ai[launchersAlive] >= 0.1f)
			{
				if (NPC.ai[waitTimer] < 250 && NPC.ai[waitTimer] > 90)
				{
					// try to match players height
					if (posDiff.Y > 500)
					{
						NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.1f, 10f);
					}
					else if (posDiff.Y < -500)
					{
						NPC.velocity.Y = Math.Max(NPC.velocity.Y - 0.1f, -10f);
					}
					else
					{
						if (NPC.velocity.Y > 0)
						{
							NPC.velocity.Y = Math.Max(0, NPC.velocity.Y - 0.1f);
						}
						else
						{
							NPC.velocity.Y = Math.Min(0, NPC.velocity.Y + 0.1f);
						}
					}

					// hover up if we're too close to the ground
					int outUp;
					int outDown;

					Collision.ExpandVertically((int)NPC.position.X / 16, (int)NPC.position.Y / 16, out outUp, out outDown, 100, 15);
					// minor leeway
					if (outDown < (NPC.position.Y / 16) + 13)
					{
						NPC.position.Y -= 4.0f;
						if (NPC.velocity.Y > 0)
						{
							NPC.velocity.Y = 0;
						}
					}
					// try to stay a certain distance away from target
					if (posDiff.X > 0)
					{
						NPC.spriteDirection = 1;
						if (posDiff.X > 500)
						{
							NPC.velocity.X = Math.Min(NPC.velocity.X + 0.1f, 5.0f);
						}
						else if (posDiff.X < 300)
						{
							NPC.velocity.X = Math.Max(NPC.velocity.X - 0.1f, -5.0f);
						}
						else
						{
							if (NPC.velocity.X > 0)
							{
								NPC.velocity.X = Math.Max(NPC.velocity.X - 0.4f, 0f);
							}
							else
							{
								NPC.velocity.X = Math.Min(NPC.velocity.X + 0.4f, 0f);
							}
						}
					}
					else
					{
						NPC.spriteDirection = -1;
						if (posDiff.X > -300)
						{
							NPC.velocity.X = Math.Min(NPC.velocity.X + 0.1f, 5.0f);
						}
						else if (posDiff.X < -500)
						{
							NPC.velocity.X = Math.Max(NPC.velocity.X - 0.1f, -5.0f);
						}
						else
						{
							if (NPC.velocity.X > 0)
							{
								NPC.velocity.X = Math.Max(NPC.velocity.X - 0.4f, 0f);
							}
							else
							{
								NPC.velocity.X = Math.Min(NPC.velocity.X + 0.4f, 0f);
							}
						}
					}
				} else
				{
					if (NPC.velocity.X > 0)
					{
						NPC.velocity.X = Math.Max(NPC.velocity.X - 0.4f, 0f);
					}
					else
					{
						NPC.velocity.X = Math.Min(NPC.velocity.X + 0.4f, 0f);
					}
					if (NPC.ai[waitTimer] <= 0)
					{
						NPC.ai[waitTimer] = 300;
					}
				}
				NPC.ai[waitTimer] -= 1;
			} else
			{
				switch (NPC.ai[state])
				{
					case 0:
						// vibrate up and down
						if ((((int)NPC.ai[waitTimer]) & 1) == 1)
						{
							NPC.position.Y += 2;
						}
						else
						{
							NPC.position.Y -= 2;
						}
						if (NPC.ai[waitTimer] <= 0)
						{
							NPC.ai[state] = 1;
							NPC.ai[waitTimer] = 160;
						}
						NPC.velocity = new Vector2(0, 0);
						break;
					case 1:
						// launch at the player roughly
						float dir = NPC.rotation;
						if (NPC.spriteDirection != 1 || NPC.direction != 1)
						{
							NPC.direction = 1;
							NPC.spriteDirection = 1;
							dir -= (float)Math.PI;
						}
						float angBetween = (float)Math.Atan2(posDiff.Y, posDiff.X);
						angBetween = Matht.AngleBetween(dir * Matht.Rad2Deg, angBetween * Matht.Rad2Deg) * Matht.Deg2Rad;
						angBetween = MathHelper.Clamp(angBetween, -3 * Matht.Deg2Rad, 3 * Matht.Deg2Rad);
						dir += angBetween;
						// speeeed
						float speed = Matht.Lerp(0, 6.0f, NPC.ai[waitTimer] / 160.0f);
						NPC.velocity = new Vector2(speed, 0).RotatedBy(dir);
						// spawn particle wahee
						Terraria.Dust.NewDust(NPC.Center + new Vector2(-40, 0).RotatedBy(dir), 10, 10, 269, -NPC.velocity.X, -NPC.velocity.Y);
						/*if (NPC.direction == -1)
						{
							dir += (float)Math.PI;
						}*/
						NPC.rotation = dir;
						// done being fast
						if (NPC.ai[waitTimer] <= 0)
						{
							NPC.velocity = new Vector2(0, 0);
							NPC.ai[state] = 2;
							NPC.ai[waitTimer] = 180;
						}
						break;

					case 2:
						// wait a bit and fire some projectiles
						if ((NPC.ai[waitTimer] > 110.1f && NPC.ai[waitTimer] <= 111.0f) || NPC.ai[waitTimer] > 30.1f && NPC.ai[waitTimer] <= 31.0f) {
							Vector2 plDiff = target.position - NPC.position;
							plDiff.Normalize();
							plDiff *= 5.0f;
							// proper offset depending on which way we're facing
							Vector2 shotOffset = new Vector2(NPC.direction == 1 ? 50 : -50, 0).RotatedBy(NPC.rotation);
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + shotOffset, plDiff, ModContent.ProjectileType<Projectiles.RoboShot>(), 12, 1.0f);
							SoundEngine.PlaySound(SoundID.Item43, NPC.position);
						}
						if (NPC.ai[waitTimer] <= 0)
						{
							// start all over again...
							NPC.ai[state] = 0;
							NPC.ai[waitTimer] = 120;
						}
						break;
				}
				NPC.ai[waitTimer] -= 1;
			}


			base.AI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(NPC.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(NPC.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(NPC.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
		}

		public override bool CheckDead()
		{
			// die particles
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(NPC.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(NPC.position, 80, 50, DustID.Water_BloodMoon, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
			return base.CheckDead();
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			if (NPC.ai[launchersAlive] > 0.1f)
			{
				return false;
			}
			return base.CanBeHitByItem(player, item);
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			//Main.NewText(NPC.ai[launchersAlive]);
			if (NPC.ai[launchersAlive] > 0.1f)
			{
				return false;
			}
			return base.CanBeHitByProjectile(projectile);
		}

		public override int SpawnNPC(int tileX, int tileY)
		{
			int retValue = base.SpawnNPC(tileX, tileY);
			// spawn our GOONS too
			launcherBit0 = NPC.NewNPC(NPC.GetSource_FromThis(), tileX, tileY, ModContent.NPCType<MissileCarrierPiece>(), 0, 0, 0, 0, retValue);
			// top front
			launcherBit1 = NPC.NewNPC(NPC.GetSource_FromThis(), tileX, tileY, ModContent.NPCType<MissileCarrierPiece>(), 0, 0, 8, 1, retValue);
			// bottom back
			launcherBit2 = NPC.NewNPC(NPC.GetSource_FromThis(), tileX, tileY, ModContent.NPCType<MissileCarrierPiece>(), 0, 1, 16, 0, retValue);
			// bottom front
			launcherBit3 = NPC.NewNPC(NPC.GetSource_FromThis(), tileX, tileY, ModContent.NPCType<MissileCarrierPiece>(), 0, 1, 24, 1, retValue);
			Main.npc[retValue].ai[launchersAlive] = 4;
			Main.npc[retValue].ai[waitTimer] = 300;
			return retValue;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 2, 1, 2));
		}
	}
}
