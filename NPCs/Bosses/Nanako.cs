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
using Mathj;

namespace Acceleration.NPCs.Bosses
{
	[AutoloadBossHead]
	class Nanako : ModNPC
	{

		const int AITimer = 0;
		const int AIState = 1;
		const int AINextState = 2;
		const int AIUnused = 3;

		int generalCounter = 0;

		Vector2 targetPosition;
		Vector2 spawnPoint;

		Vector2[] bitPositions = new Vector2[7];
		int[] bitStates = new int[7];
		int[] bitSprites = new int[7];
		int[] bitTimers = new int[7];
		float[] bitRotations = new float[7];
		Projectile[] bitDashProjs = new Projectile[7];
		Projectile[] bitLasers1 = new Projectile[50];
		Projectile[] bitLasers2 = new Projectile[50];
		Projectile[] bitLasers3 = new Projectile[50];

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nanako");
			NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 4000;
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
			Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LinearLight");
			SceneEffectPriority = SceneEffectPriority.BossLow;
			NPC.ai[AINextState] = 1;
			NPC.value = Item.buyPrice(0, 6, 50, 0);
			NPC.netAlways = true;
			for (int i = 0; i < 7; ++i)
			{
				bitStates[i] = 0;
				bitSprites[i] = 0;
				bitRotations[i] = 0;
				bitTimers[i] = 0;
				bitPositions[i] = Vector2.Zero;
			}
		}

		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			NPC.lifeMax = 4000 + 800 + (1400 * numPlayers);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(targetPosition.X);
			writer.Write(targetPosition.Y);
			for (int i = 0; i < 7; ++i)
			{
				writer.Write(bitPositions[i].X);
				writer.Write(bitPositions[i].Y);
				writer.Write((byte)bitStates[i]);
				writer.Write((byte)bitSprites[i]);
				writer.Write(bitTimers[i]);
			}
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			targetPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			for (int i = 0; i < 7; ++i)
			{
				bitPositions[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
				bitStates[i] = reader.ReadByte();
				bitSprites[i] = reader.ReadByte();
				bitTimers[i] = reader.ReadInt32();
			}
		}

		void IncrementFrameCounter(int frameStartLoop, int frameLength)
		{
			// storing frames in a strange way...
			int Y = (int)NPC.frameCounter / 100;
			int X = (int)NPC.frameCounter - (Y * 100);
			++X;
			if (X >= frameLength)
			{
				X = frameStartLoop;
			}
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
			if (target.dead || Matht.Magnitude(target.position - NPC.position) > 4000 && NPC.ai[AIState] != 0)
			{
				if (NPC.ai[AIState] != 200)
				{
					Main.NewText("Nanako has defeated all players!", Color.Green);
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
							IncrementFrameCounter(0, 4);
						}
					}
					else
					{
						NPC.ai[AIState] = 50;
						NPC.ai[AITimer] = 0;
					}
					break;
				case 1:
					// prepare to fly to a specific spot, and set up our next state
					if (NPC.life > NPC.lifeMax / 2)
					{
						// nanako is largely passive, so she spends most of her time just waffling about
						NPC.ai[AINextState] = Main.rand.Next(0, 10);
					}
					else
					{
						// BIG BANG ATTACK
						NPC.ai[AINextState] = Main.rand.Next(0, 16);
					}
					switch (NPC.ai[AINextState])
					{
						case 1:
						case 2:
						case 3:
							// charging attack	
							NPC.ai[AINextState] = 3;
							break;
						case 4:
						case 5:
							// spinning attack
							NPC.ai[AINextState] = 4;
							break;
						case 6:
							// super spinning attack
							NPC.ai[AINextState] = 5;
							break;
						default:
							// perform her "hyper" under the right circumstances
							if (NPC.life < NPC.lifeMax / 2 && NPC.ai[AINextState] == 7)
							{
								NPC.ai[AINextState] = 6;
								break;
							}
							// fly somewhere new
							NPC.ai[AINextState] = 1;
							break;
					}
					// set us to fly
					NPC.ai[AIState] = 2;
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
					}
					else
					{
						NPC.spriteDirection = -1;
					}
					if (posToMove.X > 0)
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
					else
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
					posToMove.Normalize();
					newPosToMove.Normalize();
					if (generalCounter % 3 == 0)
					{
						IncrementFrameCounter(0, 4);
					}

