using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Graphics;
using DestinyMod.Content.Items.Weapons.Ranged.Omolon;
using DestinyMod.Content.Items.Weapons.Ranged.Suros;
using DestinyMod.Content.Items.Weapons.Ranged.Hakke;
using DestinyMod.Core.UI;
using DestinyMod.Content.Items.Engrams;
using Terraria.GameContent.UI;
using DestinyMod.Common.UI;
using DestinyMod.Common.ModPlayers;
using DestinyMod.Core.Extensions;
using DestinyMod.Core.Utils;

namespace DestinyMod.Content.UI.Cryptarch
{
	public class CryptarchUI : DestinyModUIState
	{
		public VanillaItemSlotWrapper InputSlot { get; private set; }

		public bool TickPlayed { get; private set; }

		public override void PreLoad(ref string name)
		{
			AutoSetState = false;
			AutoAddHandler = true;
		}

		public override UIHandler Load() => new UIHandler(UserInterface, "Vanilla: Inventory", LayerName);

		public override void OnInitialize()
		{
			InputSlot = new VanillaItemSlotWrapper(ItemSlot.Context.PrefixItem, ItemSlot.Context.BankItem, 0.85f)
			{
				ValidItemFunc = item => item.IsAir
				|| !item.IsAir && (item.type == ModContent.ItemType<CommonEngram>()
					|| item.type == ModContent.ItemType<UncommonEngram>()
					|| item.type == ModContent.ItemType<RareEngram>())
					// || item.type == ModContent.ItemType<LegendaryEngram>()
					// || item.type == ModContent.ItemType<ExoticEngram>())
			};
			Left.Pixels = 50;
			Top.Pixels = 270;
			Append(InputSlot);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Player player = Main.LocalPlayer;
			
			if (player.talkNPC == -1 || Main.npc[player.talkNPC].type != ModContent.NPCType<NPCs.TownNPC.Cryptarch>())
			{
				player.QuickSpawnItem(player.GetSource_GiftOrReward("Cryptarch"), InputSlot.Item.type, InputSlot.Item.stack);
				ModContent.GetInstance<CryptarchUI>().UserInterface.SetState(null);
			}
		}

		public static List<int> GetLootTable(int engramType)
		{
			List<int> output = new List<int>();
			if (engramType == ModContent.ItemType<CommonEngram>())
			{
				output.Add(ModContent.ItemType<HakkeAutoRifle>(),
					ModContent.ItemType<HakkePulseRifle>(),
					ModContent.ItemType<HakkeScoutRifle>(),
					ModContent.ItemType<HakkeShotgun>(),
					ModContent.ItemType<HakkeSidearm>(),
					ModContent.ItemType<HakkeSniper>(),
					ModContent.ItemType<HakkeRocketLauncher>(),
					ModContent.ItemType<HakkeGrenadeLauncher>(),
					ModContent.ItemType<HakkeHandcannon>());
			}
			else if (engramType == ModContent.ItemType<UncommonEngram>())
			{
				output.Add(ModContent.ItemType<OmolonAutoRifle>(),
					ModContent.ItemType<OmolonPulseRifle>(),
					ModContent.ItemType<OmolonScoutRifle>(),
					ModContent.ItemType<OmolonShotgun>(),
					ModContent.ItemType<OmolonSidearm>(),
					ModContent.ItemType<OmolonSniper>(),
					ModContent.ItemType<OmolonRocketLauncher>(),
					ModContent.ItemType<OmolonGrenadeLauncher>(),
					ModContent.ItemType<OmolonHandcannon>());
			}
			else if (engramType == ModContent.ItemType<RareEngram>())
			{
				output.Add(ModContent.ItemType<SurosAutoRifle>(),
					ModContent.ItemType<SurosPulseRifle>(),
					ModContent.ItemType<SurosScoutRifle>(),
					ModContent.ItemType<SurosShotgun>(),
					ModContent.ItemType<SurosSidearm>(),
					ModContent.ItemType<SurosSniper>(),
					ModContent.ItemType<SurosRocketLauncher>(),
					ModContent.ItemType<SurosGrenadeLauncher>(),
					ModContent.ItemType<SurosHandcannon>());
			}
			return output;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			Main.craftingHide = true;

			Vector2 slotPosition = new Vector2(50, 270);

			if (InputSlot.Item.IsAir)
			{
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Place an Engram here to decrypt it",
					slotPosition + new Vector2(50, 0), new Color(new Vector4(Main.mouseTextColor)), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
				return;
			}

			Player player = Main.LocalPlayer;
			NPCPlayer npcPlayer = player.GetModPlayer<NPCPlayer>();
			DynamicSpriteFont mouseFont = FontAssets.MouseText.Value;
			Color mouseTextColor = new Color(new Vector4(Main.mouseTextColor));
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, mouseFont, "Decrypt " + InputSlot.Item.Name, slotPosition + new Vector2(50, 0), mouseTextColor, 0f, Vector2.Zero, Vector2.One);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, mouseFont, "Potential drops:", slotPosition + new Vector2(100, 30), mouseTextColor, 0f, Vector2.Zero, Vector2.One);
			TextSnippet extSnip = null;

