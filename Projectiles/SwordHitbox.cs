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
            projectile.width = 11;
            projectile.height = 11;
            projectile.alpha = 0;
            projectile.timeLeft = 2;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.melee = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            AcceleratePlayer ap = Main.player[projectile.owner].GetModPlayer<AcceleratePlayer>();
            ap.hyper += 0.05f;
            if (ap.hyper > 3.0f)
            {
                ap.hyper = 3.0f;
            }
            // grant some invuln frames
            if (ap.player.HeldItem.type == ModContent.ItemType<BeamSword>())
			{
                BeamSword bm = (BeamSword)ap.player.HeldItem.modItem;
                if (bm.swingInvulnDelay <= 0)
                {
                    ap.player.immune = true;
                    ap.player.immuneTime = 90;
                    for (int i = 0; i < ap.player.hurtCooldowns.Length; ++i)
                    {
                        ap.player.hurtCooldowns[i] = 90;
                    }
                    // put a delay
                    bm.swingInvulnDelay = 300;
                }
            }
        }

        public override void AI()
		{
            projectile.rotation = projectile.ai[0];
            if (projectile.ai[1] != 0)
			{
                projectile.scale = projectile.ai[1];
                projectile.width = 11 * (int)projectile.scale;
                projectile.height = 11 * (int)projectile.scale;
			}
		}
	}
}
