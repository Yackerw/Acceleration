using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Acceleration.Items
{
	class AccelerationGlobalItem : GlobalItem
	{
		public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (item.damage > 0)
			{
				AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
				if (ap.rbits && ap.rbitCooldown <= 0)
				{
					ap.FireRbitShots();
				}
			}
			return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}
	}
}