			List<int> lootTable = GetLootTable(InputSlot.Item.type);
			for (int itemCount = 0; itemCount < lootTable.Count - 1; itemCount++)
			{
				int itemType = lootTable[itemCount];
				ModItem modItem = ModContent.GetModItem(itemType);
				bool decrypted = npcPlayer.DecryptedItems.Contains(itemType);
				int drawY = itemCount * 25 + 55;

                TextSnippet[] text = ChatManager.ParseMessage(decrypted ? "[i:" + itemType + "] " + modItem.DisplayName.GetTranslation(Language.ActiveCulture) : "???", decrypted ? ItemRarity.GetColor(modItem.Item.rare) : Color.White).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, mouseFont, text, slotPosition + new Vector2(100, drawY), 0f, Vector2.Zero, Vector2.One, out int hoveredSnippet);
				if (hoveredSnippet > -1)
				{
					extSnip = text[0];
				}
			}

			extSnip?.OnHover();

			Point decryptPosition = new Point((int)(slotPosition.X + 70), (int)(slotPosition.Y + 40));
			bool hoveringOverDecryptButton = !PlayerInput.IgnoreMouseInterface
				&& RectangleUtils.RectangleFromCenter(decryptPosition.X, decryptPosition.Y, 30, 30).Contains(new Point(Main.mouseX, Main.mouseY));
			Texture2D decryptTexture = TextureAssets.Reforge[hoveringOverDecryptButton ? 1 : 0].Value;
			Main.spriteBatch.Draw(decryptTexture, decryptPosition.ToVector2(), null, Color.White, 0f, decryptTexture.Size() / 2f, 0.8f, SpriteEffects.None, 0f);

			if (!hoveringOverDecryptButton)
			{
				TickPlayed = false;
				return;
			}

			Main.hoverItemName = "Decrypt";

			if (!TickPlayed)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				TickPlayed = true;
			}

			player.mouseInterface = true;

			bool GiveEngramItem()
			{
				if (InputSlot.Item.stack-- <= 0)
				{
					return false;
				}

				if (Main.rand.Next(20) > lootTable.Count)
				{
					Player player = Main.LocalPlayer;
					if (InputSlot.Item.type == ModContent.ItemType<CommonEngram>())
					{
						player.QuickSpawnItem(player.GetSource_GiftOrReward("Cryptarch"), ItemID.SilverCoin, Main.rand.Next(1, 10));
					}
					else if (InputSlot.Item.type == ModContent.ItemType<UncommonEngram>())
					{
						player.QuickSpawnItem(player.GetSource_GiftOrReward("Cryptarch"), ItemID.SilverCoin, Main.rand.Next(10, 50));
					}
					else if (InputSlot.Item.type == ModContent.ItemType<RareEngram>())
					{
						player.QuickSpawnItem(player.GetSource_GiftOrReward("Cryptarch"), ItemID.GoldCoin, Main.rand.Next(1, 3));
					}
				}
				else
				{
					int random = Main.rand.Next(lootTable);
					player.QuickSpawnItem(player.GetSource_GiftOrReward("Cryptarch"), random);
					if (!npcPlayer.DecryptedItems.Contains(random))
					{
						npcPlayer.DecryptedItems.Add(random);
					}
				}

				return true;
			}

			if (Main.mouseLeftRelease && Main.mouseLeft)
			{
				while (GiveEngramItem());

				InputSlot.Item.TurnToAir();
				SoundEngine.PlaySound(SoundID.Item37);
			}
			else if (Main.mouseRightRelease && Main.mouseRight)
			{
				GiveEngramItem();
				SoundEngine.PlaySound(SoundID.Item37);
			}
		}
	}
}