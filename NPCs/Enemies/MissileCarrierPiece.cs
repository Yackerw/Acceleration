using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Acceleration;
using Terraria.ID;
using System.IO;
using Mathj;
using System;
using Terraria.Audio;

namespace Acceleration.NPCs.Enemies
{
	class MissileCarrierPiece : ModNPC
	{
		const int topOrBottom = 0;
		const int timer = 1;
		const int backFront = 2;
		const int creator = 3;

		public override void SetDefaults()
		{
			npc.lifeMax = 125;
			npc.damage = 15;
			npc.defense = 8;
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			npc.noGravity = true;
			npc.width = 86;
			npc.height = 54;
			npc.friendly = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.noTileCollide = true;
		}

		public override void AutoStaticDefaults()
		{
			base.AutoStaticDefaults();
			Main.npcFrameCount[npc.type] = 2;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frame.Y = (int)npc.ai[topOrBottom] * frameHeight;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Missile Carrier");
		}

		public override void AI()
		{
			// proper aggro handling!
			Player target = Main.player[npc.target];
			if (npc.target < 0 || npc.target == 255 || target.dead || !target.active)
			{
				npc.TargetClosest(true);
				target = Main.player[npc.target];
			}

			if (!Main.npc[(int)npc.ai[creator]].active)
			{
				npc.active = false;
				return;
			}

			MissileCarrier mc = (MissileCarrier)Main.npc[(int)npc.ai[creator]].modNPC;
			Vector2 offset = new Vector2();

			if (npc.ai[backFront] == 0)
			{
				// back
				offset.X = 16;
			} else
			{
				offset.X = -16;
			}
			if (npc.ai[topOrBottom] == 0)
			{
				// top
				offset.Y = -40;
			} else
			{
				offset.Y = 30;
			}

			npc.ai[timer] -= 1;
			if (npc.ai[timer] <= 0)
			{
				// shooty shooty time
				npc.ai[timer] = 300f;
				Vector2 projectileMom = new Vector2();
				if (npc.ai[topOrBottom] == 0)
				{
					// top
					projectileMom.Y = -10;
				} else
				{
					// bottom
					projectileMom.Y = 13;
				}
				if (npc.ai[backFront] == 0)
				{
					// back
					projectileMom.X = 10;
				} else
				{
					// front
					projectileMom.X = 15;
				}
				projectileMom.X *= npc.spriteDirection;
				float initialAngle = (float)Math.Atan2(projectileMom.Y, projectileMom.X);
				if (npc.spriteDirection == 1)
				{
					projectileMom.X += 60;
				}
				int missile = Projectile.NewProjectile(npc.position + projectileMom, new Vector2(0, 0), mod.ProjectileType("SuguriMissile"), 16, 5);
				Main.projectile[missile].rotation = initialAngle;
				initialAngle += 15 * Matht.Deg2Rad;
				projectileMom.X += 9 * npc.spriteDirection;
				projectileMom.Y += npc.ai[topOrBottom] == 0 ? -24 : 24;
				//missile = Projectile.NewProjectile(npc.position + projectileMom, new Vector2(0, 0), mod.ProjectileType("SuguriMissile"), 16, 5);
				Main.projectile[missile].rotation = initialAngle;
				Main.PlaySound(Acceleration.MissileLaunchSound);
			}

			npc.position = mc.npc.position + offset;
			npc.spriteDirection = mc.npc.spriteDirection;

			// fire off missiles

			base.AI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(npc.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(npc.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(npc.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
		}

		public override bool CheckDead()
		{
			// die particles
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(npc.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(npc.position, 80, 50, DustID.Water_BloodMoon, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
			// make sure we subtract the count from our parent
			MissileCarrier mc = (MissileCarrier)Main.npc[(int)npc.ai[creator]].modNPC;
			mc.npc.ai[MissileCarrier.launchersAlive] -= 1;
			return base.CheckDead();
		}
	}
}
