using Terraria;
using Terraria.ID;
using DestinyMod.Common.Items.ItemTypes;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace DestinyMod.Content.Items.Equipables.Dyes
{
    public class GambitDye : Dye
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(Item.type, new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Assets/Effects/Dyes/Gambit", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GambitDyePass"))
                    .UseColor(0, 1f, 0);
            }
        }

        public override void DestinySetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
        }
    }
}