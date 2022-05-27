using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Acceleration.Items.Weapons.Magic
{
	class HeatRifle : ModItem
	{

		public static void FireCharged(int whom, bool charged, float angle)
		{
			Player player = Main.player[whom];
			if (!player.active)
			{
				return;
			}
			if (player != Main.LocalPlayer)
			{
				if (player.HeldItem.type == ModContent.ItemType<HeatRifle>())
				{
					player.HeldItem.useTime = 26;
					player.HeldItem.useAnimation = 26;
					HeatRifle hr = (HeatRifle)player.HeldItem.ModItem;
					hr.charging = false;
					hr.chargeTime = 0;
				}
			}
			int projectile;
			float damage;
			Vector2 shootSpeed;
			if (!charged)
			{
				projectile = ModContent.ProjectileType<Projectiles.HeatBeam>();
				damage = 30;
				SoundEngine.PlaySound(Acceleration.BeamRifleSound, player.position);
				shootSpeed = new Vector2((float)Math.Cos(angle) * 10.0f, (float)Math.Sin(angle) * 10.0f);
			} else
			{
				projectile = ModContent.ProjectileType<Projectiles.ChargeBeam>();
				damage = 55;
				SoundEngine.PlaySound(Acceleration.ChargeShotSound, player.position);
				shootSpeed = new Vector2((float)Math.Cos(angle) * 13.0f, (float)Math.Sin(angle) * 13.0f);
			}
			Projectile proj = Main.projectile[Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem),player.position, shootSpeed, projectile, (int)(player.GetTotalDamage(DamageClass.Magic).Multiplicative * damage), 6, player.whoAmI)];
			proj.rotation = angle;
			proj.owner = player.whoAmI;
		}

		bool charging = false;
		int chargeTime;
		bool hyper = false;
		int hyperTimer = 0;
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Fires 3 lasers"
				+ "\nDamage scales with Heat"
				+ "\nRight click: fire lasers repeatedly"
				+ "\nHYPER: big laser, gain Blazing! for 8 seconds");
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12)
				.AddIngredient(ItemID.SilverBar, 20)
				.Register();
			CreateRecipe()
				.AddTile(TileID.Anvils)
				.AddIngredient(ModContent.ItemType<Items.Materials.AdvancedTechnology>(), 12)
				.AddIngredient(ItemID.TungstenBar, 20)
				.Register();
		}

		public override void SetDefaults() {
			Item.damage = 45;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 26;
			Item.height = 26;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.channel = true; //Channel so that you can held the weapon [Important]
			Item.knockBack = 6;
			Item.value = Item.sellPrice(silver : 50);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = Acceleration.BeamRifleSound;
			Item.shoot = ModContent.ProjectileType<Projectiles.HeatBeam>();
			Item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int scaling = (int) (Item.damage*(((1.0f * player.GetModPlayer<AcceleratePlayer>().heat))/100));
            // dirty hack to prevent charge shot firing something
            if (Item.useAnimation == 31)
			{
				Item.useTime = 30;
				Item.useAnimation = 30;
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
                Item.useTime = 30;
                Item.useAnimation = 30;
                charging = false;
                float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
				if (player.GetModPlayer<AcceleratePlayer>().heat <= 100) //this is so we dont accidentally do 0 damage 
				{
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(15, 0).RotatedBy(shotAngle + (10f * Mathj.Matht.Deg2Rad)), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), 1.0f, player.whoAmI, 0, shotAngle);
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(15, 0).RotatedBy(shotAngle), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), 1.0f, player.whoAmI, 0, shotAngle);
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(15, 0).RotatedBy(shotAngle + (-10f * Mathj.Matht.Deg2Rad)), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(Item.damage * player.GetTotalDamage(DamageClass.Magic).Multiplicative), 1.0f, player.whoAmI, 0, shotAngle);
					return false;
				}
                else
                {
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(15, 0).RotatedBy(shotAngle + (10f * Mathj.Matht.Deg2Rad)), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(scaling * player.GetTotalDamage(DamageClass.Magic).Multiplicative), 1.0f, player.whoAmI, 0, shotAngle);
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(15, 0).RotatedBy(shotAngle), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(scaling * player.GetTotalDamage(DamageClass.Magic).Multiplicative), 1.0f, player.whoAmI, 0, shotAngle);
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position, new Vector2(15, 0).RotatedBy(shotAngle + (-10f * Mathj.Matht.Deg2Rad)), ModContent.ProjectileType<Projectiles.HeatBeam>(), (int)(scaling * player.GetTotalDamage(DamageClass.Magic).Multiplicative), 1.0f, player.whoAmI, 0, shotAngle);
					return false;
				}
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
				// set to make player unable to use Item
				player.reuseDelay = 1;
				player.itemAnimation = 1;
				player.itemAnimationMax = 1;
				Item.useTime = 1;
				Item.useAnimation = 1;
				++chargeTime;
				if (chargeTime == 60)
				{
					SoundEngine.PlaySound(Acceleration.ChargeInitialSound, player.position);
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
						player.statMana -= player.GetManaCost(Item);
					}
					player.itemAnimation = 30;
					player.itemAnimationMax = 30;
					player.reuseDelay = 30;
					Item.useTime = 31;
					Item.useAnimation = 31;
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
					SoundEngine.PlaySound(Acceleration.beamRifleHyperSound, player.position);
					float shotAngle = (float)Math.Atan2(Main.MouseWorld.Y - player.position.Y, Main.MouseWorld.X - player.position.X);
					Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position + new Vector2(40, 0).RotatedBy(shotAngle), new Vector2(0, 0), ModContent.ProjectileType<Projectiles.BeamHyper>(), (int)(18 * player.GetTotalDamage(DamageClass.Magic).Multiplicative), 1.0f, player.whoAmI, 0, shotAngle);
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

		public override void NetReceive(BinaryReader reader)
		{
			base.NetReceive(reader);
			charging = reader.ReadBoolean();
			chargeTime = reader.ReadByte();
		}
	}
}
//if you're reading this yacker, thanks for believing in me. if it weren't for you offering to do a project with me i probably wouldn't be back into coding.