using DestinyMod.Common.GlobalItems;
using DestinyMod.Common.Items.Modifiers;
using Terraria;

namespace DestinyMod.Content.Items.Perks.Weapon.Barrels
{
    public class ChamberedCompensator : ItemPerk
    {
        public override void SetDefaults()
        {
            DisplayName = "Chambered Compensator";
            Description = "Stable barrel attachment."
                + "\n- Increases stability"
                + "\n- Moderately controls recoil"
                + "\n- Slightly decreases bullet speed";
        }

        public override void Update(Player player)
        {
            if (SocketedItem == null)
            {
                return;
            }

            SocketedItem.GetGlobalItem<ItemDataItem>().Stability += 10;
            SocketedItem.GetGlobalItem<ItemDataItem>().Recoil += 10;
        }
    }
}