﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using DestinyMod.Common.ModPlayers;

namespace DestinyMod.Common.Items.ItemTypes
{
	public abstract class GlaiveItem : DestinyModItem
	{
		public int GlaiveCharge { get; set; }

		private int OldShoot;

		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void AutomaticSetDefaults()
		{
			base.AutomaticSetDefaults();
			Item.DamageType = DamageClass.Melee;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.knockBack = 4;
			Item.shootSpeed = 2.1f;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player) => !(player.altFunctionUse == 2 && GlaiveCharge == 0);

        public override bool? UseItem(Player player)
        {
			if (player.altFunctionUse == 2)
            {
				Item.autoReuse = true;
				Item.useStyle = ItemUseStyleID.Thrust;
				OldShoot = Item.shoot;
				Item.shoot = ProjectileID.None;
				GlaiveCharge--;
				player.itemTime = player.itemAnimation = 1;
			}
			else
            {
				Item.autoReuse = false;
				Item.useStyle = ItemUseStyleID.Rapier;
				if (OldShoot != 0)
                {
					Item.shoot = OldShoot;
                }
			}
            return true;
        }

        public override void PostUpdateRunSpeeds(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				player.maxRunSpeed /= 2;
				player.accRunSpeed /= 2;
				player.dashDelay = 10;
			}
		}

		public override ItemPlayer.IterationContext DetermineModifyDrawInfoContext(Player player) => ItemPlayer.IterationContext.HeldItem;

		public override void ModifyDrawInfo(Player player, ref PlayerDrawSet drawInfo)
		{
			if (GlaiveCharge > 0)
			{
				drawInfo.DrawDataCache.Add(new DrawData(ModContent.Request<Texture2D>("Terraria/Images/MagicPixel").Value, new Rectangle((int)(drawInfo.drawPlayer.Center.X - Main.screenPosition.X) - GlaiveCharge / 2, (int)(drawInfo.drawPlayer.Center.Y - Main.screenPosition.Y) + 30, GlaiveCharge, 10), Color.White));
			}
		}
	}
}