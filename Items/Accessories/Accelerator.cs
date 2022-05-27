using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Acceleration.Items.Accessories
{
	public class Accelerator : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Accelerator");
			Tooltip.SetDefault("A legendary weapon said to have the power to stop wars and save the world");
		}
		public override void SetDefaults()
		{
			Item.damage = -1;
			Item.width = 30;
			Item.height = 28;
			Item.useTime = 20;
			//Item.useAnimation = 20;
			//Item.useStyle = 1;
			Item.knockBack = -1;
			Item.value = 10000;
			Item.rare = ItemRarityID.Orange;
			//Item.UseSound = SoundID.Item1;
			//Item.autoReuse = false;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe().AddIngredient(ItemID.DirtBlock, 10).AddTile(TileID.WorkBenches).Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// tell the player we can dash
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			ap.accel = true;
			ap.maxAccelTime = 100000000;
			player.noFallDmg = true;
		}
	}
}
