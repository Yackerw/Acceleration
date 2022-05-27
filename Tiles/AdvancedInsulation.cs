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


namespace Acceleration.Tiles
{
	public class AdvancedInsulation: ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = false;
			MinPick = 210;
			//dustType = ModContent.DustType<Sparkle>();
			ItemDrop = ModContent.ItemType<Items.Placeable.AdvancedInsulation>();
			AddMapEntry(new Color(152, 168, 128));
			//SetModTree(new Trees.ExampleTree());
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		/*public override void ChangeWaterfallStyle(ref int style)
		{
			style = Mod.GetWaterfallStyleSlot("ExampleWaterfallStyle");
		}*/
	}
}