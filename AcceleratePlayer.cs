using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using Mathj;
using Acceleration.NPCs;
using Acceleration.Misc;


namespace Acceleration
{
	public class AcceleratePlayer : ModPlayer
	{
		static public PlayerStepCallback pstepCallback = new PlayerStepCallback();
		static public PuddingCallback puddingCallback = new PuddingCallback();
		public float heat = 0f;
		public bool accel;
		public bool dashing;
		public bool up;
		public bool down;
		public bool right;
		public bool left;
		public bool prevDashing;
		public float dashDirection;
		public float hyper;
		public int ringSpawn = 0;
		public bool jump;
		public bool prevJump;
		public bool rightClick;
		public bool prevRightClick;
		public bool hyperButton;
		public bool prevHyperButton;
		public int hyperDrawTimer;
		// accelerator values
		public int maxAccelTime;
		public int accelTime;
		// rbits
		public bool rbits;
		public int rbitCooldown;
		public float rbitAngles;
		public int rbitTarget;

		public class PlayerStepCallback : SyncCallback
		{
			public override void Callback(BinaryReader reader)
			{
				int owner = reader.ReadByte();
				AcceleratePlayer ap = Main.player[owner].GetModPlayer<AcceleratePlayer>();
				ap.heat = reader.ReadUInt16();
				ap.dashDirection = reader.ReadSingle();
				ap.dashing = reader.ReadBoolean();
				ap.hyper = reader.ReadUInt16() / 10000;
				ap.accelTime = reader.ReadUInt16();
				// sync player stuff to other players
				if (Main.netMode == NetmodeID.Server)
				{
					ap.SyncStep(ap.player.whoAmI);
				}
			}
		}

		public class PuddingCallback : SyncCallback
		{
			public override void Callback(BinaryReader reader)
			{
				int owner = reader.ReadByte();
				AcceleratePlayer ap = Main.player[owner].GetModPlayer<AcceleratePlayer>();
				ap.rbitAngles = reader.ReadSingle();
				// sync it to other players
				if (Main.netMode == NetmodeID.Server)
				{
					ap.SyncRbits();
				}
			}
		}

		public override void clientClone(ModPlayer clientClone)
		{
			// store values to be synced to clone here
			AcceleratePlayer ap = clientClone as AcceleratePlayer;
			ap.heat = heat;
			ap.dashing = dashing;
			ap.dashDirection = dashDirection;
			ap.hyper = hyper;
		}

		public void SetupHyper()
		{
			hyperDrawTimer = 40;
			Main.PlaySound(Acceleration.hyperSound);
			hyper = Math.Max(0, hyper - 1.0f);
		}

		void SyncStep(int fromwho = -1)
		{
			ModPacket pack = mod.GetPacket();
			pack.Write(pstepCallback.reference);
			pack.Write((byte)player.whoAmI);
			pack.Write((UInt16)heat);
			pack.Write(dashDirection);
			pack.Write(dashing);
			pack.Write((ushort)(hyper * 10000));
			pack.Write((ushort)accelTime);
			pack.Send(-1, fromwho);
		}

		void SyncRbits()
		{
			ModPacket puddingPack = mod.GetPacket();
			puddingPack.Write(puddingCallback.reference);
			puddingPack.Write((byte)player.whoAmI);
			puddingPack.Write(rbitAngles);
			puddingPack.Send(-1, player.whoAmI);
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			/*ModPacket pack = mod.GetPacket(12);
			pack.Write((int)1);
			pack.Write((byte)player.whoAmI);
			pack.Write((UInt16)heat);
			pack.Write(dashDirection);
			pack.Write(dashing);
			pack.Send(toWho, fromWho);*/
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			//Main.NewText("AAAA");
			AcceleratePlayer ap = clientPlayer as AcceleratePlayer;
			// compare clone values and sync them here
			// fuck you i won't compare
			SyncStep(player.whoAmI);
		}

		public override void ResetEffects()
		{
			// make us unable to accelerate...
			accel = false;
			rbits = false;
		}

		// process our input
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (player != Main.LocalPlayer)
			{
				return;
			}
			prevJump = jump;
			jump = triggersSet.Jump;
			up = triggersSet.Up;
			down = triggersSet.Down;
			right = triggersSet.Right;
			left = triggersSet.Left;

