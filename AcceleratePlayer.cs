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


namespace Acceleration
{
	public class AcceleratePlayer : ModPlayer
	{
		static public PlayerStepCallback pstepCallback = new PlayerStepCallback();
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
					} else
					{
						RainbowRing.Spawn(player.whoAmI);
					}
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
				AccelerationHelper.DrawSprite("Sprites/Circle", player.position, 0, 256, new Color(1.0f, 1.0f, 1.0f, 0.5f), 0, null, ((float)hyperDrawTimer / 40) * 5);
				AccelerationHelper.DrawSprite("Sprites/Circle", player.position, 0, 256, new Color(1.0f, 1.0f, 1.0f, 0.5f), 0, null, ((1.0f - (float)hyperDrawTimer / 40)) * 6);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin();
			}
		}

		public override void OnRespawn(Player player)
		{
			//AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			heat = 0;
		}
	}
}