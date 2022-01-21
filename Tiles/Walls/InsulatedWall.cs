using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using Mathj;
using Acceleration.NPCs;


namespace Acceleration.Tiles.Walls
{
	public class InsulatedWall: ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = true;
			//dustType = ModContent.DustType<Sparkle>();
			drop = ModContent.ItemType<Items.Placeable.InsulatedWall>();
			AddMapEntry(new Color(72, 80, 64));
			//SetModTree(new Trees.ExampleTree());
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}