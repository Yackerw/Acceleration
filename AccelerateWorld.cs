using Terraria.ModLoader;
using Acceleration.UI;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using System.IO;
using Acceleration.NPCs;
using Acceleration.Items.Weapons;
using Acceleration.Items.Weapons.Magic;
using Acceleration.Items.Weapons.Melee;
using Acceleration.Items.Weapons.Ranged;
using Acceleration.Misc;
using Terraria.ModLoader.IO;
using Acceleration.Invasions;

namespace Acceleration
{
	class AccelerateWorld : ModSystem
	{
		static public bool sakiDefeated;
		bool prevDayTime = true;

		public enum Invasions { Saki = -56 };
		public override void OnWorldLoad()
		{
			RainbowRing.firstRing = null;
			RainbowRing.lastRing = null;
		}
		public override void PreUpdateWorld()
		{

		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (((Acceleration)Mod).UI != null) ((Acceleration)Mod).UI.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int ind = layers.FindIndex(layer => layer.Name.Equals("AccelerateMod: Heat"));
			if (ind == -1)
			{
				layers.Add(new LegacyGameInterfaceLayer("AccelerateMod: Heat", delegate
				{
					((Acceleration)Mod).UI.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
					return true;
				}
				));
			}
		}

		public override void PostUpdateWorld()
		{
			// handle the invasion stuff
			if (Main.invasionType == (int)Invasions.Saki)
			{
				Main.invasionProgressNearInvasion = true;
			}

			if (Main.dayTime && !prevDayTime)
			{
				// 1/3 chance for invasion to occur if eoc is dead
				if (!sakiDefeated && NPC.downedBoss1 && Main.invasionType == 0)
				{
					if (Main.rand.Next(0, 3) == 0)
					{
						SakiInvasion.StartInvasion();
					}
				}
			}
			prevDayTime = Main.dayTime;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag.Add(new KeyValuePair<string, object>("SakiDefeat", sakiDefeated));
		}

		public override void LoadWorldData(TagCompound tag)
		{
			sakiDefeated = tag.GetBool("SakiDefeat");
		}

		public override void PostDrawTiles()
		{
			// yes these have to go here for multiplayer
			RainbowRing.Update();
			Charge.Update();
			RainbowRing.UpdateDraw();
			Charge.UpdateDraw();
		}
	}
}
