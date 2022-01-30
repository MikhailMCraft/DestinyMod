using Microsoft.Xna.Framework;
using DestinyMod.Common.Items.ItemTypes;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestinyMod.Content.Items.Weapons.Ranged
{
	public class WhisperOfTheWorm : Gun
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Whisper of the Worm");
			Tooltip.SetDefault("\"A Guardian's power makes a rich feeding ground. Do not be revolted.\"");
		}

		public override void DestinySetDefaults()
		{
			Item.damage = 250;
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/WhisperOfTheWorm");
			Item.shootSpeed = 16f;
			DestinyModReuseDelay = 15;
		}

		public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (Main.rand.NextBool(4))
			{
				Dust.NewDust(position += Vector2.Normalize(velocity) * 90f, 1, 1, DustID.WhiteTorch);
			}
			return true;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, -2);
	}
}