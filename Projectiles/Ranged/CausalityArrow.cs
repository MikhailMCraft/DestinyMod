﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace TheDestinyMod.Projectiles.Ranged
{
	public class CausalityArrow : ModProjectile
	{
		public override void SetDefaults() {
			projectile.CloneDefaults(ProjectileID.FireArrow);
			aiType = ProjectileID.FireArrow;
			projectile.width = 6;
			projectile.height = 28;
			projectile.damage = 110;
			projectile.friendly = true;
			projectile.ranged = true;
		}

		public override void Kill(int timeLeft) {
			Main.PlaySound(SoundID.Item10, projectile.position);
			Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
			Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
		}
    }
}