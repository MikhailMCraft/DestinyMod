﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace TheDestinyMod.Items.Weapons.Ranged
{
	public class TrinarySystem : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Hold down the trigger to fire\nScales with world progression\n\"The mathematics are quite complicated.\"");
		}

		public override void SetDefaults() {
			item.damage = 5;
			item.crit = 1;
			item.ranged = true;
			item.noMelee = true;
			item.channel = true;
			item.rare = ItemRarityID.Green;
			item.knockBack = 0;
			item.width = 37;
			item.height = 21;
			item.useTime = 15;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 10f;
			item.useAnimation = 15;
			item.shoot = 10;
			item.useAmmo = AmmoID.Bullet;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.scale = 1.5f;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			DestinyHelper.FireFusionProjectile(player, new Vector2(position.X, position.Y - 2), new Vector2(speedX, speedY), damage, knockBack, 7, type);
            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat) {
			if (Main.hardMode) {
				flat += 20;
			}
			if (NPC.downedMechBossAny) {
				flat += 10;
			}
			if (NPC.downedPlantBoss) {
				flat += 10;
			}
        }

        public override void GetWeaponCrit(Player player, ref int crit) {
			crit = 1;
			if (Main.hardMode) {
				crit = 3;
			}
			if (NPC.downedMechBossAny) {
				crit = 5;
			}
			if (NPC.downedPlantBoss) {
				crit = 7;
			}
		}

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			scale *= 1.5f;
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override Vector2? HoldoutOffset() {
			return new Vector2(-1, 0);
		}
	}
}