using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Acceleration.Items.Accessories
{
	public class AcceleratorProto : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Accelerator Prototype");
			Tooltip.SetDefault("A powerful booster that enhances your abilities to new levels");
		}
		public override void SetDefaults()
		{
			item.damage = -1;
			item.melee = false;
			item.width = 30;
			item.height = 28;
			item.useTime = 20;
			//item.useAnimation = 20;
			//item.useStyle = 1;
			item.knockBack = -1;
			item.value = 250000;
			item.rare = ItemRarityID.Orange;
			//item.UseSound = SoundID.Item1;
			//item.autoReuse = false;
			item.accessory = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 20);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// tell the player we can dash
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			ap.accel = true;
			ap.maxAccelTime = 400;
			player.noFallDmg = true;
		}
	}
}
