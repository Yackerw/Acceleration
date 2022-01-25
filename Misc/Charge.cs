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
using Mathj;
using Acceleration.NPCs;

namespace Acceleration.Misc
{
	class Charge
	{
		int spawnTime = 30;
		int killTime = 60;
		public Vector2 position;
		public Charge next;
		public Charge previous;
		static public Charge firstCharge;
		static public Charge lastCharge;

		static public Charge AddCharge(Vector2 position)
		{
			Charge retValue = new Charge();
			retValue.position = position;

			if (firstCharge == null)
			{
				firstCharge = retValue;
			}
			if (lastCharge != null)
			{
				lastCharge.next = retValue;
			}
			retValue.previous = lastCharge;
			lastCharge = retValue;

			return retValue;
		}

		static public void RemoveCharge(Charge charge)
		{
			if (charge.previous != null)
			{
				charge.previous.next = charge.next;
			}
			if (charge.next != null)
			{
				charge.next.previous = charge.previous;
			}
			if (firstCharge == charge)
			{
				firstCharge = charge.next;
			}
			if (lastCharge == charge)
			{
				lastCharge = charge.previous;
			}
		}

		static public void Update()
		{
			Charge charge = firstCharge;
			while (charge != null)
			{
				charge.AI();
				charge = charge.next;
			}
		}

		static public void UpdateDraw()
		{
			Main.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
			Charge charge = firstCharge;
			Texture2D tex = ModContent.GetTexture("Acceleration/Misc/Charge");
			while (charge != null)
			{
				AccelerationHelper.DrawSpriteCached(tex, charge.position, 0, 40, Color.White, 0, new Vector2(1,1));
				charge = charge.next;
			}
			Main.spriteBatch.End();
		}

		public void AI()
		{
			// move up for a short while
			position.Y -= 4.0f * ((float)spawnTime / 30.0f);
			if (spawnTime > 0)
			{
				--spawnTime;
			}
			--killTime;
			if (killTime <= 0)
			{
				RemoveCharge(this);
			}
		}
	}
}
