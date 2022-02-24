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

namespace Acceleration
{
	class AccelerateWorld : ModWorld
	{

		public enum Invasions { Saki = -56 };
		public override void Initialize()
		{
			RainbowRing.firstRing = null;
			RainbowRing.lastRing = null;
		}
		public override void PreUpdate()
		{

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
