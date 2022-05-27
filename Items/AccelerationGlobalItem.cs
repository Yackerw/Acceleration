using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Acceleration.Items
{
	class AccelerationGlobalItem : GlobalItem
	{

		public override bool CanUseItem(Item item, Player player)
		{
			if (item.damage > 0)
			{
				AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
				if (ap.rbits && ap.rbitCooldown <= 0)
				{
					ap.FireRbitShots();
				}
			}
			return base.CanUseItem(item, player);
		}
	}
}
