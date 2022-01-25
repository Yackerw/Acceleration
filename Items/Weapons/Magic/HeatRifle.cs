using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Acceleration.Items.Weapons.Magic
{
	class HeatRifle : ModItem
	{

		public class HeatRifleFireCallback : SyncCallback
		{
			public override void Callback(BinaryReader reader)
			{
				int whom = reader.ReadByte();
				bool charged = reader.ReadBoolean();
				float angle = reader.ReadSingle();
				HeatRifle.FireCharged(whom, charged, angle);
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
				if (player.HeldItem.type == Acceleration.thisMod.ItemType("HeatRifle"))
				{
					player.HeldItem.useTime = 26;
					player.HeldItem.useAnimation = 26;
					HeatRifle hr = (HeatRifle)player.HeldItem.modItem;
					hr.charging = false;
					hr.chargeTime = 0;
				}
			}
			int projectile;
			float damage;
			Vector2 shootSpeed;
			if (!charged)
			{
				projectile = Acceleration.thisMod.ProjectileType("HeatBeam");
				damage = 30;
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


		public static HeatRifleFireCallback callBack = new HeatRifleFireCallback();
		bool charging = false;
		int chargeTime;
		bool hyper = true;
		int hyperTimer = 0;
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Fires 3 lasers"
				+ "\nDamage scales with Heat"
				+ "\nRight click: fire lasers repeatedly"
				+ "\nHYPER: big laser, gain Blazing! for 8 seconds");
		}

		public override void AddRecipes()
		{
			ModRecipe currRecipe = new ModRecipe(Acceleration.thisMod);
			currRecipe.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12);
			currRecipe.AddIngredient(ItemID.SilverBar, 20);
			currRecipe.AddTile(TileID.Anvils);
			currRecipe.SetResult(ModContent.ItemType<HeatRifle>());
			currRecipe.AddRecipe();

			currRecipe = new ModRecipe(Acceleration.thisMod);
			currRecipe.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12);
			currRecipe.AddIngredient(ItemID.TungstenBar, 20);
			currRecipe.AddTile(TileID.Anvils);
			currRecipe.SetResult(ModContent.ItemType<HeatRifle>());
			currRecipe.AddRecipe();
		}

		public override void SetDefaults() {
			item.damage = 30;
			item.magic = true;
			item.mana = 10;
			item.width = 26;
			item.height = 26;
			item.useTime = 30;
			item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.channel = true; //Channel so that you can held the weapon [Important]
			item.knockBack = 6;
			item.value = Item.sellPrice(silver : 50);
			item.rare = ItemRarityID.Orange;
			item.UseSound = Acceleration.BeamRifleSound;
			item.shoot = ModContent.ProjectileType<Projectiles.HeatBeam>();
			item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int scaling = (int) (30*(((1.0f * player.GetModPlayer<AcceleratePlayer>().heat))/100));
			if (player.GetModPlayer<AcceleratePlayer>().heat <= 100) //this is so we dont accidentally do 0 damage 
            {
				item.damage = 30;
			} else
            {
				item.damage = scaling; //this prevents an item description glitch!! must be after 30 alone. this is because it reads whatever is first!
            }
            // dirty hack to prevent charge shot firing something
            if (item.useAnimation == 31)
			{
				item.useTime = 30;
				item.useAnimation = 30;
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
                item.useTime = 30;
                item.useAnimation = 30;
                charging = false;
                float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
                Projectile.NewProjectile(player.position, new Vector2(10, 0).RotatedBy(shotAngle + (10f * Mathj.Matht.Deg2Rad)), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(item.damage * player.magicDamageMult), 1.0f, player.whoAmI, 0, shotAngle);
				Projectile.NewProjectile(player.position, new Vector2(10, 0).RotatedBy(shotAngle), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(item.damage * player.magicDamageMult), 1.0f, player.whoAmI, 0, shotAngle);
				Projectile.NewProjectile(player.position, new Vector2(10, 0).RotatedBy(shotAngle + (-10f * Mathj.Matht.Deg2Rad)), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(item.damage * player.magicDamageMult), 1.0f, player.whoAmI, 0, shotAngle);
                return false;
            }
            else
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
					player.itemAnimation = 30;
					player.itemAnimationMax = 30;
					player.reuseDelay = 30;
					item.useTime = 31;
					item.useAnimation = 31;
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
					Projectile.NewProjectile(player.position + new Vector2(40, 0).RotatedBy(shotAngle), new Vector2(0, 0), ModContent.ProjectileType<Projectiles.BeamHyper>(), (int)(18 * player.magicDamageMult), 1.0f, player.whoAmI, 0, shotAngle);
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
//if you're reading this yacker, thanks for believing in me. if it weren't for you offering to do a project with me i probably wouldn't be back into coding.