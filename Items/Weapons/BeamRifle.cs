using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace Acceleration.Items.Weapons
{
	class BeamRifle : ModItem
	{

		public class BeamRifleFireCallback : SyncCallback
		{
			public override void Callback(BinaryReader reader)
			{
				int whom = reader.ReadByte();
				bool charged = reader.ReadBoolean();
				float angle = reader.ReadSingle();
				BeamRifle.FireCharged(whom, charged, angle);
			}
		}

		public static void FireCharged(int whom, bool charged, float angle)
		{
			Player player = Main.player[whom];
			if (!player.active)
			{
				return;
			}
			if (player != Main.LocalPlayer)
			{
				if (player.HeldItem.type == Acceleration.thisMod.ItemType("BeamRifle"))
				{
					player.HeldItem.useTime = 26;
					player.HeldItem.useAnimation = 26;
					BeamRifle br = (BeamRifle)player.HeldItem.modItem;
					br.charging = false;
					br.chargeTime = 0;
				}
			}
			int projectile;
			float damage;
			Vector2 shootSpeed;
			if (!charged)
			{
				projectile = Acceleration.thisMod.ProjectileType("Beam");
				damage = 27;
				Main.PlaySound(Acceleration.BeamRifleSound, player.position);
				shootSpeed = new Vector2((float)Math.Cos(angle) * 10.0f, (float)Math.Sin(angle) * 10.0f);
			} else
			{
				projectile = Acceleration.thisMod.ProjectileType("ChargeBeam");
				damage = 55;
				Main.PlaySound(Acceleration.ChargeShotSound, player.position);
				shootSpeed = new Vector2((float)Math.Cos(angle) * 13.0f, (float)Math.Sin(angle) * 13.0f);
			}
			Projectile proj = Main.projectile[Projectile.NewProjectile(player.position, shootSpeed, projectile, (int)(player.magicDamageMult * damage), 6, player.whoAmI)];
			proj.rotation = angle;
			proj.owner = player.whoAmI;
		}


		public static BeamRifleFireCallback callBack = new BeamRifleFireCallback();
		bool charging = false;
		int chargeTime;
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Fires a laser"
				+ "\nRight click: charge shot"
				+ "\nHYPER: big laser");
		}

		public override void SetDefaults() {
			item.damage = 27;
			item.magic = true;
			item.mana = 10;
			item.width = 26;
			item.height = 26;
			item.useTime = 25;
			item.useAnimation = 25;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.channel = true; //Channel so that you can held the weapon [Important]
			item.knockBack = 6;
			item.value = Item.sellPrice(silver : 50);
			item.rare = ItemRarityID.Orange;
			item.UseSound = Acceleration.BeamRifleSound;
			item.shoot = ModContent.ProjectileType<Projectiles.Beam>();
			item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			// dirty hack to prevent charge shot firing something
			if (item.useAnimation == 26)
			{
				item.useTime = 25;
				item.useAnimation = 25;
				return false;
			}
			if (!player.GetModPlayer<AcceleratePlayer>().rightClick || !charging)
			{
				item.useTime = 25;
				item.useAnimation = 25;
				charging = false;
				return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
			} else
			{
				// do not shoot! we are still charging!
				return false;
			}
		}

		public override void HoldItem(Player player)
		{
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			if (ap.rightClick && !ap.prevRightClick && player.reuseDelay <= 0)
			{
				charging = true;
				chargeTime = 0;
			}

			if (charging)
			{
				// set to make player unable to use item
				player.reuseDelay = 1;
				player.itemAnimation = 1;
				player.itemAnimationMax = 1;
				item.useTime = 1;
				item.useAnimation = 1;
				++chargeTime;
				if (chargeTime == 60)
				{
					Main.PlaySound(Acceleration.ChargeInitialSound, player.position);
				}
				if (chargeTime > 60)
				{
					chargeTime = 60;
				}
				// upon release, fire either a normal shot or charge shot depending on how long we've charged
				if (!ap.rightClick && player == Main.LocalPlayer)
				{
					if (player.CheckMana(10, true))
					{
						float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
						//if (Main.netMode == NetmodeID.SinglePlayer)
						//{
							FireCharged(player.whoAmI, chargeTime >= 60, shotAngle);
							if (chargeTime >= 60)
							{
								player.velocity += new Vector2((float)Math.Cos(shotAngle) * -8.0f, (float)Math.Sin(shotAngle) * -8.0f);
							}
						//}
						//else
						//{
							// tell server to spawn it
							/*ModPacket packet = Acceleration.thisMod.GetPacket();
							packet.Write(callBack.reference);
							packet.Write((byte)player.whoAmI);
							packet.Write(chargeTime >= 60);
							packet.Write(shotAngle);
							packet.Send();*/
						//}
						player.statMana -= player.GetManaCost(item);
					}
					player.itemAnimation = 25;
					player.itemAnimationMax = 25;
					player.reuseDelay = 25;
					item.useTime = 26;
					item.useAnimation = 26;
					charging = false;
					chargeTime = 0;
				}
			}

			base.HoldItem(player);
		}

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			writer.Write(charging);
			writer.Write((byte)chargeTime);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			base.NetRecieve(reader);
			charging = reader.ReadBoolean();
			chargeTime = reader.ReadByte();
		}

	}
}
