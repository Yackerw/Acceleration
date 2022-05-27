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
			Item.damage = -1;
			Item.width = 30;
			Item.height = 28;
			Item.useTime = 20;
			//Item.useAnimation = 20;
			//Item.useStyle = 1;
			Item.knockBack = -1;
			Item.value = 5000;
			Item.rare = ItemRarityID.Orange;
			//Item.UseSound = SoundID.Item1;
			//Item.autoReuse = false;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 20)
				.AddIngredient(ItemID.FallenStar, 5)
				.Register();
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
