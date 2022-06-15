using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Mathj;
using System;
using Terraria.DataStructures;

namespace Acceleration.Items.Weapons.Ranged
{
	class BugCannon : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("\nFires bug mines"
				+ "\nHYPER: Rings of mines");
		}

		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 64;
			Item.height = 32;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.channel = true; //Channel so that you can held the weapon [Important]
			Item.knockBack = 6;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item11;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.Ranged.PreHardMode.FriendlyBugMine>();
			Item.shootSpeed = 8.0f;
			Item.autoReuse = true;
		}

		public bool hyper;
		public int hyperTimer;

		public override void HoldItem(Player player)
		{
			AcceleratePlayer ap = player.GetModPlayer<AcceleratePlayer>();
			if (!hyper && ap.hyperButton && !ap.prevHyperButton && ap.hyper > 1.0f && player.reuseDelay <= 0)
			{
				hyper = true;
				hyperTimer = 60;
				ap.SetupHyper();
			}
			if (hyper && hyperTimer > 0)
			{
				--hyperTimer;
				player.reuseDelay = 1;
				player.itemAnimation = 1;
				player.itemAnimationMax = 1;
				// just spawn a whole buncha bullets
				float bulletAngle = (hyperTimer / 60.0f) * (float)Math.PI * 2;
				Vector2 spawnPos = new Vector2(50, 0).RotatedBy(bulletAngle);
				Vector2 spawnVel = new Vector2(8, 0).RotatedBy(bulletAngle);
				Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position + spawnPos, spawnVel, ModContent.ProjectileType<Projectiles.Weapons.Ranged.PreHardMode.FriendlyBugMine>(), (int)(Item.damage * 1.5f * player.GetTotalDamage(DamageClass.Ranged).Multiplicative), Item.knockBack, player.whoAmI, -1);
				Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.position - spawnPos, -spawnVel, ModContent.ProjectileType<Projectiles.Weapons.Ranged.PreHardMode.FriendlyBugMine>(), (int)(Item.damage * 1.5f * player.GetTotalDamage(DamageClass.Ranged).Multiplicative), Item.knockBack, player.whoAmI, -1);
				SoundEngine.PlaySound(SoundID.Item11, player.position);
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// no shooty!
			if (hyper)
			{
				if (hyperTimer <= 0)
				{
					hyper = false;
				}
				return false;
			}
			return true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity = velocity.RotatedByRandom(0.2f);
		}

	}
}
