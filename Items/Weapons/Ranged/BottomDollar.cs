﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;

namespace TheDestinyMod.Items.Weapons.Ranged {
	public class BottomDollar : ModItem {

		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Scales with world progression\n\"Never count yourself out.\"");
		}

		public override void SetDefaults() {
			item.damage = 15;
			item.crit = 5;
			item.ranged = true;
			item.noMelee = true;
			item.rare = ItemRarityID.Green;
			item.knockBack = 0;
			item.width = 58;
			item.height = 28;
			item.useTime = 20;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/HandCannon120");
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 10f;
			item.useAnimation = 20;
			item.shoot = 10;
			item.useAmmo = AmmoID.Bullet;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.scale = 0.8f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			Projectile.NewProjectile(position.X, position.Y - 5, speedX, speedY, type, damage, knockBack, player.whoAmI);
			return false;
		}

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat) {
			if (Main.hardMode) {
				flat += 25;
			}
        }

        public override void GetWeaponCrit(Player player, ref int crit) {
			crit = 5;
			if (Main.hardMode) {
				crit = 8;
			}
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			scale *= 0.8f;
            return true;
        }

        public override Vector2? HoldoutOffset() {
			return new Vector2(0, -2);
		}
	}
}