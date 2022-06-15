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
using Acceleration.NPCs.Bosses;
using Acceleration.Misc;
using System;

namespace Acceleration
{
	class Acceleration : Mod
	{

		public HeatUI heat;
		public UserInterface UI;

		public static SoundStyle dashSound;
		public static SoundStyle RRSound;
		public static SoundStyle BeamRifleSound;
		public static SoundStyle ChargeInitialSound;
		public static SoundStyle ChargeShotSound;
		public static SoundStyle MissileLaunchSound;
		public static SoundStyle hyperSound;
		public static SoundStyle beamRifleHyperSound;
		public static SoundStyle tambourineSound;
		public static SoundStyle sword2Sound;
		public static SoundStyle sword3Sound;
		public static SoundStyle swordHyperSound;
		public static SoundStyle bossDeathSound;
		public ModKeybind hyperKey;

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
				hyperKey = KeybindLoader.RegisterKeybind(this, "Hyper", Microsoft.Xna.Framework.Input.Keys.V);
				heat = new HeatUI();
				heat.Activate();
				UI = new UserInterface();
				UI.SetState(heat);
				heat.visible = true;
				dashSound = new SoundStyle("Acceleration/Sounds/Custom/air01");
				RRSound = new SoundStyle("Acceleration/Sounds/Custom/power36");
				BeamRifleSound = new SoundStyle("Acceleration/Sounds/Custom/BeamRifle");
				ChargeInitialSound = new SoundStyle("Acceleration/Sounds/Custom/Charge");
				ChargeShotSound = new SoundStyle("Acceleration/Sounds/Custom/ChargeShot");
				MissileLaunchSound = new SoundStyle("Acceleration/Sounds/Custom/Burst00");
				hyperSound = new SoundStyle("Acceleration/Sounds/Custom/HyperUse");
				beamRifleHyperSound = new SoundStyle("Acceleration/Sounds/Custom/gun11_r");
				tambourineSound = new SoundStyle("Acceleration/Sounds/Custom/Tambourine");
				sword2Sound = new SoundStyle("Acceleration/Sounds/Custom/sword2");
				sword3Sound = new SoundStyle("Acceleration/Sounds/Custom/sword3");
				swordHyperSound = new SoundStyle("Acceleration/Sounds/Custom/crash16_c");
				bossDeathSound = new SoundStyle("Acceleration/Sounds/Custom/don11");
				
				// load our shaders
				//Ref<Effect> projectileShader = new Ref<Effect>(GetEffect("Effects/RainbowRing"));

				//GameShaders.Misc["RainbowRing"] = new MiscShaderData(projectileShader, "ModdersToolkitShaderPass");
			}
			thisMod = this;
			// smoother way to handle network functions...
			System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			int i = 0;
			Type t = new SyncCallback().GetType();
			KEEPGOING:
			try
			{
				while (i < asms.Length)
				{
					var asm = asms[i];
					foreach (var type in asm.GetTypes())
					{
						if (type.BaseType == t)
						{
							RegisterNetFunc(type);
						}
					}
					++i;
				}
			} catch
			{
				++i;
				goto KEEPGOING;
			}
		}

		public override void PostSetupContent()
		{
			Mod bossChecklist = null;
			try
			{
				bossChecklist = ModLoader.GetMod("BossChecklist");
			} catch
			{
				// none
			}
			if (bossChecklist != null)
			{
				bossChecklist.Call(
					"AddBoss",
					1.5f,
					ModContent.NPCType<NPCs.Bosses.Saki>(),
					this, // Mod
					"Saki",
					(Func<bool>)(() => AccelerateWorld.sakiDefeated),
					null,
					null,
					new List<int> { ModContent.ItemType<Items.Weapons.Ranged.Maracca>(), ModContent.ItemType<Items.Weapons.Magic.PowerBell>(), ModContent.ItemType<Items.Weapons.Melee.Tambourine>() },
					"Shows up at the end of the first Shifu invasion",
					null,
					null,
					null,
					null);
				// Additional bosses here
			}
		}



		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			int packetStart = (int)reader.BaseStream.Position;
			int id = reader.ReadInt32();
			if (id == -1)
			{
				int len = reader.ReadUInt16();
				// relay to everyone else
				int whom = reader.ReadByte();
				ModPacket relayPacket = GetPacket();
				while (reader.BaseStream.Position < len + packetStart + 7)
				{
					relayPacket.Write((byte)reader.ReadByte());
				}
				relayPacket.Send(-1, whom);
				reader.BaseStream.Seek(packetStart + 7, SeekOrigin.Begin);
				id = reader.ReadInt32();
			}

			if (id < callbacks.Count)
			{
				callbacks[id].GetMethod("Callback").Invoke(null, new object[] { reader });
			}
		}

		List<Type> callbacks = new List<Type>();

		public void RegisterNetFunc(Type t)
		{
			Logger.Info(t.ToString());
			SyncCallback.references.Add(t, callbacks.Count);
			callbacks.Add(t);
		}

		public override void Unload()
		{
			base.Unload();
			// kill our reference to ourself?
			thisMod = null;
		}
	}
}
