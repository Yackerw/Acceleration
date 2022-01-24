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
    class BeamSword : ModProjectile
    {
        int animTimer;

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 10;
            projectile.alpha = 0;
            projectile.timeLeft = 40;
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
            // stick to the player
            Vector2 playerPos = Main.player[projectile.owner].position;
            projectile.spriteDirection = (int)projectile.ai[1];
            switch (projectile.ai[0])
			{
                case 0:
                    projectile.rotation = Matht.Lerp(80 * Matht.Deg2Rad, 10 * Matht.Deg2Rad, (float)Math.Sin((projectile.timeLeft / 40.0f) * (Math.PI / 2)));
                    Vector2 rotPos = new Vector2(20 * projectile.spriteDirection, -20).RotatedBy(projectile.rotation);
                    projectile.position = playerPos + rotPos + new Vector2(20 * projectile.spriteDirection, 0);
                    break;
			}
            ++animTimer;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            AccelerationHelper.DrawSprite("Projectiles/BeamSword", projectile.position, 0, 48, Color.White, projectile.rotation, spriteBatch);
            return false;
		}
	}
}