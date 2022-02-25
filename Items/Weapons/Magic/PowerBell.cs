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
			item.UseSound = Acceleration.BeamRifleSound;
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
					player.itemAnimation = 25;
					player.itemAnimationMax = 25;
					player.reuseDelay = 25;
					item.useTime = 25;
					item.useAnimation = 25;
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
				if (hyperTimer == 25)
				{
					Main.PlaySound(Acceleration.beamRifleHyperSound, player.position);
					float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
					Projectile projectile = Main.projectile[Projectile.NewProjectile(player.position + new Vector2(40, 0).RotatedBy(shotAngle), new Vector2(0, 0), ModContent.ProjectileType<Projectiles.BeamHyper>(), (int)(item.damage * 0.666f * player.magicDamageMult), 1.0f, player.whoAmI, 0, shotAngle)];
					// spawn the rest of them...
					// draw a good number of projectiles
					Vector2 spawnPos = projectile.Center;
					int projToSpawn = ModContent.ProjectileType<Projectiles.BeamHyper>();
					Vector2 normal = new Vector2(1, 0).RotatedBy(shotAngle);
					Vector2 spawnIncrement = normal * 40;
					for (int i = 0; i < 40; ++i)
					{
						Vector2 moveAmnt = Collision.TileCollision(spawnPos, spawnIncrement, 40, 40, false, false, 0);
						if (moveAmnt.X != spawnIncrement.X || moveAmnt.Y != spawnIncrement.Y)
						{
							spawnPos += moveAmnt;
							break;
						}
						spawnPos += spawnIncrement;
						Projectile proj = Main.projectile[Projectile.NewProjectile(spawnPos, new Vector2(0, 0), projToSpawn, projectile.damage, projectile.knockBack, projectile.owner, 2 + ((i * 2) % 4 <= 1 ? 1 : 0), projectile.ai[1])];
						proj.alpha = projectile.alpha;
						// hack for ordering...
						if (proj.whoAmI < projectile.whoAmI)
						{
							proj.timeLeft -= 1;
						}
					}
					Projectile explodeProj = Main.projectile[Projectile.NewProjectile(spawnPos, new Vector2(0, 0), projToSpawn, projectile.damage, projectile.knockBack, projectile.owner, 4, 0)];
					if (explodeProj.whoAmI < projectile.whoAmI)
					{
						explodeProj.timeLeft -= 1;
					}
				}
			}

			base.HoldItem(player);
		}
	}
}