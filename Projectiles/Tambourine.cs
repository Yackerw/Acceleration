﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Acceleration.Projectiles
{
	class Tambourine : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.alpha = 0;
            projectile.timeLeft = 360;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.light = 0.3f;
        }
        public override void AI()
        {
            base.AI();
            // face our momentum
            projectile.rotation += 25*Mathj.Matht.Deg2Rad;
            Vector2 tamspeed = projectile.velocity;
            tamspeed.Normalize();
            if (projectile.ai[0] == 0 && (projectile.velocity.X * projectile.velocity.X) + (projectile.velocity.Y * projectile.velocity.Y) > 16f) //tambo slows down over time 
            {
                projectile.velocity -= 0.2f * tamspeed;
            }
            if (projectile.ai[0] == 0 && (projectile.velocity.X * projectile.velocity.X) + (projectile.velocity.Y * projectile.velocity.Y) < 16f) //slow down the slow down for some juicy boss dps 
            {
                projectile.velocity -= 0.05f * tamspeed;
            }
            if (projectile.ai[0] == 1) //tambo return 
            {
                projectile.velocity -= -0.1f * tamspeed;
                projectile.tileCollide = true; 
            }
            if (Vector2.Dot(projectile.velocity, tamspeed) < 0) //dot product multiplies 
            {
                projectile.ai[0] = 1;
            }
        }
    }
}