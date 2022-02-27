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
				"\nSummons protective rbits around the player." +
				"\nCounts as summon damage");
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
			item.value = Item.sellPrice(0, 1, 0, 0);
			item.rare = ItemRarityID.Orange;
			//item.UseSound = SoundID.Item1;
			//item.autoReuse = false;
			item.accessory = true;
			item.expert = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			// tell the player we can dash
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			ap.rbits = true;
		}
	}
}
