using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Mathj;
using System;
using Terraria.DataStructures;

namespace Acceleration.Items.Weapons.Ranged
{
	class Maracca : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Throws maraccas at foes"
				+ "\nHYPER: Throws lots at once");
		}

		public override void SetDefaults()
		{
			Item.damage = 22;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 26;
			Item.height = 26;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.channel = true; //Channel so that you can held the weapon [Important]
			Item.knockBack = 6;
			Item.value = Item.sellPrice(copper: 50);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Ranged.FriendlyMaracca>();
			Item.shootSpeed = 8.0f;
			Item.noUseGraphic = true;
			Item.maxStack = 999;
			Item.consumable = true;
		}

		public bool hyper;
		public int hyperTimer;

		public override void HoldItem(Player player)
		{
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			if (!hyper && ap.hyperButton && !ap.prevHyperButton && ap.hyper >= 1.0f && player.reuseDelay <= 0)
			{
				hyper = true;
				hyperTimer = 70;
				ap.SetupHyper();
			}
			if (hyper && hyperTimer > 50)
			{
				// get where we're aiming
				Vector2 projectileSpeed = Main.MouseWorld - player.Center;
				projectileSpeed.Normalize();
				projectileSpeed *= 8.0f;
				projectileSpeed = projectileSpeed.RotateRandom(12.0f * Matht.Deg2Rad);
				Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, projectileSpeed, ModContent.ProjectileType<Projectiles.Weapons.Ranged.FriendlyMaracca>(), (int)(Item.damage * player.GetTotalDamage(DamageClass.Ranged).Multiplicative), Item.knockBack, player.whoAmI);
			}
			if (hyper)
			{
				--hyperTimer;
				if (hyperTimer <= 0)
				{
					hyper = false;
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			return hyper ? false : base.CanUseItem(player);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// no shooty!
			if (hyper)
			{
				if (hyperTimer <= 0)
				{
					hyper = false;
				}
				return false;
			}
			return true;
		}

	}
}
