﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using TheDestinyMod.Projectiles.Ranged;

namespace TheDestinyMod.Items.Weapons.Ranged
{
	public class TwoTailedFox : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Two-Tailed Fox");
			Tooltip.SetDefault("Fires two rockets, one Void and one Solar\nThe Solar rocket deals damage over time, and the Void rocket disables enemies\n\"Adorably murderous.\"");
		}

		public override void SetDefaults() {
			item.damage = 520;
			item.ranged = true;
			item.width = 132;
			item.height = 48;
			item.useTime = 35;
			item.useAnimation = 35;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 4;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.rare = ItemRarityID.Red;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Gjallarhorn");
			item.autoReuse = true;
			item.shoot = 10;
			item.shootSpeed = 16f;
			item.useAmmo = AmmoID.Rocket;
			item.scale = .80f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
			Projectile.NewProjectile(position.X, position.Y - 6, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<TwoTailedVoid>(), damage, knockBack, player.whoAmI);
			Vector2 otherPert = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
			while (otherPert == perturbedSpeed) {
				otherPert = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
			}
			perturbedSpeed = otherPert;
			Projectile.NewProjectile(position.X, position.Y - 6, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<TwoTailedSolar>(), damage, knockBack, player.whoAmI);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			scale *= 0.8f;
			return true;
		}

		public override Vector2? HoldoutOffset() {
			return new Vector2(-50, -5);
		}
	}
}