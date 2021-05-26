﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheDestinyMod.Projectiles.Ranged;

namespace TheDestinyMod.Items.Weapons.Ranged
{
    public class GravitonLance : ModItem
	{
		internal int shot;

		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Second shot of a burst deals double damage\nKills with this bullet summon a seeking projectile\n\"Think of space-time as a tapestry on a loom. This weapon is the needle.\"");
		}

		public override void SetDefaults() {
			item.damage = 45;
			item.ranged = true;
			item.width = 76;
			item.height = 40;
			item.useTime = 6;
			item.useAnimation = 18;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 4;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.rare = ItemRarityID.Yellow;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GravitonLance");
			item.shoot = 10;
			item.shootSpeed = 16f;
			item.useAmmo = AmmoID.Bullet;
			item.scale = .80f;
			item.reuseDelay = 5;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			shot++;
			if (shot == 2) {
				Projectile.NewProjectile(position.X, position.Y - 5, speedX, speedY, ModContent.ProjectileType<GravitonBullet>(), damage * 3, knockBack, player.whoAmI);
			}
			else {
				Projectile.NewProjectile(position.X, position.Y - 5, speedX, speedY, type, damage, knockBack, player.whoAmI);
			}
			if (shot == 3) {
				shot = 0;
			}
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			scale *= 0.8f;
			return true;
		}

		public override Vector2? HoldoutOffset() {
			return new Vector2(-20, -2);
		}
	}
}