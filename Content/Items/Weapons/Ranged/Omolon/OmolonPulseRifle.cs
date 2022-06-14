using DestinyMod.Content.Recipes.RecipeGroups;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestinyMod.Content.Items.Weapons.Ranged.Omolon
{
	public class OmolonPulseRifle : OmolonWeapon
	{
		public override void SetStaticDefaults() => Tooltip.SetDefault("Three round burst"
			+ "\nStandard Omolon Pulse Rifle");

		public override void DestinySetDefaults()
		{
			Item.damage = 18;
			Item.useTime = 4;
			Item.useAnimation = 12;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = new SoundStyle("DestinyMod/Assets/Sounds/Item/Weapons/Ranged/JadeRabbitBurst");
			Item.shootSpeed = 16f;
			DestinyModReuseDelay = 14;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, new Vector2(position.X, position.Y - 2), velocity, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-5, 3);

		public override void AddRecipes() => CreateRecipe(1)
			.AddIngredient(ItemID.SoulofLight, 15)
			.AddRecipeGroup(ModContent.GetInstance<AdamantiteOrTitaniumBars>().RecipeGroup, 8)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}