			// enable our dash if at the right time
			if (jump && !prevJump && player.velocity.Y != 0.0f && !player.sliding && accelTime < maxAccelTime)
			{
				dashing = true;
				// mild punishment for spam
				accelTime += 20;
				accelTime -= (int)player.wingTime;
				accelTime -= player.rocketTime;
				player.wingTime = 0;
				player.rocketTime = 0;
			}
			else
			{
				// only cancel a dash if we're not pressing the button
				if (dashing)
				{
					++accelTime;
					// end if we've met our acceleration timer
					if (!jump || accelTime >= maxAccelTime)
					{
						dashing = false;
					}
				}
				else
				{
					// reset our acceleration timer
					if (player.velocity.Y == 0)
					{
						accelTime = 0;
					}
					dashing = false;
				}
			}

			prevRightClick = rightClick;
			rightClick = triggersSet.MouseRight;
			prevHyperButton = hyperButton;
			hyperButton = ((Acceleration)mod).hyperKey.Current;
		}

		public void FireRbitShots()
		{
			// only if we're local
			if (player.whoAmI != Main.myPlayer)
			{
				return;
			}
			// fire the shot from our expected rbit positions
			Vector2 rbitPosition = new Vector2(32.0f, 0).RotatedBy(rbitAngles);
			rbitPosition = rbitPosition.RotatedBy(-40 * Matht.Deg2Rad);
			for (int i = 0; i < 8; ++i)
			{
				Vector2 shotSpeed = rbitPosition;
				shotSpeed.Normalize();
				shotSpeed *= 8.0f;
				Projectile.NewProjectile(player.Center + rbitPosition, shotSpeed, ModContent.ProjectileType<Projectiles.Accessories.RbitShot>(), (int)(12.0f * player.minionDamage), 0, player.whoAmI);
				rbitPosition = rbitPosition.RotatedBy(10 * Matht.Deg2Rad);
			}
			rbitCooldown = 90;
		}

		// apply our dash
		public override void PreUpdateMovement()
		{
			if (hyperDrawTimer > 0)
			{
				--hyperDrawTimer;
			}
			if (dashing && accel)
			{
				// if we're not local, just hover
				if (player != Main.LocalPlayer)
				{
					player.velocity = new Vector2(10f * (float)Math.Cos(dashDirection), 10f * (float)Math.Sin(dashDirection));
					// dirty hack because terraria determines whether or not you're grounded entirely by your y velocity for some god forsaken reason
					if (player.velocity.Y == 0.0f)
					{
						player.velocity.Y = 0.00001f;
					}
					if (!prevDashing)
					{
						Main.PlaySound(Acceleration.dashSound, player.position);
					}
					goto DASHEND;
				}
				// if we weren't dashing before, give us speed and a lot of heat
				if (!prevDashing)
				{
					heat += 30;
					// if we're holding neutral, dash in the direction of our speed
					if (!up && !down && !left && !right)
					{
						dashDirection = (float)Math.Atan2(player.velocity.Y, player.velocity.X);
					}
					else
					{
						dashDirection = (float)Math.Atan2((up ? -1 : 0) + (down ? 1 : 0), (right ? 1 : 0) + (left ? -1 : 0));
					}
					Main.PlaySound(Acceleration.dashSound, player.position);
					// force spawn a rainbow colored ring at the start of our dash
					ringSpawn = 12;
				}
				// update dash direction
				// only if you're inputting though
				if (up || down || right || left)
				{
					float tmpDir = (float)Math.Atan2((up ? -1 : 0) + (down ? 1 : 0), (right ? 1 : 0) + (left ? -1 : 0));
					float angBtwn = Matht.AngleBetween(dashDirection * Matht.Rad2Deg, tmpDir * Matht.Rad2Deg) * Matht.Deg2Rad;
					dashDirection += angBtwn > 0 ? Math.Min(2.5f * Matht.Deg2Rad, angBtwn) : Math.Max(-2.5f * Matht.Deg2Rad, angBtwn);
					dashDirection %= 360 * Matht.Deg2Rad;
				}
				player.velocity = new Vector2(10f * (float)Math.Cos(dashDirection), 10f * (float)Math.Sin(dashDirection));
				// dirty hack because terraria determines whether or not you're grounded entirely by your y velocity for some god forsaken reason
				if (player.velocity.Y == 0.0f)
				{
					player.velocity.Y = 0.00001f;
				}
				heat += 0.8f;
				heat = Math.Min(300, heat);
				// handle our rings
				++ringSpawn;
				if (ringSpawn >= 12)
				{
					// try to determine offset for the rainbow ring
					//Vector2 newPos = new Vector2((12f * (float)Math.Cos(dashDirection)) - (24f * (float)Math.Sin(dashDirection)), (24f * (float)Math.Cos(dashDirection)) + (12f * (float)Math.Sin(dashDirection)));
					//int rRing = Projectile.NewProjectile(new Vector2(player.position.X + newPos.X), new Vector2(0, 0), mod.ProjectileType("RainbowRing"), 0, 0, Main.myPlayer);
					//RainbowRing ring = (RainbowRing)Main.projectile[rRing].modProjectile;
					ringSpawn = 0;
					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						ModPacket mp = Acceleration.thisMod.GetPacket();
						// sorta awkward but first 4 is packet id
						mp.Write(RainbowRing.rrc.reference);
						mp.Write((byte)player.whoAmI);
						mp.Send(-1, player.whoAmI);
					}
					RainbowRing.Spawn(player.whoAmI);
					//ring.projectile.rotation = dashDirection;
					ringSpawn = 0;
				}
			} else
			{
				heat -= 0.75f;
				heat = Math.Max(0, heat);
			}
			DASHEND:
			// store that so we can know
			prevDashing = dashing;

