using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Acceleration.NPCs
{
	class AccelGlobalNPC : GlobalNPC
	{
		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (type == NPCID.Demolitionist)
			{
				// add maraccas if the local player has them
				bool hasMaraccas = false;
				for (int i = 0; i < Main.LocalPlayer.inventory.Length; ++i) {
					if (Main.LocalPlayer.inventory[i].type == ModContent.ItemType<Items.Weapons.Ranged.Maracca>())
					{
						hasMaraccas = true;
					}
				}
				if (hasMaraccas)
				{
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Weapons.Ranged.Maracca>());
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 1, 0);
					++nextSlot;
				}
			}
		}
	}
}
