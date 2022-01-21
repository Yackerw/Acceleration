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
			item.damage = -1;
			item.melee = false;
			item.width = 30;
			item.height = 28;
			item.useTime = 20;
			//item.useAnimation = 20;
			//item.useStyle = 1;
			item.knockBack = -1;
			item.value = 10000;
			item.rare = ItemRarityID.Orange;
			//item.UseSound = SoundID.Item1;
			//item.autoReuse = false;
			item.accessory = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// tell the player we can dash
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			ap.accel = true;
			player.noFallDmg = true;
		}
	}
}
