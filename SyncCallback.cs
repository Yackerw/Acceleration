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
using Mathj;
using Acceleration.NPCs;

namespace Acceleration
{
	public class SyncCallback
	{
		public static Dictionary<Type, int> references = new Dictionary<Type, int>();

		MemoryStream ms = new MemoryStream();

		public void Write(byte input)
		{
			ms.WriteByte(input);
		}

		public void Write(byte[] input)
		{
			ms.Write(input, 0, input.Length);
		}

		public void Write(float input)
		{
			Write(BitConverter.GetBytes(input));
		}

		public void Write(ushort input)
		{
			Write(BitConverter.GetBytes(input));
		}

		public void Write(int input)
		{
			Write(BitConverter.GetBytes(input));
		}

		public ModPacket GetPacket()
		{
			ModPacket mp = Acceleration.thisMod.GetPacket();
			mp.Write(references[this.GetType()]);
			return mp;
		}

		public void SendPacketRelayed(ModPacket packet)
		{
			// LOL sike, make a new one
			ModPacket newPacket = Acceleration.thisMod.GetPacket();
			newPacket.Write(-1);
			newPacket.Write((ushort)packet.BaseStream.Position);
			newPacket.Write((byte)Main.myPlayer);
			int len = (int)packet.BaseStream.Position;
			// ??
			packet.BaseStream.Seek(4, SeekOrigin.Begin);
			//Acceleration.thisMod.Logger.Info("START HERE");
			for (int i = 4; i < len; ++i)
			{
				byte toWrite = (byte)packet.BaseStream.ReadByte();
				//Acceleration.thisMod.Logger.Info(toWrite.ToString());
				newPacket.Write(toWrite);
			}
			newPacket.Send(-1, Main.myPlayer);
		}
	}
}
