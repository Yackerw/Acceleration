using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Mathj;
using System;
using Terraria.DataStructures;

namespace Acceleration.Items.Weapons.Ranged
{
	class MachineGun : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("20% chance not to use ammo" +
				"\nFires rapid fire bullets"
				+ "\nHYPER: Rings of bullets");
		}

		public override void SetDefaults() {
			Item.damage = 9;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 26;
			Item.height = 26;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.channel = true; //Channel so that you can held the weapon [Important]
			Item.knockBack = 6;
			Item.value = Item.sellPrice(gold : 5);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item11;
			Item.shoot = ModContent.ProjectileType<Projectiles.SuguriBullet>();
			Item.autoReuse = true;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override bool CanConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= 0.2f;
		}

		public bool hyper;
		public int hyperTimer;

		public override void HoldItem(Player player)
		{
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			if (!hyper && ap.hyperButton && !ap.prevHyperButton && ap.hyper > 1.0f && player.reuseDelay <= 0)
			{
				hyper = true;
				hyperTimer = 60;
				ap.SetupHyper();
			}
			if (hyper && hyperTimer > 0)
			{
				--hyperTimer;
				player.reuseDelay = 1;
				player.itemAnimation = 1;
				player.itemAnimationMax = 1;
				// just spawn a whole buncha bullets
				float bulletAngle = (hyperTimer / 60.0f) * (float)Math.PI * 2;
				Vector2 spawnPos = new Vector2(50, 0).RotatedBy(bulletAngle);
				Vector2 spawnVel = new Vector2(8, 0).RotatedBy(bulletAngle);
				Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position + spawnPos, spawnVel, ModContent.ProjectileType<Projectiles.SuguriBullet>(), (int)(Item.damage * 2.75f * player.GetTotalDamage(DamageClass.Ranged).Multiplicative), Item.knockBack, player.whoAmI, -1);
				Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position - spawnPos, -spawnVel, ModContent.ProjectileType<Projectiles.SuguriBullet>(), (int)(Item.damage * 2.75f * player.GetTotalDamage(DamageClass.Ranged).Multiplicative), Item.knockBack, player.whoAmI, -1);
				SoundEngine.PlaySound(SoundID.Item11, player.position);
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12)
				.AddIngredient(ItemID.IllegalGunParts)
				.Register();
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
			Vector2 sp = velocity.RotatedByRandom(15.0f * Matht.Deg2Rad);

			int projToFire = 0;
			float speed = 8.0f;
			bool canShoot = false;
			int dmg = Item.damage;
			float kBack = 0;
			int ammoID;
			player.PickAmmo(Item, ref projToFire, ref speed, ref canShoot, ref dmg, ref kBack, out ammoID, true);
			float angle = (float)Math.Atan2(sp.Y, sp.X);
			// spawn boolet
			int proj = Projectile.NewProjectile(player.GetSource_ItemUse(Item) ,position, new Vector2(speed, 0).RotatedBy(angle), ModContent.ProjectileType<Projectiles.SuguriBullet>(), dmg, kBack, player.whoAmI, projToFire);
			Projectile realProj = Main.projectile[proj];
			if (projToFire == ProjectileID.MeteorShot)
			{
				realProj.penetrate = 2;
			}
			return false;
		}

	}
}
