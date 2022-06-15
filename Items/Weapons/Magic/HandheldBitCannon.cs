using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Acceleration.Items.Weapons.Magic
{
	class HandheldBitCannon : ModItem
	{
		bool hyper = false;
		int hyperTimer = 0;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Fires a laser"
				+ "\nRight click: charge shot"
				+ "\nHYPER: Big laser");
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12)
				.AddIngredient(ItemID.SilverBar, 20)
				.Register();
			CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12)
				.AddIngredient(ItemID.TungstenBar, 20)
				.Register();
		}

		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 4;
			Item.width = 64;
			Item.height = 32;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.channel = true; //Channel so that you can held the weapon [Important]
			Item.knockBack = 3;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = Acceleration.BeamRifleSound;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.PreHardMode.FriendlyNanakoRoundShot>();
			Item.shootSpeed = 10f;
			Item.autoReuse = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (hyper)
			{
				if (hyperTimer <= 0)
				{
					hyper = false;
				}
				return false;
			}
			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			// add a little bit of inaccuracy
			velocity = velocity.RotatedByRandom(0.2f);
		}

		public override void HoldItem(Player player)
		{
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();

			if (ap.hyperButton && !ap.prevHyperButton && player.reuseDelay <= 0 && ap.hyper >= 1.0f)
			{
				hyperTimer = 70;
				hyper = true;
				//Projectile.NewProjectile
				// um, try rendering, i guess
				ap.SetupHyper();
			}
			if (hyper)
			{
				--hyperTimer;
				player.reuseDelay = 2;
				player.itemAnimation = 2;
				player.itemAnimationMax = 2;
				if (hyperTimer <= 0)
				{
					hyper = false;
				}
				// actually fire the projectile
				if (hyperTimer == 25)
				{
					SoundEngine.PlaySound(Acceleration.beamRifleHyperSound, player.position);
					float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
					AccelerationHelper.SummonLaser(ModContent.ProjectileType<Projectiles.Weapons.Magic.PreHardMode.FriendlyNanakoLaser>(),
						player.GetSource_ItemUse(Item),
						shotAngle,
						player.Center,
						(int)(Item.damage),
						Item.knockBack,
						40,
						player.whoAmI,
						false);
				}
			}

			base.HoldItem(player);
		}
	}
}
