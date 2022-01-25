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

namespace Acceleration
{
	class Acceleration : Mod
	{

		public HeatUI heat;
		UserInterface UI;

		public static LegacySoundStyle dashSound;
		public static LegacySoundStyle RRSound;
		public static LegacySoundStyle BeamRifleSound;
		public static LegacySoundStyle ChargeInitialSound;
		public static LegacySoundStyle ChargeShotSound;
		public static LegacySoundStyle MissileLaunchSound;
		public static LegacySoundStyle hyperSound;
		public static LegacySoundStyle beamRifleHyperSound;
		public static LegacySoundStyle tambourineSound;
		public static LegacySoundStyle sword2Sound;
		public static LegacySoundStyle sword3Sound;
		public static LegacySoundStyle swordHyperSound;
		public ModHotKey hyperKey;

		public static Acceleration thisMod;

		public Acceleration()
		{
		}

		public override void Load()
		{
			// register our dash input
			if (!Main.dedServ)
			{
				//dashKey = RegisterHotKey("Dash", "C");
				hyperKey = RegisterHotKey("Hyper", "V");
				heat = new HeatUI();
				heat.Activate();
				UI = new UserInterface();
				UI.SetState(heat);
				heat.visible = true;
				dashSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/air01");
				RRSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/power36");
				BeamRifleSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/BeamRifle");
				ChargeInitialSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/Charge");
				ChargeShotSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/ChargeShot");
				MissileLaunchSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/Burst00");
				hyperSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/HyperUse");
				beamRifleHyperSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/gun11_r");
				tambourineSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/Tambourine");
				sword2Sound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/sword2");
				sword3Sound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/sword3");
				swordHyperSound = GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/crash16_c");
				// load our shaders
				Ref<Effect> projectileShader = new Ref<Effect>(GetEffect("Effects/RainbowRing"));

				GameShaders.Misc["RainbowRing"] = new MiscShaderData(projectileShader, "ModdersToolkitShaderPass");
			}
			thisMod = this;
			RegisterNetFunc(AcceleratePlayer.pstepCallback);
			RegisterNetFunc(RainbowRing.rrc);
			RegisterNetFunc(BeamRifle.callBack);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (UI != null) UI.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int ind = layers.FindIndex(layer => layer.Name.Equals("AccelerateMod: Heat"));
			if (ind == -1)
			{
				layers.Add(new LegacyGameInterfaceLayer("AccelerateMod: Heat", delegate
				{
					UI.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
					return true;
				}
				));
			}
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			int id = reader.ReadInt32();
			if (id < callbacks.Count)
			{
				callbacks[id].Callback(reader);
			}
		}

		List<SyncCallback> callbacks = new List<SyncCallback>();

		public void RegisterNetFunc(SyncCallback callback)
		{
			callback.reference = callbacks.Count;
			callbacks.Add(callback);
		}

		public override void Unload()
		{
			base.Unload();
			// kill our reference to ourself?
			thisMod = null;
		}
	}
}
