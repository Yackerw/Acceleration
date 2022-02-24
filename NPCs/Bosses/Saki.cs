﻿using System;
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
using Mathj;

namespace Acceleration.NPCs.Bosses
{
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
			NPCID.Sets.MustAlwaysDraw[npc.type] = true;
		}

		public override void SetDefaults()
		{
			npc.lifeMax = 2700;
			npc.damage = 15;
			npc.defense = 6;
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			npc.noGravity = true;
			npc.width = 20;
			npc.height = 47;
			npc.friendly = false;
			npc.noTileCollide = true;
			npc.boss = true;
			npc.npcSlots = 50;
			music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/MigratoryBirdFromNorth");
			musicPriority = MusicPriority.BossLow;
			npc.ai[AINextState] = 1;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = 2700 + 400 + (800 * numPlayers);
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
			int Y = (int)npc.frameCounter / 100;
			int X = (int)npc.frameCounter - (Y * 100);
			++X;
			X = X % frameLength;
			npc.frameCounter = X + (Y * 100);
		}

		void ChangeSetKeepFrame(int set)
		{
			int Y = (int)npc.frameCounter / 100;
			int X = (int)npc.frameCounter - (Y * 100);
			npc.frameCounter = set + X;
		}

		void SetFrameManually(int frame)
		{
			int Y = (int)npc.frameCounter / 100;
			npc.frameCounter = Y + frame;
		}

		public override void AI()
		{
			if (spawnPoint.Y == 0)
			{
				// set this so we can keep the npc around where it started
				spawnPoint = npc.position;
			}
			// handle aggro
			Player target = Main.player[npc.target];
			if (npc.target < 0 || npc.target == 255 || target.dead || !target.active)
			{
				npc.TargetClosest(true);
				target = Main.player[npc.target];
			}
			// leave if all targets dead/too far away
			if (target.dead || Matht.Magnitude(target.position - npc.position) > 4000)
			{
				if (npc.ai[AIState] != 200)
				{
					Main.NewText("Saki has defeated all players!", Color.Green, true);
					npc.ai[AIState] = 200;
				}
			}
			Vector2 targDiff = target.position - npc.position;
			switch (npc.ai[AIState])
			{
				case 0:
					// wait a little while
					--npc.ai[AITimer];
					if (npc.ai[AITimer] <= 0)
					{
						npc.ai[AIState] = 1;
					}
					if (generalCounter % 3 == 0)
					{
						IncrementFrameCounter(8);
					}
					break;
				case 1:
					// prepare to fly to a specific spot, and set up our next state
					npc.ai[AINextState] = Main.rand.Next(0, 4);
					switch (npc.ai[AINextState])
					{
						case 1:
							// maraccas
							npc.ai[AINextState] = 3;
							break;
						case 2:
							// semi-circular shots
							npc.ai[AINextState] = 4;
							break;
						default:
							npc.ai[AINextState] = 0;
							break;
					}
					// set us to fly
					npc.ai[AIState] = 2;
					//npc.frameCounter = 200;
					// set our position
					targetPosition = new Vector2(Main.rand.NextFloat(-300, 300), Main.rand.NextFloat(-300, 300));
					// ensure it stays around the spawn point enough...and don't go beneath the player (unless they're above our spawn point)
					// try to move towards player if we're too far away
					while ((npc.position.Y + targetPosition.Y) > target.position.Y || (Matht.Magnitude((npc.position + targetPosition) - target.position) > 300 && Matht.DotProduct(targetPosition, target.position - npc.position) < 0.1f))
					//while (Matht.Magnitude((npc.position + targetPosition) - spawnPoint) > 800 || ((npc.position.Y + targetPosition.Y) > target.position.Y && target.position.Y > spawnPoint.Y) || npc.position.Y + targetPosition.Y > spawnPoint.Y)
					{
						targetPosition = new Vector2(Main.rand.NextFloat(-300, 300), Main.rand.NextFloat(-300, 300));
					}
					targetPosition += npc.position;
					break;
				case 2:
					// travel to target position, unless we already there
					Vector2 posToMove = targetPosition - npc.position;
					posToMove.Normalize();
					posToMove *= 3.0f;
					npc.position += posToMove;
					Vector2 newPosToMove = targetPosition - npc.position;
					// face the player
					if (targDiff.X > 0)
					{
						npc.spriteDirection = 1;
					} else
					{
						npc.spriteDirection = -1;
					}
					if (posToMove.X > 0)
					{
						if (npc.spriteDirection == 1)
						{
							ChangeSetKeepFrame(200);
						}
						else
						{
							ChangeSetKeepFrame(100);
						}
					} else
					{
						if (npc.spriteDirection == 1)
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
					if (generalCounter % 45 == 0)
					{
						switch (Main.rand.Next(0, 3))
						{
							case 0:
								Vector2 tambSpeed = targDiff;
								tambSpeed.Normalize();
								tambSpeed *= 1.5f;
								Projectile.NewProjectile(npc.position, tambSpeed, ModContent.ProjectileType<SakiTambourine>(), 15, 1.0f);
								break;
							default:
								// this is dumb why is ai0 not working
								int proj = Projectile.NewProjectile(npc.position, new Vector2(0, 6).RotatedByRandom(360*Matht.Deg2Rad), ModContent.ProjectileType<SakiOrb>(), 15, 1.0f, 255, npc.whoAmI, target.whoAmI);
								Projectile newProj = Main.projectile[proj];
								newProj.ai[0] = npc.whoAmI;
								break;
						}
					}

					if (Matht.DotProduct(posToMove, newPosToMove) <= 0)
					{
						npc.ai[AIState] = npc.ai[AINextState];
						if (npc.ai[AIState] == 1) {
							npc.ai[AINextState] = 2;
							npc.ai[AITimer] = 0;
						} else if (npc.ai[AIState] == 3 || npc.ai[AIState] == 4)
						{
							npc.ai[AITimer] = 160;
						}
						else
						{
							npc.ai[AINextState] = 0;
						}
					}

					break;
				case 3:
					npc.frameCounter = Math.Min(((160 - (int)npc.ai[AITimer]) / 5), 11);
					ChangeSetKeepFrame(500);
					--npc.ai[AITimer];
					if (npc.ai[AITimer] <= 135 && npc.ai[AITimer] >= 125)
					{
						// spawn maraccas in random directions
						Projectile.NewProjectile(npc.position, new Vector2(Main.rand.NextFloat(-6.0f, 6.0f), Main.rand.NextFloat(-6.0f, 0.0f)), ModContent.ProjectileType<SakiMaracca>(), 13, 3.0f);
					}
					if (npc.ai[AITimer] <= 0)
					{
						npc.ai[AIState] = 1;
					}
					break;
				case 4:
					npc.frameCounter = Math.Min(((160 - (int)npc.ai[AITimer]) / 5), 11);
					ChangeSetKeepFrame(500);
					--npc.ai[AITimer];
					if (npc.ai[AITimer] <= 135 && npc.ai[AITimer] >= 115)
					{
						// pellets in a rotational pattern...
						if (npc.ai[AITimer] % 2 == 1)
						{
							float rotationalAdditive = (npc.ai[AITimer] - 125) * 1.7f * Matht.Deg2Rad;
							for (int i = 0; i < 5; ++i)
							{
								float rotationalBase = i * 72 * Matht.Deg2Rad;
								Projectile proj = Main.projectile[Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<SakiPellet>(), 13, 3.0f, 255, 0, 0)];
								proj.rotation = rotationalBase + rotationalAdditive;
								// minus a little for blue
								proj = Main.projectile[Projectile.NewProjectile(npc.position, Vector2.Zero, ModContent.ProjectileType<SakiPellet>(), 13, 3.0f, 255, 1, 1)];
								proj.rotation = rotationalBase + rotationalAdditive - (3 * Matht.Deg2Rad);
								proj.ai[1] = 1;
							}
						}
					}
					if (npc.ai[AITimer] <= 0)
					{
						npc.ai[AIState] = 1;
					}
					break;
				case 100:
					if (generalCounter % 2 == 0)
					{
						IncrementFrameCounter(2);
					}
					--npc.ai[AITimer];
					if (npc.ai[AITimer] <= 0)
					{
						npc.ai[AIState] = 101;
						npc.life = 0;
						npc.realLife = 0;
						// TODO: particle effects, sfx for death
					}
					break;
				case 200:
					// simply leave
					IncrementFrameCounter(8);
					ChangeSetKeepFrame(0);
					npc.velocity.Y -= 0.25f;
					npc.velocity.Y = Math.Max(-8.0f, npc.velocity.Y);
					if (npc.position.Y <= 0)
					{
						npc.active = false;
					}
					break;
			}

			++generalCounter;
		}

		public override bool CheckDead()
		{
			if (npc.ai[AIState] != 101)
			{
				if (npc.ai[AIState] != 100)
				{
					npc.ai[AIState] = 100;
					npc.ai[AITimer] = 140;
					npc.frameCounter = 1000;
					npc.life = 1000;
					npc.realLife = 1000;
					npc.friendly = true;
				}
				return false;
			}
			npc.friendly = false;
			npc.boss = true;
			return base.CheckDead();
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			int Y = (int)npc.frameCounter / 100;
			int X = (int)npc.frameCounter - (Y * 100);
			Rectangle frame = new Rectangle(X * 66, Y * 66, 64, 64);

			AccelerationHelper.DrawSpriteRect(ModContent.GetTexture("Acceleration/NPCs/Bosses/SakiSheet"), npc.Center, frame, drawColor, 0, Vector2.One, spriteBatch, npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

			return false;
		}
	}
}