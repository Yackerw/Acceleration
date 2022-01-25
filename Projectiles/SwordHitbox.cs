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
        }

        public override void AI()
		{
            projectile.rotation = projectile.ai[0];
            if (projectile.ai[1] != 0)
			{
                projectile.scale = projectile.ai[1];
                projectile.width *= (int)projectile.scale;
                projectile.height *= (int)projectile.scale;
			}
		}
	}
}
