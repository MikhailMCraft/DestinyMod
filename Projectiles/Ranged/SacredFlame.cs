﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace TheDestinyMod.Projectiles.Ranged
{
	public class SacredFlame : ModProjectile
	{
		public override void SetDefaults() {
			projectile.CloneDefaults(ProjectileID.FireArrow);
			aiType = ProjectileID.FireArrow;
			projectile.width = 2;
			projectile.height = 34;
			projectile.damage = 10;
			projectile.timeLeft = 500;
			projectile.friendly = true;
			projectile.ranged = true;
		}

        public override void Kill(int timeLeft) {
			Main.PlaySound(SoundID.Item10, projectile.position);
			Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
			Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
		}

        public override void AI() {
			if (projectile.localAI[0] == 0f) {
				AdjustMagnitude(ref projectile.velocity);
				projectile.localAI[0] = 1f;
			}
			Vector2 move = Vector2.Zero;
			float distance = 400f;
			bool target = projectile.HomeInOnNPC(distance, ref move);
			if (target) {
				AdjustMagnitude(ref move);
				projectile.velocity = (10 * projectile.velocity + move) / 11f;
				AdjustMagnitude(ref projectile.velocity);
			}
		}

		private void AdjustMagnitude(ref Vector2 vector) {
			float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			if (magnitude > 15f) {
				vector *= 15f / magnitude;
			}
		}
	}
}