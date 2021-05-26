﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheDestinyMod.Items.Accessories
{
    public abstract class ExoticAccessory : ModItem
    {
        public override bool CanEquipAccessory(Player player, int slot) {
            if (player.GetModPlayer<DestinyPlayer>().exoticEquipped) {
                return false;
            }
            return base.CanEquipAccessory(player, slot);
        }
    }
}