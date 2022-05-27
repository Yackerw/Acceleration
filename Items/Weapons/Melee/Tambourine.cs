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
			Item.damage = 10;
			Item.DamageType = DamageClass.Melee;
			Item.width = 48;
			Item.height = 48;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = Acceleration.tambourineSound;
			Item.shoot = ModContent.ProjectileType<Projectiles.Tambourine>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.channel = false;
		}
	}
}
