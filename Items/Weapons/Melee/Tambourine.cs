using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Acceleration.Items.Weapons.Melee
{
	class Tambourine : ModItem
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
			item.UseSound = Acceleration.tambourineSound;
			item.shoot = ModContent.ProjectileType<Projectiles.Tambourine>();
			item.shootSpeed = 10f;
			item.noUseGraphic = true;
			item.channel = false;
		}
	}
}
