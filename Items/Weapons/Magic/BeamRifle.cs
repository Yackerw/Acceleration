using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Acceleration.Items.Weapons.Magic
{
	class BeamRifle : ModItem
	{

		public class BeamRifleFireCallback : SyncCallback
		{
			public static void Callback(BinaryReader reader)
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
			BeamRifle br = null; ;
			if (player.HeldItem.type == Acceleration.thisMod.ItemType("BeamRifle"))
				br = (BeamRifle)player.HeldItem.modItem;
			// err checking
			if (br == null)
			{
				return;
			}
			int projectile;
			float damage;
			Vector2 shootSpeed;
			if (!charged)
			{
				projectile = Acceleration.thisMod.ProjectileType("Beam");
				damage = br.item.damage;
				Main.PlaySound(Acceleration.BeamRifleSound, player.position);
				shootSpeed = new Vector2((float)Math.Cos(angle) * 10.0f, (float)Math.Sin(angle) * 10.0f);
			} else
			{
				projectile = Acceleration.thisMod.ProjectileType("ChargeBeam");
				damage = br.item.damage * 2.1f;
				Main.PlaySound(Acceleration.ChargeShotSound, player.position);
				shootSpeed = new Vector2((float)Math.Cos(angle) * 13.0f, (float)Math.Sin(angle) * 13.0f);
			}
			Projectile proj = Main.projectile[Projectile.NewProjectile(player.Center, shootSpeed, projectile, (int)(player.magicDamageMult * damage), 6, player.whoAmI)];
			proj.rotation = angle;
			proj.owner = player.whoAmI;
		}

		bool charging = false;
		int chargeTime;
		bool chargeFire;
		bool hyper = false;
		int hyperTimer = 0;
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Fires a laser"
				+ "\nRight click: charge shot"
				+ "\nHYPER: Big laser");
		}

		public override void AddRecipes()
		{
			ModRecipe currRecipe = new ModRecipe(Acceleration.thisMod);
			currRecipe.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12);
			currRecipe.AddIngredient(ItemID.SilverBar, 20);
			currRecipe.AddTile(TileID.Anvils);
			currRecipe.SetResult(ModContent.ItemType<BeamRifle>());
			currRecipe.AddRecipe();

			currRecipe = new ModRecipe(Acceleration.thisMod);
			currRecipe.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12);
			currRecipe.AddIngredient(ItemID.TungstenBar, 20);
			currRecipe.AddTile(TileID.Anvils);
			currRecipe.SetResult(ModContent.ItemType<BeamRifle>());
			currRecipe.AddRecipe();
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
			if (chargeFire)
			{
				chargeFire = false;
				return false;
			}
			if (hyper)
			{
				if (hyperTimer <= 0)
				{
					hyper = false;
				}
				return false;
			}
			if ((!player.GetModPlayer<AcceleratePlayer>().rightClick || !charging) && hyper == false)
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
					if (player.CheckMana(player.GetManaCost(item), true))
					{
						float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.Center.Y, Main.MouseWorld.X - player.Center.X);
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
					}
					player.itemAnimation = 25;
					player.itemAnimationMax = 25;
					player.reuseDelay = 25;
					item.useTime = 25;
					item.useAnimation = 25;
					chargeFire = true;
					charging = false;
					chargeTime = 0;
				}
			}

			if (ap.hyperButton && !ap.prevHyperButton && player.reuseDelay <= 0 && ap.hyper >= 1.0f)
			{
				hyperTimer = 70;
				hyper = true;
				//Projectile.NewProjectile
				// um, try rendering, i guess
				ap.SetupHyper();
			}
			if (hyper)
			{
				--hyperTimer;
				player.reuseDelay = 1;
				player.itemAnimation = 1;
				player.itemAnimationMax = 1;
				// actually fire the projectile
				if (hyperTimer == 25)
				{
					Main.PlaySound(Acceleration.beamRifleHyperSound, player.position);
					float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
					Projectile projectile = Main.projectile[Projectile.NewProjectile(player.position + new Vector2(40, 0).RotatedBy(shotAngle), new Vector2(0, 0), ModContent.ProjectileType<Projectiles.BeamHyper>(), (int)(item.damage * 0.666f * player.magicDamageMult), 1.0f, player.whoAmI, 0, shotAngle)];
					// spawn the rest of them...
					// draw a good number of projectiles
					Vector2 spawnPos = projectile.Center;
					int projToSpawn = ModContent.ProjectileType<Projectiles.BeamHyper>();
					Vector2 normal = new Vector2(1, 0).RotatedBy(shotAngle);
					Vector2 spawnIncrement = normal * 40;
					for (int i = 0; i < 40; ++i)
					{
						Vector2 moveAmnt = Collision.TileCollision(spawnPos, spawnIncrement, 40, 40, false, false, 0);
						if (moveAmnt.X != spawnIncrement.X || moveAmnt.Y != spawnIncrement.Y)
						{
							spawnPos += moveAmnt;
							break;
						}
						spawnPos += spawnIncrement;
						Projectile proj = Main.projectile[Projectile.NewProjectile(spawnPos, new Vector2(0, 0), projToSpawn, projectile.damage, projectile.knockBack, projectile.owner, 2 + ((i * 2) % 4 <= 1 ? 1 : 0), projectile.ai[1])];
						proj.alpha = projectile.alpha;
						// hack for ordering...
						if (proj.whoAmI < projectile.whoAmI)
						{
							proj.timeLeft -= 1;
						}
					}
					Projectile explodeProj = Main.projectile[Projectile.NewProjectile(spawnPos, new Vector2(0, 0), projToSpawn, projectile.damage, projectile.knockBack, projectile.owner, 4, 0)];
					if (explodeProj.whoAmI < projectile.whoAmI)
					{
						explodeProj.timeLeft -= 1;
					}
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
