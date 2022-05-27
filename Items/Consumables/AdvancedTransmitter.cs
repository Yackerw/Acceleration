using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Acceleration.Invasions;
using Terraria.ID;
using Terraria;

namespace Acceleration.Items.Consumables
{
	class AdvancedTransmitter : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.noMelee = true;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(silver: 15);
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = 999;
			Item.consumable = true;
		}

		public override bool? UseItem(Player player)
		{
			if (Main.invasionType != 0)
			{
				return false;
			}
			SakiInvasion.StartInvasion();
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 20)
				.AddIngredient(ItemID.FallenStar, 5)
				.Register();
		}
	}
}
