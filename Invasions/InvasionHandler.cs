﻿using System;
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

		public override void NPCLoot(NPC npc)
		{
			base.NPCLoot(npc);
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
					if (Main.invasionSize <= 0)
					{
						Main.invasionType = 0;
						Main.NewText("The sky is decelerating...", 12, 140, 255, true);
					}
					return;
				}
			}
		}
	}
}
