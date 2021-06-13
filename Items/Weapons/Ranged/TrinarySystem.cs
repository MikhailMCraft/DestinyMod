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
			Tooltip.SetDefault("Scales with world progression\nHold down the trigger to fire\n\"The mathematics are quite complicated.\"");
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
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/AceOfSpades");
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 10f;
			item.useAnimation = 15;
			item.shoot = 10;
			item.useAmmo = AmmoID.Bullet;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.scale = 1.5f;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 30f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
                position += muzzleOffset;
            }
            Projectile.NewProjectile(position.X, position.Y - 2, speedX, speedY, ModContent.ProjectileType<Projectiles.Ranged.FusionShot>(), damage, knockBack, player.whoAmI, 7, type);
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