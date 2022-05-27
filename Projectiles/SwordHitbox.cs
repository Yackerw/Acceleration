using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Mathj;
using Acceleration.Items.Weapons.Melee;

namespace Acceleration.Projectiles
{
	class SwordHitbox : ModProjectile
	{
		public override void SetDefaults()
		{
            Projectile.width = 11;
            Projectile.height = 11;
            Projectile.alpha = 0;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            AcceleratePlayer ap = Main.player[Projectile.owner].GetModPlayer<AcceleratePlayer>();
            ap.hyper += 0.05f;
            if (ap.hyper > 3.0f)
            {
                ap.hyper = 3.0f;
            }
            // grant some invuln frames
            if (ap.Player.HeldItem.type == ModContent.ItemType<BeamSword>())
			{
                BeamSword bm = (BeamSword)ap.Player.HeldItem.ModItem;
                if (bm.swingInvulnDelay <= 0)
                {
                    ap.Player.immune = true;
                    ap.Player.immuneTime = 90;
                    for (int i = 0; i < ap.Player.hurtCooldowns.Length; ++i)
                    {
                        ap.Player.hurtCooldowns[i] = 90;
                    }
                    // put a delay
                    bm.swingInvulnDelay = 300;
                }
            }
        }

        public override void AI()
		{
            Projectile.rotation = Projectile.ai[0];
            if (Projectile.ai[1] != 0)
			{
                Projectile.scale = Projectile.ai[1];
                Projectile.width = 11 * (int)Projectile.scale;
                Projectile.height = 11 * (int)Projectile.scale;
			}
		}
	}
}
