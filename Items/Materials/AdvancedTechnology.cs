using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Acceleration.Items.Materials
{
	class AdvancedTechnology : ModItem
	{
		public override void SetDefaults()
		{
			item.material = true;
			item.noMelee = true;
			item.maxStack = 999;
			item.width = 20;
			item.height = 20;
		}
	}
}
