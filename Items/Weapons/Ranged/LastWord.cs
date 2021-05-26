using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using TheDestinyMod.Items.Materials;

namespace TheDestinyMod.Items.Weapons.Ranged
{
	public class LastWord : ModItem
	{
		public override void SetStaticDefaults() {
            DisplayName.SetDefault("The Last Word");
			Tooltip.SetDefault("Six round burst\nOnly the first shot consumes ammo\n\"Yours, until the last flame dies and all words have been spoken.\"");
			Tooltip.AddTranslation(GameCulture.Polish, "Seria sześciu rund\nTylko pierwszy strzał używa ammunicji\n\"Dopóki nie zgaśnie ostatni płomień i wszystkie słowa zostaną wypowiedziane\"");
		}

		public override void SetDefaults() {
			item.damage = 60;
			item.ranged = true;
			item.noMelee = true;
			item.autoReuse = true;
			item.rare = ItemRarityID.Yellow;
			item.knockBack = 0;
			item.width = 40;
			item.height = 20;
			item.useTime = 4;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TheLastWord");
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shootSpeed = 30f;
			item.useAnimation = 24;
			item.shoot = 10;
			item.scale = .80f;
			item.useAmmo = AmmoID.Bullet;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.reuseDelay = 24;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Ectoplasm, 20);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			scale *= 0.8f;
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(3));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
            return true;
        }

		public override bool ConsumeAmmo(Player player) {
			return !(player.itemAnimation < item.useAnimation - 2);
		}

		public override Vector2? HoldoutOffset() {
			return new Vector2(5, 2);
		}
	}
}