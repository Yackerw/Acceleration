using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Acceleration;

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

		public override void AddRecipes()
		{
			ModRecipe currRecipe = new ModRecipe(Acceleration.thisMod);
			currRecipe.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12);
			currRecipe.AddIngredient(ItemID.SilverBar, 20);
			currRecipe.AddTile(TileID.Anvils);
			currRecipe.SetResult(ModContent.ItemType<BeamRifle>());
			currRecipe.AddRecipe();

			currRecipe = new ModRecipe(Acceleration.thisMod);
			currRecipe.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12);
			currRecipe.AddIngredient(ItemID.TungstenBar, 20);
			currRecipe.AddTile(TileID.Anvils);
			currRecipe.SetResult(ModContent.ItemType<BeamRifle>());
			currRecipe.AddRecipe();
		}

		public override void SetDefaults()
		{
			item.damage = 25;
			item.magic = true;
			item.mana = 10;
			item.width = 26;
			item.height = 26;
			item.useTime = 27;
			item.useAnimation = 27;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true;
			item.channel = true; //Channel so that you can held the weapon [Important]
			item.knockBack = 6;
			item.value = Item.sellPrice(silver: 50);
			item.rare = ItemRarityID.Orange;
			item.UseSound = SoundID.Item25;
			item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Magic.FriendlySakiOrb>();
			item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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
			return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}

		public override void HoldItem(Player player)
		{
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			if (ap.rightClick && !ap.prevRightClick && player.reuseDelay <= 0)
			{
				// upon release, fire either a normal shot or charge shot depending on how long we've charged
				if (player == Main.LocalPlayer && player.CheckMana(player.GetManaCost(item), true))
				{
					float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
					// shoot here
					// spawn 3 orange 3 blue projectiles
					int projType = ModContent.ProjectileType<Projectiles.Weapons.Magic.FriendlySakiPellet>();
					Projectile proj = Main.projectile[Projectile.NewProjectile(player.position, Vector2.Zero, projType, (int)(item.damage * player.magicDamage), item.knockBack, player.whoAmI)];
					proj.rotation = shotAngle;
					proj = Main.projectile[Projectile.NewProjectile(player.position, Vector2.Zero, projType, (int)(item.damage * player.magicDamage), item.knockBack, player.whoAmI)];
					proj.rotation = shotAngle + 17 * Mathj.Matht.Deg2Rad;
					proj = Main.projectile[Projectile.NewProjectile(player.position, Vector2.Zero, projType, (int)(item.damage * player.magicDamage), item.knockBack, player.whoAmI)];
					proj.rotation = shotAngle - 17 * Mathj.Matht.Deg2Rad;
					proj = Main.projectile[Projectile.NewProjectile(player.position, Vector2.Zero, projType, (int)(item.damage * player.magicDamage), item.knockBack, player.whoAmI, 1)];
					proj.rotation = shotAngle - 2 * Mathj.Matht.Deg2Rad;
					proj = Main.projectile[Projectile.NewProjectile(player.position, Vector2.Zero, projType, (int)(item.damage * player.magicDamage), item.knockBack, player.whoAmI, 1)];
					proj.rotation = shotAngle + 15 * Mathj.Matht.Deg2Rad;
					proj = Main.projectile[Projectile.NewProjectile(player.position, Vector2.Zero, projType, (int)(item.damage * player.magicDamage), item.knockBack, player.whoAmI, 1)];
					proj.rotation = shotAngle - 19 * Mathj.Matht.Deg2Rad;
					Main.PlaySound(item.UseSound, player.position);
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
				player.reuseDelay = 1;
				player.itemAnimation = 1;
				player.itemAnimationMax = 1;
				// actually fire the projectile
				if (hyperTimer == 60)
				{
					Projectile.NewProjectile(player.Center - new Vector2(0, 160), Vector2.Zero, ModContent.ProjectileType<Projectiles.Weapons.Magic.FriendlyBigBangBell>(), (int)(item.damage * player.magicDamage), item.knockBack * 0.5f, player.whoAmI);
				}
			}

			base.HoldItem(player);
		}
	}
}