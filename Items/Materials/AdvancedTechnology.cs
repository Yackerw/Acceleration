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
			Item.material = true;
			Item.noMelee = true;
			Item.maxStack = 999;
			Item.width = 20;
			Item.height = 20;
		}
	}
}
