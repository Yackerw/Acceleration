using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Acceleration.Projectiles.Saki;
using Acceleration.Items.Bags;
using Terraria.GameContent.ItemDropRules;
using Mathj;

namespace Acceleration.NPCs.Bosses
{
	[AutoloadBossHead]
	class Saki : ModNPC
	{

		const int AITimer = 0;
		const int AIState = 1;
		const int AINextState = 2;
		const int AIUnused = 3;

		int generalCounter = 0;

		Vector2 targetPosition;
		Vector2 spawnPoint;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Saki");
			NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 2700;
			NPC.damage = -1;
			NPC.defense = 6;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.width = 20;
			NPC.height = 47;
			NPC.friendly = false;
			NPC.noTileCollide = true;
			NPC.boss = true;
			NPC.npcSlots = 50;
			Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/MigratoryBirdFromNorth");
			SceneEffectPriority = SceneEffectPriority.BossLow;
			NPC.ai[AINextState] = 1;
			NPC.value = Item.buyPrice(0, 6, 50, 0);
			NPC.netAlways = true;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			NPC.lifeMax = 2700 + 400 + (800 * numPlayers);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(targetPosition.X);
			writer.Write(targetPosition.Y);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			targetPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}

		void IncrementFrameCounter(int frameLength)
		{
			// storing frames in a strange way...
			int Y = (int)NPC.frameCounter / 100;
			int X = (int)NPC.frameCounter - (Y * 100);
			++X;
			X = X % frameLength;
			NPC.frameCounter = X + (Y * 100);
		}

		void ChangeSetKeepFrame(int set)
		{
			int Y = (int)NPC.frameCounter / 100;
			int X = (int)NPC.frameCounter - (Y * 100);
			NPC.frameCounter = set + X;
		}

		void SetFrameManually(int frame)
		{
			int Y = (int)NPC.frameCounter / 100;
			NPC.frameCounter = (Y * 100) + frame;
		}

		public override void AI()
		{
			NPC.DiscourageDespawn(100);
			NPC.netUpdate = true;
			if (spawnPoint.Y == 0)
			{
				// set this so we can keep the NPC around where it started
				spawnPoint = NPC.position;
			}
			// handle aggro
			Player target = Main.player[NPC.target];
			if (NPC.target < 0 || NPC.target == 255 || target.dead || !target.active || Matht.Magnitude(target.position - NPC.position) > 4000)
			{
				NPC.TargetClosest(true);
				target = Main.player[NPC.target];
			}
			// leave if all targets dead/too far away
			if (target.dead || Matht.Magnitude(target.position - NPC.position) > 8000 && NPC.ai[AIState] != 0)
			{
				if (NPC.ai[AIState] != 200)
				{
					Main.NewText("Saki has defeated all players!", Color.Green);
					NPC.ai[AIState] = 200;
				}
			}
			Vector2 targDiff = target.position - NPC.position;
			switch (NPC.ai[AIState])
			{
				case 0:
					// charge towards nearest player
					if (Matht.Magnitude(targDiff) > 600)
					{
						Vector2 moveAmnt = targDiff;
						moveAmnt.Normalize();
						moveAmnt *= 12.0f;
						NPC.position += moveAmnt;
						// face the player
						if (targDiff.X > 0)
						{
							NPC.spriteDirection = 1;
						}
						else
						{
							NPC.spriteDirection = -1;
						}
						if (moveAmnt.X > 0)
						{
							if (NPC.spriteDirection == 1)
							{
								ChangeSetKeepFrame(200);
							}
							else
							{
								ChangeSetKeepFrame(100);
							}
						}
						else
						{
							if (NPC.spriteDirection == 1)
							{
								ChangeSetKeepFrame(100);
							}
							else
							{
								ChangeSetKeepFrame(200);
							}
						}
						if (generalCounter % 3 == 0)
						{
							IncrementFrameCounter(4);
						}
					} else
					{
						NPC.ai[AIState] = 1;
					}
					break;
				case 1:
					// prepare to fly to a specific spot, and set up our next state
					if (NPC.life > NPC.lifeMax / 2)
					{
						NPC.ai[AINextState] = Main.rand.Next(0, 4);
					} else
					{
						// BIG BANG ATTACK
						NPC.ai[AINextState] = Main.rand.Next(0, 5);
					}
					switch (NPC.ai[AINextState])
					{
						case 1:
							// maraccas
							NPC.ai[AINextState] = 3;
							break;
						case 2:
							// semi-circular shots
							NPC.ai[AINextState] = 4;
							break;
						case 4:
							// big bang bell
							NPC.ai[AINextState] = 5;
							break;
						default:
							NPC.ai[AINextState] = 1;
							break;
					}
					// set us to fly
					NPC.ai[AIState] = 2;
					//NPC.frameCounter = 200;
					// set our position
					targetPosition = new Vector2(Main.rand.NextFloat(-300, 300), Main.rand.NextFloat(-300, 300));
					// ensure it stays around the spawn point enough...and don't go beneath the player (unless they're above our spawn point)
					// try to move towards player if we're too far away
					while (((NPC.position.Y + targetPosition.Y) > target.position.Y && Matht.DotProduct(targetPosition, new Vector2(0, -1)) < 0) || (Matht.Magnitude((NPC.position + targetPosition) - target.position) > 300 && Matht.DotProduct(targetPosition, target.position - NPC.position) < 0.1f))
					//while (Matht.Magnitude((NPC.position + targetPosition) - spawnPoint) > 800 || ((NPC.position.Y + targetPosition.Y) > target.position.Y && target.position.Y > spawnPoint.Y) || NPC.position.Y + targetPosition.Y > spawnPoint.Y)
					{
						targetPosition = new Vector2(Main.rand.NextFloat(-300, 300), Main.rand.NextFloat(-300, 300));
					}
					targetPosition += NPC.position;
					SetFrameManually(0);
					// if player is too far away, charge back to them
					if (Matht.Magnitude(target.position - NPC.position) >= 1300)
					{
						NPC.ai[AIState] = 0;
					}
					break;
				case 2:
					// travel to target position, unless we already there
					Vector2 posToMove = targetPosition - NPC.position;
					posToMove.Normalize();
					posToMove *= 3.0f;
					NPC.position += posToMove;
					Vector2 newPosToMove = targetPosition - NPC.position;
					// face the player
					if (targDiff.X > 0)
					{
						NPC.spriteDirection = 1;
					} else
					{
						NPC.spriteDirection = -1;
					}
					if (posToMove.X > 0)
					{
						if (NPC.spriteDirection == 1)
						{
							ChangeSetKeepFrame(200);
						}
						else
						{
							ChangeSetKeepFrame(100);
						}
					} else
					{
						if (NPC.spriteDirection == 1)
						{
							ChangeSetKeepFrame(100);
						}
						else
						{
							ChangeSetKeepFrame(200);
						}
					}
					posToMove.Normalize();
					newPosToMove.Normalize();
					if (generalCounter % 3 == 0)
					{
						IncrementFrameCounter(4);
					}

					// spawn a projectile every now and then
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (generalCounter % 45 == 0)
						{
							switch (Main.rand.Next(0, 3))
							{
								case 0:
									Vector2 tambSpeed = targDiff;
									tambSpeed.Normalize();
									tambSpeed *= 1.5f;
									Projectile.NewProjectile(NPC.GetSource_FromThis(),NPC.position, tambSpeed, ModContent.ProjectileType<SakiTambourine>(), 15, 1.0f);
									break;
								default:
									// this is dumb why is ai0 not working
									Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, new Vector2(0, 6).RotatedByRandom(360 * Matht.Deg2Rad), ModContent.ProjectileType<SakiOrb>(), 15, 1.0f, Main.myPlayer, NPC.whoAmI, target.whoAmI);
									break;
							}
						}
					}

