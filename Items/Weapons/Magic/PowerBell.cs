using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Acceleration;
using Terraria.DataStructures;
using Terraria.Audio;

namespace Acceleration.Items.Weapons.Magic
{
	class PowerBell : ModItem
	{

		bool altFire;
		bool hyper = false;
		int hyperTimer = 0;
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Fires a homing projectile"
				+ "\nRight click: fires a spread of projectiles"
				+ "\nHYPER: Big Bang Bell:" +
				"\nA large homing projectile");
		}

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 26;
			Item.height = 26;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.noMelee = true;
			Item.channel = true; //Channel so that you can held the weapon [Important]
			Item.knockBack = 6;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item25;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.FriendlySakiOrb>();
			Item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// dirty hack to prevent charge shot firing something
			if (altFire)
			{
				altFire = false;
				return false;
			}
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

		public override void HoldItem(Player player)
		{
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			if (ap.rightClick && !ap.prevRightClick && player.reuseDelay <= 0)
			{
				// upon release, fire either a normal shot or charge shot depending on how long we've charged
				if (player == Main.LocalPlayer && player.CheckMana(player.GetManaCost(Item), true))
				{
					float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
					// shoot here
					// spawn 3 orange 3 blue projectiles
					int projType = ModContent.ProjectileType<Projectiles.Weapons.Magic.FriendlySakiPellet>();
					Projectile proj = Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(1, 0).RotatedBy(shotAngle), projType, (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), Item.knockBack, player.whoAmI)];
					proj = Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(1, 0).RotatedBy(shotAngle + 17 * Mathj.Matht.Deg2Rad), projType, (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), Item.knockBack, player.whoAmI)];
					proj = Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(1, 0).RotatedBy(shotAngle - 17 * Mathj.Matht.Deg2Rad), projType, (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), Item.knockBack, player.whoAmI)];
					proj = Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(1, 0).RotatedBy(shotAngle - 2 * Mathj.Matht.Deg2Rad), projType, (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), Item.knockBack, player.whoAmI, 1)];
					proj = Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(1, 0).RotatedBy(shotAngle + 15 * Mathj.Matht.Deg2Rad), projType, (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), Item.knockBack, player.whoAmI, 1)];
					proj = Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(1, 0).RotatedBy(shotAngle - 19 * Mathj.Matht.Deg2Rad), projType, (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), Item.knockBack, player.whoAmI, 1)];
					SoundEngine.PlaySound(Item.UseSound, player.position);
					player.itemAnimation = 27;
					player.itemAnimationMax = 27;
					player.reuseDelay = 27;
					altFire = true;
				}
			}

			if (ap.hyperButton && !ap.prevHyperButton && player.reuseDelay <= 0 && ap.hyper >= 1.0f)
			{
				hyperTimer = 70;
				hyper = true;
				ap.SetupHyper();
			}
			if (hyper)
			{
				--hyperTimer;
				if (hyperTimer <= 0)
				{
					hyper = false;
				}
				player.reuseDelay = 1;
				player.itemAnimation = 1;
				player.itemAnimationMax = 1;
				// actually fire the projectile
				if (hyperTimer == 60)
				{
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center - new Vector2(0, 160), Vector2.Zero, ModContent.ProjectileType<Projectiles.Weapons.Magic.FriendlyBigBangBell>(), (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), Item.knockBack * 0.5f, player.whoAmI);
				}
			}

			base.HoldItem(player);
		}
	}
}