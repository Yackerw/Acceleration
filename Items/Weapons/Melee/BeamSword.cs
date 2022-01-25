using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Acceleration.Items.Weapons.Melee
{
	class BeamSword : ModItem
	{
		int swingAnim = 0;
		int swingTimer = 0;
		public override void SetDefaults()
		{
			item.damage = 30;
			item.melee = true;
			item.width = 48;
			item.height = 48;
			item.useTime = 25;
			item.useAnimation = 25;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noMelee = true;
			item.knockBack = 0;
			item.value = Item.sellPrice(silver: 50);
			item.rare = ItemRarityID.Orange;
			item.shoot = ModContent.ProjectileType<Projectiles.BeamSword>();
			item.shootSpeed = 10f;
			item.noUseGraphic = true;
			item.channel = false;
		}

		int hyperTimer = 0;
		bool hyper;

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (hyper)
			{
				return false;
			}
			float mouseDir = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
			int dmg = item.damage;
			if (swingAnim == 2)
			{
				dmg = 40;
			}
			Terraria.Audio.LegacySoundStyle soundToPlay;
			switch (swingAnim)
			{
				case 1:
					soundToPlay = Acceleration.sword3Sound;
					break;
				default:
					soundToPlay = Acceleration.sword2Sound;
					break;
			}
			Main.PlaySound(soundToPlay, player.position);
			Projectile.NewProjectile(player.position, new Vector2(0, 0), ModContent.ProjectileType<Projectiles.BeamSword>(), (int)(dmg * player.meleeDamageMult), item.knockBack, player.whoAmI, swingAnim + (item.useTime << 2), mouseDir);
			++swingAnim;
			if (swingAnim == 3)
			{
				swingAnim = 0;
			}
			swingTimer = 50;
			return false;
		}

		public override void HoldItem(Player player)
		{
			base.HoldItem(player);
			if (swingTimer > 0)
			{
				--swingTimer;
				if (swingTimer <= 0)
				{
					swingAnim = 0;
				}
			}
			// hyper
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			if (ap.hyperButton && !ap.prevHyperButton && player.reuseDelay <= 0)
			{
				ap.SetupHyper();
				hyperTimer = 25;
				hyper = true;
			}
			if (hyper)
			{
				--hyperTimer;
				if (hyperTimer <= 0)
				{
					hyper = false;
					player.velocity = new Vector2(0, 0);
					return;
				}

				if (hyperTimer == 7)
				{
					// play sfx
					Main.PlaySound(Acceleration.swordHyperSound, player.position);
					player.velocity = new Vector2(45, 0).RotatedBy(AccelerationHelper.GetMouseRotation(player));
					// give us some i frames
					player.immune = true;
					player.immuneTime = 60;
					for (int i = 0; i < player.hurtCooldowns.Length; ++i) {
						player.hurtCooldowns[i] = 60;
					}
				}
				// BOOM! accelerate!
				if (hyperTimer <= 7)
				{
					ap.dashing = false;
					// play a bunch of particles
					Vector2 particleSpeed = new Vector2(6, 0).RotatedBy(Math.Atan2(player.velocity.Y, player.velocity.X) + (Math.PI / 2));
					Vector2 invertPSpeed = -particleSpeed;
					Vector2 particlePos = player.position;
					for (int i = 0; i < 5; ++i)
					{
						bool updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.TopazBolt, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.SapphireBolt, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.RubyBolt, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.EmeraldBolt, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.AmethystBolt, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						particlePos += player.velocity / 5.0f;
					}
					// spawn the hurty
					Projectile.NewProjectile(player.position + player.velocity / 2, new Vector2(0, 0), ModContent.ProjectileType<Projectiles.SwordHitbox>(), (int)(120 * player.meleeDamageMult), item.knockBack, player.whoAmI, 0, 4);
				}

			}
		}
	}
}