					if (Matht.DotProduct(posToMove, newPosToMove) <= 0)
					{
						NPC.ai[AIState] = NPC.ai[AINextState];
						if (NPC.ai[AIState] == 1)
						{
							NPC.ai[AINextState] = 2;
							NPC.ai[AITimer] = 0;
						}
						else
						{
							NPC.ai[AINextState] = 0;
							NPC.ai[AITimer] = 0;
						}
					}

					break;
				case 3:
					ChangeSetKeepFrame(700);
					// set our bits to actually dash
					if (NPC.ai[AITimer] == 0)
					{
						SetFrameManually(0);
						for (int i = 0; i < 4; ++i)
						{
							bitStates[i] = 3;
							bitTimers[i] = 0;
							bitSprites[i] = 3;
							bitDashProjs[i] = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(), bitPositions[i], Vector2.Zero, ModContent.ProjectileType<Projectiles.Nanako.NanakoBitDash>(), 50, 1, Main.myPlayer)];
						}
					}
					if (NPC.ai[AITimer] % 4 == 0)
					{
						IncrementFrameCounter(2, 8);
					}
					++NPC.ai[AITimer];
					// done
					if (NPC.ai[AITimer] >= 240)
					{
						NPC.ai[AIState] = 1;
					}
					break;
				case 4:
					{
						ChangeSetKeepFrame(700);
						// set bits to circle us
						if (NPC.ai[AITimer] == 0)
						{
							SetFrameManually(0);
							for (int i = 0; i < 7; ++i)
							{
								bitStates[i] = 4;
								bitTimers[i] = 0;
								bitSprites[i] = 0;
								bitRotations[i] = 0;
							}
						}
						if (NPC.ai[AITimer] % 4 == 0)
						{
							IncrementFrameCounter(2, 8);
						}
						++NPC.ai[AITimer];
						if (NPC.ai[AITimer] >= 400)
						{
							for (int i = 0; i < 7; ++i)
							{
								bitStates[i] = 0;
								bitTimers[i] = 0;
								bitSprites[i] = 0;
								bitRotations[i] = 0;
							}
							NPC.ai[AIState] = 1;
						}
					}
					break;
				case 5:
					ChangeSetKeepFrame(700);
					// set our bits to return to us
					if (NPC.ai[AITimer] == 0)
					{
						SetFrameManually(0);
						for (int i = 0; i < 7; ++i)
						{
							bitStates[i] = 6;
							bitTimers[i] = 0;
							bitSprites[i] = 0;
							bitDashProjs[i] = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromThis(), bitPositions[i], Vector2.Zero, ModContent.ProjectileType<Projectiles.Nanako.NanakoBitDash>(), 50, 1, Main.myPlayer)];
						}
						NPC.ai[AITimer] += 1;
					}
					if (generalCounter % 4 == 0)
					{
						IncrementFrameCounter(2, 8);
					}
					bool bitsInPosition = true;
					for (int i = 0; i < 7; ++i)
					{
						if (bitStates[i] != 7 && bitStates[i] != 8)
						{
							bitsInPosition = false;
						}
					}
					if (bitsInPosition)
					{
						if (NPC.ai[AITimer] == 1)
						{
							// play laser sound
							SoundEngine.PlaySound(Acceleration.beamRifleHyperSound, NPC.position);
						}
						++NPC.ai[AITimer];
						// tell the bits to start blasting
						for (int i = 0; i < 7; ++i)
						{
							bitStates[i] = 8;
						}
						// done
						if (NPC.ai[AITimer] >= 400)
						{
							NPC.ai[AIState] = 1;
							for (int i = 0; i < 7; ++i)
							{
								bitStates[i] = 0;
								bitTimers[i] = 0;
								bitSprites[i] = 0;
								bitRotations[i] = 0;
							}
						}
					}
					break;
				case 6:
					{
						ChangeSetKeepFrame(700);
						// set our bits to fire their laser
						if (NPC.ai[AITimer] == 0)
						{
							SetFrameManually(0);
							for (int i = 0; i < 7; ++i)
							{
								bitStates[i] = 9;
								bitTimers[i] = 20 * i;
								bitSprites[i] = 0;
							}
							SoundEngine.PlaySound(Acceleration.hyperSound, NPC.position);
						}
						if (NPC.ai[AITimer] % 4 == 0)
						{
							IncrementFrameCounter(2, 8);
						}
						++NPC.ai[AITimer];
						// done
						if (NPC.ai[AITimer] >= 260)
						{
							NPC.ai[AIState] = 1;
							for (int i = 0; i < 7; ++i)
							{
								bitStates[i] = 0;
								bitSprites[i] = 0;
								bitTimers[i] = 0;
							}
						}
					}
					break;
				case 100:
					if (generalCounter % 2 == 0)
					{
						IncrementFrameCounter(0, 2);
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
				case 50:
					// spawn bits
					if (NPC.ai[AITimer] == 0)
						SoundEngine.PlaySound(Acceleration.hyperSound, NPC.position);
					ChangeSetKeepFrame(600);
					if (NPC.ai[AITimer] % 4 == 0 && NPC.ai[AITimer] != 0)
					{
						IncrementFrameCounter(2, 4);
					}
					if (NPC.ai[AITimer] % 6 == 0)
					{
						int currBit = (int)NPC.ai[AITimer] / 6;
						bitPositions[currBit] = (new Vector2(0, 24.0f).RotatedBy((NPC.ai[AITimer] / 6) * (float)Math.PI / 3.5f)) + NPC.Center;
						bitRotations[currBit] = 0;
						bitStates[currBit] = 0;
						bitTimers[currBit] = 0;
					}
					if (NPC.ai[AITimer] >= 41)
					{
						NPC.ai[AIState] = 1;
						NPC.ai[AITimer] = 0;
					}
					++NPC.ai[AITimer];
					break;
				case 200:
					// simply leave
					IncrementFrameCounter(0, 8);
					ChangeSetKeepFrame(0);
					NPC.velocity.Y -= 0.25f;
					NPC.velocity.Y = Math.Max(-8.0f, NPC.velocity.Y);
					if (NPC.position.Y <= 0)
					{
						NPC.active = false;
					}
					break;
			}

			// update bit AI
			for (int i = 0; i < 7; ++i)
			{
				UpdateBitAI(i);
			}

			++generalCounter;
		}

		void UpdateBitAI(int bit)
		{
			switch (bitStates[bit])
			{
				case 0:
				case 1:
					{
						// just circle around nanako
						float radius = (bitTimers[bit] / 40.0f) * (float)Math.PI;
						if (bitStates[bit] == 0)
						{
							bitPositions[bit] += new Vector2(0, 3.0f).RotatedBy(radius);
						} else
						{
							bitPositions[bit] -= new Vector2(0, 3.0f).RotatedBy(radius);
						}
						// end it
						++bitTimers[bit];
						if (bitTimers[bit] > 80)
						{
							int nextRand = Main.rand.Next(0, 4);
							if (nextRand == 0)
							{
								// return to nanako
								bitStates[bit] = 2;
							} else if (nextRand < 2)
							{
								bitTimers[bit] = 0;
								bitStates[bit] = 0;
							} else
							{
								bitTimers[bit] = 0;
								bitStates[bit] = 1;
							}
						}
						// randomly fire off projectiles
						if (Main.rand.Next(0, 60) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_FromThis(), bitPositions[bit], new Vector2(0, 4).RotatedByRandom(MathF.PI * 2), ModContent.ProjectileType<Projectiles.Nanako.NanakoBitProjectile>(), 30, 1.0f, Main.myPlayer);
						}
					}
					break;
				case 2:
					{
						// target nanako
						Vector2 targetPos = NPC.Center + new Vector2(0, 24.0f).RotatedBy(bit * (float)Math.PI / 3.5f);
						Vector2 targAngle = targetPos - bitPositions[bit];
						targAngle.Normalize();
						targAngle *= 7;
						bitPositions[bit] += targAngle;
						targAngle.Normalize();
						Vector2 dotUse = targetPos - bitPositions[bit];
						dotUse.Normalize();
						if (Matht.DotProduct(targAngle, dotUse) < 0)
						{
							bitStates[bit] = 0;
						}
					}
					break;
				case 3:
					{
						// target a player
						Vector2 targetPos = Main.player[NPC.target].position;
						Vector2 targAngleVec = targetPos - bitPositions[bit];
						float angle = MathF.Atan2(targAngleVec.Y, targAngleVec.X);
						bitRotations[bit] = angle;
						targAngleVec.Normalize();
						targAngleVec *= 6.0f;
						bitPositions[bit] += targAngleVec;
						// animate
						if (bitTimers[bit] % 4 == 0)
						{
							bitSprites[bit] += 1;
							if (bitSprites[bit] >= 6)
							{
								bitSprites[bit] = 3;
							}
						}
						// move our hitbox
						if (Main.netMode != NetmodeID.MultiplayerClient)
							bitDashProjs[bit].Center = bitPositions[bit];
						// done
						if (bitTimers[bit] >= 240)
						{
							bitStates[bit] = 0;
							bitTimers[bit] = 0;
							bitSprites[bit] = 0;
							bitRotations[bit] = 0;
						}
						++bitTimers[bit];
					}
					break;
				case 4:
					{
						// target nanako
						Vector2 targetPos = NPC.Center + new Vector2(0, 24.0f).RotatedBy(bit * (float)Math.PI / 3.5f);
						Vector2 targAngle = targetPos - bitPositions[bit];
						targAngle.Normalize();
						targAngle *= 7;
						bitPositions[bit] += targAngle;
						targAngle.Normalize();
						Vector2 dotUse = targetPos - bitPositions[bit];
						dotUse.Normalize();
						if (Matht.DotProduct(targAngle, dotUse) < 0)
						{
							bitStates[bit] = 5;
							bitRotations[bit] = Main.rand.NextFloat(0, MathF.PI * 2);
						}
					}
					break;
				case 5:
					{
						++bitTimers[bit];
						// open up
						if (bitTimers[bit] <= 8)
						{
							if (bitTimers[bit] % 4 == 0)
							{
								++bitSprites[bit];
							}
						} else
						{
							// spin around
							bitRotations[bit] += 0.07f;
							// fire a shot
							if (bitTimers[bit] % 8 == 0)
							{
								int projType;
								if (bit % 2 == 0)
								{
									projType = ModContent.ProjectileType<Projectiles.Nanako.NanakoBitProjectile>();
								} else
								{
									projType = ModContent.ProjectileType<Projectiles.Nanako.NanakoRoundShot>();
								}
								if (Main.netMode != NetmodeID.MultiplayerClient)
									Projectile.NewProjectile(NPC.GetSource_FromThis(), bitPositions[bit], new Vector2(0, 7.0f).RotatedBy(bitRotations[bit]), projType, 35, 1, Main.myPlayer);

							}

						}
					}
					break;
				case 6:
					{
						// target nanako
						Vector2 targetPos = NPC.Center + new Vector2(0, 24.0f).RotatedBy(bit * (float)Math.PI / 3.5f);
						Vector2 targAngle = targetPos - bitPositions[bit];
						targAngle.Normalize();
						targAngle *= 7;
						bitPositions[bit] += targAngle;
						targAngle.Normalize();
						Vector2 dotUse = targetPos - bitPositions[bit];
						dotUse.Normalize();
						if (Matht.DotProduct(targAngle, dotUse) < 0)
						{
							bitStates[bit] = 7;
							// rotate appropriately
							Vector2 targetRotation = bitPositions[bit] - NPC.Center;
							bitRotations[bit] = MathF.Atan2(targetRotation.Y, targetRotation.X);
						}
					}
					break;
					// sorry, no case 7! they do nothing!
				case 8:
					{
						++bitTimers[bit];
						// open up
						if (bitTimers[bit] <= 8)
						{
							if (bitTimers[bit] % 4 == 0)
							{
								++bitSprites[bit];
							}
						}
						else
						{
							// spin
							bitRotations[bit] += 0.03f;
							// don't try to manipulate projectiles if we're a client, server will handle it
							if (Main.netMode == NetmodeID.MultiplayerClient)
								break;
							// shoot lasers if we're 0, 2, or 5
							if (bit == 2 || bit == 0 || bit == 5)
							{
								Projectile[] bitLaser = null;
								if (bit == 0)
								{
									bitLaser = bitLasers1;
								}
								if (bit == 2)
								{
									bitLaser = bitLasers2;
								}
								if (bit == 5)
								{
									bitLaser = bitLasers3;
								}
								if (bitTimers[bit] == 9)
								{
									// summon our laser
									bitLaser = AccelerationHelper.SummonLaser(ModContent.ProjectileType<Projectiles.Nanako.NanakoBitLaser>(),
										NPC.GetSource_FromThis(),
										bitRotations[bit],
										bitPositions[bit],
										35,
										1,
										40,
										Main.myPlayer,
										true);
									switch (bit)
									{
										case 0:
											bitLasers1 = bitLaser;
											break;
										case 2:
											bitLasers2 = bitLaser;
											break;
										case 5:
											bitLasers3 = bitLaser;
											break;
									}
								}
								// position the laser
								AccelerationHelper.UpdateLaser(bitLaser, bitPositions[bit] + new Vector2(20, 0).RotatedBy(bitRotations[bit]) - new Vector2(20, 20), bitRotations[bit], 40);
							} else
							{
								// just summon some random projectiles
								if (Main.rand.Next(0, 30) == 0)
								{
									Projectile.NewProjectile(NPC.GetSource_FromThis(), bitPositions[bit], new Vector2(0, 4).RotatedByRandom(MathF.PI * 2), ModContent.ProjectileType<Projectiles.Nanako.NanakoBitProjectile>(), 30, 1.0f, Main.myPlayer);
								}
							}
						}
					}
					break;
				case 9:
					{
						// face target
						if (bitTimers[bit] <= 120)
						{
							Vector2 bitAngle = Main.player[NPC.target].position - bitPositions[bit];
							bitRotations[bit] = MathF.Atan2(bitAngle.Y, bitAngle.X);
						}
						// schut
						if (bitTimers[bit] > 120)
						{
							if (bitTimers[bit] >= 128)
							{
								bitSprites[bit] = 2;
							} else if (bitTimers[bit] >= 124)
							{
								bitSprites[bit] = 1;
							}
							if (bitTimers[bit] == 140)
							{
								SoundEngine.PlaySound(Acceleration.beamRifleHyperSound, bitPositions[bit]);
								AccelerationHelper.SummonLaser(ModContent.ProjectileType<Projectiles.Nanako.NanakoHyperLaser>(),
									NPC.GetSource_FromThis(),
									bitRotations[bit],
									(bitPositions[bit] + new Vector2(20, 0).RotatedBy(bitRotations[bit])),
									35,
									1,
									40,
									Main.myPlayer,
									false);
							}
						}
						++bitTimers[bit];
					}
					break;
			}
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
					ChangeSetKeepFrame(900);
					SetFrameManually(0);
				}
				return false;
			}
			NPC.friendly = false;
			NPC.boss = true;
			AccelerateWorld.sakiDefeated = true;
			return base.CheckDead();
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 position, Color drawColor)
		{
			// since bosses use a lot of sprites, this is a custom system set up that spans both the X and Y axis. 100 = go down 1 on the Y axis, 1 = go right 1 on the X axis
			int Y = (int)NPC.frameCounter / 100;
			int X = (int)NPC.frameCounter - (Y * 100);
			Rectangle frame = new Rectangle(X * 66, Y * 66, 64, 64);

			AccelerationHelper.DrawSpriteRect(ModContent.Request<Texture2D>("Acceleration/NPCs/Bosses/NanakoSheet").Value, NPC.Center, frame, drawColor, 0, Vector2.One, spriteBatch, NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

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

			// draw bits
			for (int i = 0; i < 7; ++i)
			{
				AccelerationHelper.DrawSprite("Acceleration/Projectiles/Nanako/NanakoBit", bitPositions[i], bitSprites[i], 32, Color.White, bitRotations[i], Vector2.One, spriteBatch);
			}

			return false;
		}
	}
}
