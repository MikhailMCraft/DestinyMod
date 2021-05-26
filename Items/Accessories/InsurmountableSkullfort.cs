﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheDestinyMod.Items.Accessories
{
	[AutoloadEquip(EquipType.Face)]
	public class InsurmountableSkullfort : ExoticAccessory
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("An Insurmountable Skullfort");
			Tooltip.SetDefault("\"BRAINVAULT Sigma-ACTIUM-IX Cranial Dreadnought (Invincible Type)\"");
		}

		public override void SetDefaults() {
			item.width = 26;
			item.height = 28;
			item.rare = ItemRarityID.Yellow;
			item.value = Item.sellPrice(gold: 1);
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetModPlayer<DestinyPlayer>().exoticEquipped = true;
			player.accRunSpeed = 6f; // The player's maximum run speed with accessories
			player.moveSpeed += 0.05f; // The acceleration multiplier of the player's movement speed
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			if (!Main.LocalPlayer.GetModPlayer<DestinyPlayer>().titan && DestinyConfig.Instance.restrictClassItems) {
				tooltips.Add(new TooltipLine(mod, "HasClass", "You must be a Titan to equip this") { overrideColor = new Color(255, 0, 0) });
			}
		}

		public override bool CanEquipAccessory(Player player, int slot) {
			if (DestinyConfig.Instance.restrictClassItems) {
				return Main.LocalPlayer.GetModPlayer<DestinyPlayer>().titan;
			}
			return base.CanEquipAccessory(player, slot);
		}
	}
}