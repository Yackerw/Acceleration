using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Acceleration.NPCs.Enemies;

namespace Acceleration.Invasions
{
	class SakiInvasion
	{
		static public int[] enemies = { ModContent.NPCType<RoboBall>(), ModContent.NPCType<MissileBot>(), ModContent.NPCType<MissileCarrier>() };
		static public float[] enemyRate = { 0.8f, 0.8f, 0.05f };

		static public void StartInvasion()
		{
			Main.NewText("The sky is accelerating...", 15, 140, 255, true);
			Main.invasionType = (int)AccelerateWorld.Invasions.Saki;
			Main.invasionProgress = 0;
			Main.invasionProgressMax = 100;
			Main.invasionX = Main.spawnTileX;
			Main.invasionSize = 100;
			Main.invasionSizeStart = 100;
			Main.invasionProgressWave = 0;
		}
	}
}
