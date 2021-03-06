using DestinyMod.Common.Items.ItemTypes;
using DestinyMod.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestinyMod.Content.Items.Weapons.Ranged
{
	public class Sunshot : Gun
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Fires explosive rounds"
			+ "\nRounds highlight enemies on contact"
			+ "\n'Can't outrun the sunrise. -Liu Feng'");
		}

		public override void DestinySetDefaults()
		{
			Item.damage = 84;
			Item.rare = ItemRarityID.Red;
			Item.knockBack = 0;
			Item.useTime = 20;
			Item.crit = 10;
			Item.UseSound = new SoundStyle("DestinyMod/Assets/Sounds/Item/Weapons/Ranged/AceOfSpades");
			Item.useAnimation = 20;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, new Vector2(position.X, position.Y - 4), velocity, ModContent.ProjectileType<SunshotBullet>(), damage, knockback, player.whoAmI);
			return false;
		}

		public override Vector2? HoldoutOffset() => new Vector2(3, -1);
	}
}