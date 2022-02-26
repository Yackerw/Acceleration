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
			item.maxStack = 999;
			item.consumable = true;
			item.width = 24;
			item.height = 24;
			item.rare = ItemRarityID.Green;
			item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();
			// TODO
			// player.QuickSpawnItem
			if (Main.rand.Next(0, 3) <= 1)
			{
				player.QuickSpawnItem(ModContent.ItemType<Maracca>(), 30);
			}
			if (Main.rand.Next(0, 3) <= 1)
			{
				player.QuickSpawnItem(ModContent.ItemType<Items.Weapons.Melee.Tambourine>(), 1);
			}
			if (Main.rand.Next(0, 3) <= 1)
			{
				player.QuickSpawnItem(ModContent.ItemType<Items.Weapons.Magic.PowerBell>(), 1);
			}
		}

		public override int BossBagNPC => ModContent.NPCType<Saki>();
	}
}
