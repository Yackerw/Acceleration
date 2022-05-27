using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace Acceleration.Items.Accessories
{
	class Pudding : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pudding");
			Tooltip.SetDefault("A treat so tasty, it was said to have been sealed away..." +
				"\nSummons protective rbits around the player" +
				"\nCounts as summon damage");
		}
		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Summon;
			Item.width = 30;
			Item.height = 28;
			Item.useTime = 20;
			//Item.useAnimation = 20;
			//Item.useStyle = 1;
			Item.knockBack = -1;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;
			//Item.UseSound = SoundID.Item1;
			//Item.autoReuse = false;
			Item.accessory = true;
			Item.expert = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// tell the player we can dash
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			ap.rbits = true;
		}
	}
}
