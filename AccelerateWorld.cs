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
	class AccelerateWorld : ModWorld
	{
		static public bool sakiDefeated;
		bool prevDayTime = true;

		public enum Invasions { Saki = -56 };
		public override void Initialize()
		{
			RainbowRing.firstRing = null;
			RainbowRing.lastRing = null;
		}
		public override void PreUpdate()
		{

		}

		public override void PostUpdate()
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

		public override TagCompound Save()
		{
			TagCompound tc = new TagCompound();
			tc.Add(new KeyValuePair<string, object>("SakiDefeat", sakiDefeated));
			return tc;
		}

		public override void Load(TagCompound tag)
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
