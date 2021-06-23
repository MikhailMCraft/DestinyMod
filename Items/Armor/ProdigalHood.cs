﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheDestinyMod.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class ProdigalHood : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("7% increased magic critical strike chance");
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = Item.sellPrice(0, 0, 90, 0);
			item.rare = ItemRarityID.Orange;
			item.defense = 8;
		}

		public override void UpdateEquip(Player player) {
			player.magicCrit += 7;
		}

		public override void UpdateArmorSet(Player player) {
			player.setBonus = "17% increased ranged damage";
			player.rangedDamage += 0.17f;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Materials.RelicIron>(), 20);
			recipe.AddIngredient(ModContent.ItemType<Materials.PlasteelPlating>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}