					if (Matht.DotProduct(posToMove, newPosToMove) <= 0)
					{
						NPC.ai[AIState] = NPC.ai[AINextState];
						if (NPC.ai[AIState] == 1) {
							NPC.ai[AINextState] = 2;
							NPC.ai[AITimer] = 0;
						} else if (NPC.ai[AIState] == 3 || NPC.ai[AIState] == 4)
						{
							NPC.ai[AITimer] = 160;
						}
						else
						{
							NPC.ai[AINextState] = 0;
						}
					}

					break;
				case 3:
					NPC.frameCounter = Math.Min(((160 - (int)NPC.ai[AITimer]) / 5), 11);
					ChangeSetKeepFrame(500);
					--NPC.ai[AITimer];
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (NPC.ai[AITimer] <= 135 && NPC.ai[AITimer] >= 125)
						{
							// spawn maraccas in random directions
							Projectile mar = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, new Vector2(Main.rand.NextFloat(-6.0f, 6.0f), Main.rand.NextFloat(-6.0f, 0.0f)), ModContent.ProjectileType<SakiMaracca>(), 13, 3.0f, Main.myPlayer, Main.rand.Next(0, 2))];
							//mar.ai[0] = Main.rand.Next(0, 2);
						}
					}
					if (NPC.ai[AITimer] <= 0)
					{
						NPC.ai[AIState] = 1;
					}
					break;
				case 4:
					NPC.frameCounter = Math.Min(((160 - (int)NPC.ai[AITimer]) / 5), 11);
					ChangeSetKeepFrame(500);
					--NPC.ai[AITimer];
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (NPC.ai[AITimer] <= 135 && NPC.ai[AITimer] >= 115)
						{
							// pellets in a rotational pattern...
							if (NPC.ai[AITimer] % 2 == 1)
							{
								float rotationalAdditive = (NPC.ai[AITimer] - 125) * 1.7f * Matht.Deg2Rad;
								for (int i = 0; i < 5; ++i)
								{
									float rotationalBase = i * 72 * Matht.Deg2Rad;
									Projectile proj = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, new Vector2(1, 0).RotatedBy(rotationalBase + rotationalAdditive), ModContent.ProjectileType<SakiPellet>(), 13, 3.0f, Main.myPlayer, 0, 0)];
									// minus a little for blue
									proj = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, new Vector2(1, 0).RotatedBy(rotationalBase + rotationalAdditive - (3 * Matht.Deg2Rad)), ModContent.ProjectileType<SakiPellet>(), 13, 3.0f, Main.myPlayer, 1, 1)];
								}
							}
						}
					}
					if (NPC.ai[AITimer] <= 0)
					{
						NPC.ai[AIState] = 1;
					}
					break;
				case 5:
					// do a little prep work for big bang bell
					NPC.ai[AIState] = 6;
					NPC.ai[AITimer] = 120;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 170), Vector2.Zero, ModContent.ProjectileType<SakiBigBangBell>(), 20, 1.0f, 255, target.whoAmI, target.whoAmI);
					}
					SoundEngine.PlaySound(Acceleration.hyperSound, NPC.position);
					break;
				case 6:
					// animate
					--NPC.ai[AITimer];
					if (NPC.ai[AITimer] > 20)
					{
						NPC.frameCounter = Math.Min(2, 24 - (NPC.ai[AITimer] / 5));
					} else
					{
						NPC.frameCounter = 3 + Math.Min(2, 4 - (NPC.ai[AITimer] / 5));
					}
					ChangeSetKeepFrame(700);
					// done
					if (NPC.ai[AITimer] <= 0)
					{
						NPC.ai[AIState] = 1;
					}
					break;
				case 100:
					if (generalCounter % 2 == 0)
					{
						IncrementFrameCounter(2);
					}
					--NPC.ai[AITimer];
					if (NPC.ai[AITimer] <= 0)
					{
						NPC.ai[AIState] = 101;
						NPC.StrikeNPCNoInteraction(9999, 0, 0);
						// TODO: particle effects, sfx for death
						for (int i = 0; i < 5; ++i)
						{
							Dust.NewDust(NPC.Center, 20, 20, DustID.GemTopaz, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6));
							Dust.NewDust(NPC.Center, 20, 20, DustID.GemSapphire, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6));
							Dust.NewDust(NPC.Center, 20, 20, DustID.GemRuby, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6));
							Dust.NewDust(NPC.Center, 20, 20, DustID.GemEmerald, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6));
							Dust.NewDust(NPC.Center, 20, 20, DustID.GemAmethyst, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6));
						}
						SoundEngine.PlaySound(Acceleration.bossDeathSound, NPC.position);
					}
					break;
				case 200:
					// simply leave
					IncrementFrameCounter(8);
					ChangeSetKeepFrame(0);
					NPC.velocity.Y -= 0.25f;
					NPC.velocity.Y = Math.Max(-8.0f, NPC.velocity.Y);
					if (NPC.position.Y <= 0)
					{
						NPC.active = false;
					}
					break;
			}

			++generalCounter;
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return NPC.ai[AIState] > 100 ? false : base.CanBeHitByItem(player, item);
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return NPC.ai[AIState] > 100 ? false : base.CanBeHitByProjectile(projectile);
		}

		public override bool CheckDead()
		{
			if (NPC.ai[AIState] != 101)
			{
				if (NPC.ai[AIState] != 100)
				{
					NPC.ai[AIState] = 100;
					NPC.ai[AITimer] = 140;
					NPC.frameCounter = 1000;
					NPC.life = 1;
					NPC.active = true;
					NPC.dontTakeDamage = true;
				}
				return false;
			}
			NPC.friendly = false;
			NPC.boss = true;
			AccelerateWorld.sakiDefeated = true;
			return base.CheckDead();
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Ranged.Maracca>(), 3, 30, 50));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Melee.Tambourine>(), 3, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Magic.PowerBell>(), 3, 1, 1));
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SakiBag>()));
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// since bosses use a lot of sprites, this is a custom system set up that spans both the X and Y axis. 100 = go down 1 on the Y axis, 1 = go right 1 on the X axis
			int Y = (int)NPC.frameCounter / 100;
			int X = (int)NPC.frameCounter - (Y * 100);
			Rectangle frame = new Rectangle(X * 66, Y * 66, 64, 64);

			AccelerationHelper.DrawSpriteRect(ModContent.Request<Texture2D>("Acceleration/NPCs/Bosses/SakiSheet").Value, NPC.Center, frame, drawColor, 0, Vector2.One, spriteBatch, NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

			// draw a little circle that closes in on her before she goes "pop"
			if (NPC.ai[AIState] == 100)
			{
				if (NPC.ai[AITimer] < 60)
				{
					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
					float circleScale = (NPC.ai[AITimer] / 60) * 2.5f;
					AccelerationHelper.DrawSprite("Acceleration/Sprites/Circle", NPC.Center, 0, 256, new Color(1.0f, 1.0f, 1.0f, 1.0f - Math.Max((NPC.ai[AITimer] - 40) / 20.0f, 0)), 0, new Vector2(circleScale, circleScale), spriteBatch);
					spriteBatch.End();
					spriteBatch.Begin();
				}
			}

			return false;
		}
	}
}
