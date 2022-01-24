using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Acceleration.Items.Accessories
{
	public class Booster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Booster");
			Tooltip.SetDefault("A primitive piece of propelling technology");
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
			item.value = 5000;
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
			ap.maxAccelTime = 30;
			player.noFallDmg = true;
		}
	}
}
