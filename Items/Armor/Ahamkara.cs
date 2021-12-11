﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheDestinyMod.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class Ahamkara : ExoticArmor, IClassArmor
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Skull of Dire Ahamkara");
			Tooltip.SetDefault("Increases movement speed by 10%\n\"Reality is of the finest flesh, oh bearer mine. And are you not...hungry?\"");
		}

		public override void SetDefaults() {
			item.width = 22;
			item.height = 20;
			item.rare = ItemRarityID.Yellow;
			item.value = Item.sellPrice(gold: 1);
			item.defense = 20;
		}

        public override void UpdateEquip(Player player) {
			player.DestinyPlayer().exoticEquipped = true;
			player.moveSpeed += 0.1f;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) => tooltips.Add(DestinyHelper.GetRestrictedClassTooltip(DestinyClassType.Warlock));

		public DestinyClassType ArmorClassType() => DestinyClassType.Warlock;
	}
}