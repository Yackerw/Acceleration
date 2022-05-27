using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Acceleration.Items.Weapons.Melee
{
	class BeamSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("3 hit combo" +
				"\nGives invuln on hit"
				+ "\nHYPER: Super speed dash attack");
		}
		public class BeamSwordCallback : SyncCallback
		{
			public static void Callback(BinaryReader reader)
			{
				int whom = reader.ReadByte();
				if (Main.player[whom].HeldItem.type == ModContent.ItemType<BeamSword>())
				{
					BeamSword bm = (BeamSword)Main.player[whom].HeldItem.ModItem;
					bm.hyperTimer = 25;
					bm.hyper = true;
				}
			}
		}
		int swingAnim = 0;
		int swingTimer = 0;
		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = DamageClass.Melee;
			Item.width = 48;
			Item.height = 48;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 4;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<Projectiles.BeamSwordProjectile>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.channel = false;
		}

		int hyperTimer = 0;
		bool hyper;
		public int swingInvulnDelay;

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12)
				.AddIngredient(ItemID.SilverBar, 10)
				.AddIngredient(ItemID.Ruby, 5)
				.Register();
			CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12)
				.AddIngredient(ItemID.TungstenBar, 10)
				.AddIngredient(ItemID.Ruby, 5)
				.Register();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (hyper)
			{
				return false;
			}
			float mouseDir = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
			float dmg = Item.damage;
			if (swingAnim == 2)
			{
				dmg *= 1.333f;
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
			SoundEngine.PlaySound(soundToPlay, player.position);
			Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem),player.position, new Vector2(0, 0), ModContent.ProjectileType<Projectiles.BeamSwordProjectile>(), (int)(dmg * player.GetDamage(DamageClass.Melee).Multiplicative), Item.knockBack * (swingAnim == 2 ? 2.5f : 1), player.whoAmI, swingAnim + ((int)(Item.useTime * player.GetTotalAttackSpeed(DamageClass.Melee)) << 2), mouseDir);
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
			if (swingInvulnDelay > 0)
			{
				--swingInvulnDelay;
			}
			// hyper
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			if (ap.hyperButton && !ap.prevHyperButton && player.reuseDelay <= 0 && ap.hyper >= 1.0f && !hyper)
			{
				ap.SetupHyper();
				hyperTimer = 25;
				player.reuseDelay = 25;
				hyper = true;
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					BeamSwordCallback bsc = new BeamSwordCallback();
					ModPacket mp = bsc.GetPacket();
					mp.Write((byte)player.whoAmI);
					//mp.Send(-1, Main.myPlayer);
					bsc.SendPacketRelayed(mp);
				}

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
					SoundEngine.PlaySound(Acceleration.swordHyperSound, player.position);
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
						Dust.NewDust(particlePos, 20, 20, DustID.GemTopaz, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.GemSapphire, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.GemRuby, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.GemEmerald, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						updown = Main.rand.NextBool();
						Dust.NewDust(particlePos, 20, 20, DustID.GemAmethyst, updown ? particleSpeed.X : invertPSpeed.X, updown ? particleSpeed.Y : invertPSpeed.Y);
						particlePos += player.velocity / 5.0f;
					}
					// spawn the hurty
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem),player.position + player.velocity / 2, new Vector2(0, 0), ModContent.ProjectileType<Projectiles.SwordHitbox>(), (int)(Item.damage * 4 * player.GetTotalDamage(DamageClass.Melee).Multiplicative), Item.knockBack, player.whoAmI, 0, 4);
				}

			}
		}
	}
}
