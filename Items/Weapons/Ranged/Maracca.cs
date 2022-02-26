using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Mathj;
using System;

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
			item.damage = 22;
			item.ranged = true;
			item.width = 26;
			item.height = 26;
			item.useTime = 12;
			item.useAnimation = 12;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noMelee = true;
			item.channel = true; //Channel so that you can held the weapon [Important]
			item.knockBack = 6;
			item.value = Item.sellPrice(copper: 50);
			item.rare = ItemRarityID.Orange;
			item.UseSound = SoundID.Item1;
			item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Ranged.FriendlyMaracca>();
			item.shootSpeed = 8.0f;
			item.noUseGraphic = true;
			item.maxStack = 999;
			item.consumable = true;
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
				Projectile.NewProjectile(player.Center, projectileSpeed, ModContent.ProjectileType<Projectiles.Weapons.Ranged.FriendlyMaracca>(), (int)(item.damage * player.rangedDamage), item.knockBack, player.whoAmI);
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

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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
