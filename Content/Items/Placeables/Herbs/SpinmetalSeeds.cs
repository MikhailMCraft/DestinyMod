using Terraria;
using Terraria.ModLoader;
using DestinyMod.Common.Items.ItemTypes;

namespace DestinyMod.Content.Items.Placeables.Herbs
{
	public class SpinmetalSeeds : TileItem
	{
		public override void DestinySetDefaults()
		{
			Item.maxStack = 99;
			Item.placeStyle = 0;
			Item.value = Item.buyPrice(copper: 80);
			Item.createTile = ModContent.TileType<Tiles.Herbs.Spinmetal>();
		}
	}
}