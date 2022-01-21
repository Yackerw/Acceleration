using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;
using Acceleration;
using Mathj;

namespace Acceleration.UI
{
	class HeatUI : UIState
	{
		public UIText heatText;
		public UIElement hyperBar;
		public UIElement hyperBar1;
		public UIElement hyperBar2;
		public UIImage hyperBarHolder;

		public bool visible;

		public override void OnInitialize()
		{
			//heatPanel = new UIText("Heat 000%");
			/*heatPanel = new UIPanel();
			heatPanel.Left.Set(100f, 0f);
			heatPanel.Top.Set(100f, 0f);
			heatPanel.Width.Set(100f, 0f);
			heatPanel.Height.Set(100f, 0f);*/
			heatText = new UIText("Heat 000%");
			heatText.Left.Set(10f, 0f);
			heatText.Top.Set(80f, 0f);
			heatText.Width.Set(100f, 0f);
			heatText.Height.Set(100f, 0f);
			//heatPanel.Append(heatText);
			base.Append(heatText);

			hyperBar = new UIElement();
			hyperBar.Left.Set(10f, 0f);
			hyperBar.Top.Set(110f, 0f);
			hyperBar.Width.Set(120f, 0f);
			hyperBar.Height.Set(20f, 0f);
			Append(hyperBar);

			hyperBar1 = new UIElement();
			hyperBar1.Left.Set(4, 0f);
			hyperBar1.Top.Set(4, 0f);
			hyperBar1.Width.Set(40f, 0f);
			hyperBar1.Height.Set(8f, 0f);
			hyperBar.Append(hyperBar1);

			hyperBarHolder = new UIImage(ModContent.GetTexture("Acceleration/UI/HyperBar"));
			hyperBarHolder.Left.Set(0, 0f);
			hyperBarHolder.Top.Set(0, 0f);
			hyperBarHolder.Width.Set(120f, 0f);
			hyperBarHolder.Height.Set(20f, 0f);
			hyperBar.Append(hyperBarHolder);
		}

		public override void Update(GameTime gameTime)
		{
			Player p = Main.player[Main.myPlayer];
			AcceleratePlayer ap = p.GetModPlayer<AcceleratePlayer>();
			heatText.SetText(string.Concat("Heat ", ap.heat.ToString("000"), "%"));
			heatText.TextColor = new Color(1, Matht.Lerp(1, 0, ap.heat / 300.0f), Matht.Lerp(1, 0, ap.heat / 300.0f));
			if (!Main.playerInventory)
			{
				heatText.Top.Set(80f, 0f);
				hyperBar.Top.Set(110f, 0f);
			} else
			{
				heatText.Top.Set(260f, 0f);
				hyperBar.Top.Set(290f, 0f);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			// draw the "bar"
			Color backColor;
			Color frontColor;
			float hyper = Main.LocalPlayer.GetModPlayer<AcceleratePlayer>().hyper;
			int width;
			if (hyper < 1.0f)
			{
				backColor = Color.Black;
				frontColor = Color.Blue;
				width = (int)(hyper * 112);
			} else if (hyper < 2.0f)
			{
				backColor = Color.Blue;
				frontColor = Color.Green;
				width = (int)((hyper - 1.0f) * 112);
			}
			else
			{
				backColor = Color.Green;
				frontColor = Color.Yellow;
				width = (int)((hyper - 2.0f) * 112);
			}
			Rectangle backRect = new Rectangle((int)hyperBar.Left.Pixels + 4, (int)hyperBar.Top.Pixels + 4, 112, 16);
			Rectangle rect = new Rectangle((int)hyperBar.Left.Pixels + 4, (int)hyperBar.Top.Pixels + 4, width, 16);
			spriteBatch.Draw(Main.magicPixel, backRect, backColor);
			spriteBatch.Draw(Main.magicPixel, rect, frontColor);
			base.DrawSelf(spriteBatch);
		}
	}
}