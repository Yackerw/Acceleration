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
			item.width = 26;
			item.height = 26;
			item.useTime = 30;
			item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true;
			item.knockBack = 6;
			item.value = Item.sellPrice(silver: 15);
			item.rare = ItemRarityID.Orange;
			item.maxStack = 999;
			item.consumable = true;
		}

		public override bool UseItem(Player player)
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
			ModRecipe currRecipe = new ModRecipe(Acceleration.thisMod);
			currRecipe.AddIngredient(ModContent.ItemType<Materials.AdvancedTechnology>(), 20);
			currRecipe.AddTile(TileID.WorkBenches);
			currRecipe.SetResult(ModContent.ItemType<AdvancedTransmitter>());
			currRecipe.AddRecipe();
		}
	}
}
