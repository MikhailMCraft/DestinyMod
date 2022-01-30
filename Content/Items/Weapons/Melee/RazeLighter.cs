using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestinyMod.Common.Items;
using DestinyMod.Content.Items.Materials;
using DestinyMod.Content.Projectiles.Weapons.Melee;

namespace DestinyMod.Content.Items.Weapons.Melee
{
    public class RazeLighter : DestinyModItem
    {
        public override void DestinySetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 7;
            Item.useAnimation = 25;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 22, silver: 22);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/RazeLighter");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RazeLighterProjectile>();
            Item.shootSpeed = 40f;
        }

        public override void AddRecipes() => CreateRecipe(1)
            .AddIngredient(ItemID.HellstoneBar, 20)
            .AddIngredient(ModContent.ItemType<RelicIron>(), 10)
            .AddTile(TileID.Anvils)
            .Register();
    }
}