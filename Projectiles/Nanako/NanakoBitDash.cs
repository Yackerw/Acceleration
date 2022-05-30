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
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Mathj;
using Acceleration.NPCs;

namespace Acceleration.Projectiles.Nanako
{
	class NanakoBitDash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			//Projectile.alpha = 50;
			Projectile.timeLeft = 240;
			Projectile.penetrate = 1;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.Name = "Bit";
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			Projectile.netUpdate = true;
		}
	}
}
