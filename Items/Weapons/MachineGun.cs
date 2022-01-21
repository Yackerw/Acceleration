using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Mathj;
using System;

namespace Acceleration.Items.Weapons
{
	class MachineGun : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("20% chance not to use ammo" +
				"\nFires rapid fire bullets"
				+ "\nHYPER: big laser");
		}

		public override void SetDefaults() {
			item.damage = 9;
			item.ranged = true;
			item.width = 26;
			item.height = 26;
			item.useTime = 8;
			item.useAnimation = 8;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.channel = true; //Channel so that you can held the weapon [Important]
			item.knockBack = 6;
			item.value = Item.sellPrice(silver : 50);
			item.rare = ItemRarityID.Orange;
			item.UseSound = SoundID.Item11;
			item.shoot = ModContent.ProjectileType<Projectiles.SuguriBullet>();
			item.autoReuse = true;
			item.useAmmo = AmmoID.Bullet;
		}

		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= 0.2f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 sp = new Vector2(speedX, speedY).RotatedByRandom(25.0f * Matht.Deg2Rad);

			int projToFire = 0;
			float speed = 8.0f;
			bool canShoot = false;
			int dmg = item.damage;
			float kBack = 0;
			player.PickAmmo(item, ref projToFire, ref speed, ref canShoot, ref dmg, ref kBack, true);
			float angle = (float)Math.Atan2(sp.Y, sp.X);
			// spawn boolet
			int proj = Projectile.NewProjectile(position, new Vector2(speed, 0).RotatedBy(angle), mod.ProjectileType("SuguriBullet"), dmg, kBack, player.whoAmI, projToFire);
			Projectile realProj = Main.projectile[proj];
			if (projToFire == ProjectileID.MeteorShot)
			{
				realProj.penetrate = 2;
			}
			return false;
		}

	}
}
