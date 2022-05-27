using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Acceleration.Invasions
{
	internal class SakiSceneEffect : ModSceneEffect
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/GreenBird");
		public override SceneEffectPriority Priority => SceneEffectPriority.Event;
		public override bool IsSceneEffectActive(Player player)
		{
			if (Main.invasionType == (int)AccelerateWorld.Invasions.Saki)
			{
				return true;
			}
			return false;
		}
	}
}
