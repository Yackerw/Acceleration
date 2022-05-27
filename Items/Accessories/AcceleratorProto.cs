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
			Item.damage = -1;
			Item.width = 30;
			Item.height = 28;
			Item.useTime = 20;
			//Item.useAnimation = 20;
			//Item.useStyle = 1;
			Item.knockBack = -1;
			Item.value = 250000;
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
			ap.maxAccelTime = 400;
			player.noFallDmg = true;
		}
	}
}
