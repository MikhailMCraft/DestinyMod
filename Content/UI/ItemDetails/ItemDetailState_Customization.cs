﻿using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using DestinyMod.Core.UI;
using DestinyMod.Common.UI;
using System.Collections.Generic;
using DestinyMod.Common.Items.PerksAndMods;
using DestinyMod.Common.GlobalItems;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.UI;
using DestinyMod.Content.UI.MouseText;

namespace DestinyMod.Content.UI.ItemDetails
{
	public partial class ItemDetailsState : DestinyModUIState
	{
		public UIText CosmeticsTitle { get; private set; }

		public UISeparator CosmeticsSeparator { get; private set; }

		public UIItemSlotWithBackground DyeSlot { get; private set; }

		public int InitialiseCosmeticsSection(int yPos)
		{
			if (!InspectedItemData.Shaderable)
            {
				return yPos;
            }

			CosmeticsTitle = new UIText("Item Cosmetics");
			CosmeticsTitle.Left.Pixels = 10;
			CosmeticsTitle.Top.Pixels = yPos;
			MasterBackground.Append(CosmeticsTitle);

			CosmeticsSeparator = new UISeparator();
			CosmeticsSeparator.Left.Pixels = 10;
			CosmeticsSeparator.Top.Pixels = yPos += 20;
			CosmeticsSeparator.Width.Pixels = 300f;
			CosmeticsSeparator.Height.Pixels = 2f;
			CosmeticsSeparator.Color = BaseColor_Light;
			MasterBackground.Append(CosmeticsSeparator);

			Texture2D dyeSlotBackground = ModContent.Request<Texture2D>("DestinyMod/Content/UI/ItemDetails/DyeSlot", AssetRequestMode.ImmediateLoad).Value;
			DyeSlot = new UIItemSlotWithBackground(dyeSlotBackground, isItemValid: (item) => item.dye > 0);
			DyeSlot.Left.Pixels = 10;
			DyeSlot.Top.Pixels = yPos += 8;
			DyeSlot.BlockItemInput = false;
			DyeSlot.Item = InspectedItem.GetGlobalItem<ItemDataItem>().Shader;
			DyeSlot.OnUpdate += HandleDyeSlotMouseText;
            DyeSlot.OnUpdateItem += UpdateItemShader;
			MasterBackground.Append(DyeSlot);
			return yPos + (int)DyeSlot.Height.Pixels + 10;
		}

        public void HandleDyeSlotMouseText(UIElement affectedElement)
		{
			if (affectedElement is not UIItemSlotWithBackground uIItemSlotWithBackground || !uIItemSlotWithBackground.ContainsPoint(Main.MouseScreen))
			{
				return;
			}

			string title = uIItemSlotWithBackground.Item.IsAir ? "Default Shader" : uIItemSlotWithBackground.Item.HoverName;
			string subTitle = uIItemSlotWithBackground.Item.IsAir ? "None" : "Shader";
			MouseText_TitleAndSubtitle.UpdateData(title, subTitle);

			MouseTextState mouseTextState = ModContent.GetInstance<MouseTextState>();
			mouseTextState.AppendToMasterBackground(MouseText_TitleAndSubtitle);
		}

		public void UpdateItemShader(UIItemSlotWithBackground uIItemSlotWithBackground)
		{
			InspectedItem.GetGlobalItem<ItemDataItem>().Shader = uIItemSlotWithBackground.Item;
		}
	}
}