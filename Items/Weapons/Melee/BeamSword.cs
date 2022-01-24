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
		public override void SetDefaults()
		{
			item.damage = 10;
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
			item.UseSound = Acceleration.BeamRifleSound;
			item.shoot = ModContent.ProjectileType<Projectiles.BeamSword>();
			item.shootSpeed = 10f;
			item.noUseGraphic = true;
			item.channel = false;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(player.position, new Vector2(0, 0), ModContent.ProjectileType<Projectiles.BeamSword>(), (int)(item.damage * player.meleeDamageMult), item.knockBack, player.whoAmI, 0);
			return false;
		}
	}
}
