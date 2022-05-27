using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Acceleration.NPCs.Bosses;
using Acceleration.Items.Weapons.Ranged;

namespace Acceleration.Items.Bags
{
	class SakiBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Green;
			Item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor(player.GetSource_DropAsItem());
			// TODO
			// player.QuickSpawnItem
			if (Main.rand.Next(0, 3) <= 1)
			{
				player.QuickSpawnItem(player.GetSource_DropAsItem(), ModContent.ItemType<Maracca>(), 30);
			}
			if (Main.rand.Next(0, 3) <= 1)
			{
				player.QuickSpawnItem(player.GetSource_DropAsItem(), ModContent.ItemType<Items.Weapons.Melee.Tambourine>(), 1);
			}
			if (Main.rand.Next(0, 3) <= 1)
			{
				player.QuickSpawnItem(player.GetSource_DropAsItem(), ModContent.ItemType<Items.Weapons.Magic.PowerBell>(), 1);
			}
			player.QuickSpawnItem(player.GetSource_DropAsItem(), ModContent.ItemType<Items.Accessories.Pudding>());
		}

		public override int BossBagNPC => ModContent.NPCType<Saki>();
	}
}