			// if we have pudding on and some summon slots, fire boolet
			if (rbits)
			{
				--rbitCooldown;
				if (rbitCooldown <= 0 && player.numMinions > 0 && rbitTarget != -1)
				{
					FireRbitShots();
				}
			}
		}

		// make us take more damage from heat
		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			// a projectile, ignore the damage
			if (dashing && damageSource.SourceProjectileType != 0 && Main.projectile[damageSource.SourceProjectileIndex].magic)
			{
				return false;
			}
			// take more damage from heat
			float defDamage;
			if (Main.expertMode)
			{
				defDamage = damage - (player.statDefense * 0.75f);
			} else
			{
				defDamage = damage - (player.statDefense * 0.5f);
			}
			damage += (int)(defDamage * (heat * 0.01f));
			return true;
		}

		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
			if (hyperDrawTimer > 0)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
				float drawScale0 = ((float)hyperDrawTimer / 40) * 5;
				float drawScale1 = ((1.0f - (float)hyperDrawTimer / 40)) * 6;
				AccelerationHelper.DrawSprite("Sprites/Circle", player.position, 0, 256, new Color(1.0f, 1.0f, 1.0f, 0.5f), 0, new Vector2(drawScale0, drawScale0), null);
				AccelerationHelper.DrawSprite("Sprites/Circle", player.position, 0, 256, new Color(1.0f, 1.0f, 1.0f, 0.5f), 0, new Vector2(drawScale1, drawScale1), null);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin();
			}
			if (rbits)
			{
				// render our rbits from pudding
				Texture2D rbitTex = mod.GetTexture("Projectiles/Accessories/rbit");
				Vector2 rbitPosition = Vector2.Zero;
				if (Main.myPlayer == player.whoAmI)
				{
					if (player.numMinions > 0)
					{
						// find nearest enemy and angle to that
						rbitTarget = AccelerationHelper.FindClosestNPC(player.Center, 600);
						if (rbitTarget != -1)
						{
							NPC targ = Main.npc[rbitTarget];
							Vector2 targDiff = targ.Center - player.Center;
							rbitAngles += Matht.AngleBetween(rbitAngles * Matht.Rad2Deg, (float)Math.Atan2(targDiff.Y, targDiff.X) * Matht.Rad2Deg) * Matht.Deg2Rad * 0.5f;
						}
						rbitPosition = new Vector2(32.0f, 0).RotatedBy(rbitAngles);
					}
					else
					{
						rbitPosition = Main.MouseWorld - player.Center;
						rbitPosition.Normalize();
						rbitPosition *= 32.0f;
						rbitAngles = (float)Math.Atan2(rbitPosition.Y, rbitPosition.X);
					}
					// sync the value
					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						SyncRbits();
					}
				} else
				{
					rbitPosition = new Vector2(32.0f, 0).RotatedBy(rbitAngles);
				}
				rbitPosition = rbitPosition.RotatedBy(-40 * Matht.Deg2Rad);
				for (int i = 0; i < 8; ++i) {
					AccelerationHelper.DrawSpriteCached(rbitTex, player.Center + rbitPosition, 0, 24, Lighting.GetColor((int)(player.Center.X + rbitPosition.X) / 16, (int)(player.Center.Y + rbitPosition.Y) / 16), 0, Vector2.One);
					rbitPosition = rbitPosition.RotatedBy(10 * Matht.Deg2Rad);
				}
			}
		}

		public override void OnRespawn(Player player)
		{
			//AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			heat = 0;
		}
	}
}