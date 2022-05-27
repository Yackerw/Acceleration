using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Acceleration.Invasions
{
	class InvasionHandler : GlobalNPC
	{

		void UpdatePool(int[] enemies, float[] rate, IDictionary<int, float> pool)
		{
			pool.Clear();
			for (int i = 0; i < enemies.Length; ++i)
			{
				pool.Add(enemies[i], rate[i]);
			}
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			base.EditSpawnPool(pool, spawnInfo);
			if (Main.invasionType == (int)AccelerateWorld.Invasions.Saki)
			{
				// insert our own enemy pool
				UpdatePool(SakiInvasion.enemies, SakiInvasion.enemyRate, pool);
			}
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (Main.invasionType == (int)AccelerateWorld.Invasions.Saki)
			{
				spawnRate = 100;
				// ????
				maxSpawns = 10000;
			}
		}

		public override void OnKill(NPC npc)
		{
			int[] npcs = null;
			if (Main.invasionType == (int)AccelerateWorld.Invasions.Saki)
			{
				npcs = SakiInvasion.enemies;
			} else
			{
				return;
			}
			for (int i = 0; i < npcs.Length; ++i)
			{
				if (npc.type == npcs[i])
				{
					--Main.invasionSize;
					Main.invasionProgress += 1;
					if (Main.invasionSize <= 0)
					{
						// if we're saki invasion, and saki hasn't been defeated yet, spawn her
						if (Main.invasionType == (int)AccelerateWorld.Invasions.Saki && !AccelerateWorld.sakiDefeated)
						{
							NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)Main.rightWorld / 2, (int)Main.topWorld + 800, ModContent.NPCType<NPCs.Bosses.Saki>());
							Main.NewText("A girl descends from the sky...");
						}
						Main.invasionType = 0;
						Main.NewText("The sky is decelerating...", 12, 140, 255);
					}
					return;
				}
			}
		}
	}
}
