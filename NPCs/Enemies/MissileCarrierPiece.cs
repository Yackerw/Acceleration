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
			NPC.lifeMax = 125;
			NPC.damage = 15;
			NPC.defense = 8;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.width = 86;
			NPC.height = 54;
			NPC.friendly = false;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath14;
			NPC.noTileCollide = true;
		}

		public override void AutoStaticDefaults()
		{
			base.AutoStaticDefaults();
			Main.npcFrameCount[NPC.type] = 2;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = (int)NPC.ai[topOrBottom] * frameHeight;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Missile Carrier");
		}

		public override void AI()
		{
			// proper aggro handling!
			Player target = Main.player[NPC.target];
			if (NPC.target < 0 || NPC.target == 255 || target.dead || !target.active)
			{
				NPC.TargetClosest(true);
				target = Main.player[NPC.target];
			}

			if (!Main.npc[(int)NPC.ai[creator]].active)
			{
				NPC.active = false;
				return;
			}

			MissileCarrier mc = (MissileCarrier)Main.npc[(int)NPC.ai[creator]].ModNPC;
			Vector2 offset = new Vector2();

			if (NPC.ai[backFront] == 0)
			{
				// back
				offset.X = 16;
			} else
			{
				offset.X = -16;
			}
			if (NPC.ai[topOrBottom] == 0)
			{
				// top
				offset.Y = -40;
			} else
			{
				offset.Y = 30;
			}

			NPC.ai[timer] -= 1;
			if (NPC.ai[timer] <= 0)
			{
				// shooty shooty time
				NPC.ai[timer] = 300f;
				Vector2 projectileMom = new Vector2();
				if (NPC.ai[topOrBottom] == 0)
				{
					// top
					projectileMom.Y = -10;
				} else
				{
					// bottom
					projectileMom.Y = 13;
				}
				if (NPC.ai[backFront] == 0)
				{
					// back
					projectileMom.X = 10;
				} else
				{
					// front
					projectileMom.X = 15;
				}
				projectileMom.X *= NPC.spriteDirection;
				float initialAngle = (float)Math.Atan2(projectileMom.Y, projectileMom.X);
				if (NPC.spriteDirection == 1)
				{
					projectileMom.X += 60;
				}
				int missile = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position + projectileMom, new Vector2(0, 0), ModContent.ProjectileType<Projectiles.SuguriMissile>(), 16, 5);
				Main.projectile[missile].rotation = initialAngle;
				initialAngle += 15 * Matht.Deg2Rad;
				projectileMom.X += 9 * NPC.spriteDirection;
				projectileMom.Y += NPC.ai[topOrBottom] == 0 ? -24 : 24;
				//missile = Projectile.NewProjectile(NPC.position + projectileMom, new Vector2(0, 0), mod.ProjectileType("SuguriMissile"), 16, 5);
				Main.projectile[missile].rotation = initialAngle;
				SoundEngine.PlaySound(Acceleration.MissileLaunchSound);
			}

			NPC.position = mc.NPC.position + offset;
			NPC.spriteDirection = mc.NPC.spriteDirection;

			// fire off missiles

			base.AI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(NPC.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(NPC.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			Dust.NewDust(NPC.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
		}

		public override bool CheckDead()
		{
			// die particles
			for (int i = 0; i < 20; ++i)
			{
				Dust.NewDust(NPC.position, 80, 50, DustID.Iron, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
				Dust.NewDust(NPC.position, 80, 50, DustID.Water_BloodMoon, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(0, 6));
			}
			// make sure we subtract the count from our parent
			MissileCarrier mc = (MissileCarrier)Main.npc[(int)NPC.ai[creator]].ModNPC;
			mc.NPC.ai[MissileCarrier.launchersAlive] -= 1;
			return base.CheckDead();
		}
	}
}
