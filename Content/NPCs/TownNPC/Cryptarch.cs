using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent.Events;
using DestinyMod.Content.Items.Engrams;
using DestinyMod.Content.Items.Weapons.Ranged.Khvostov;
using DestinyMod.Common.ModPlayers;
using DestinyMod.Common.NPCs.NPCTypes;
using DestinyMod.Content.UI.Cryptarch;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using System.Collections.Generic;

namespace DestinyMod.Content.NPCs.TownNPC
{
	[AutoloadHead]
	public class Cryptarch : GenericTownNPC
	{
		public override void DestinySetStaticDefaults()
		{
			DisplayName.SetDefault("Cryptarch");
		}

        public override ITownNPCProfile TownNPCProfile()
        {
            return base.TownNPCProfile();
        }

		public override List<string> SetNPCNameList() => new List<string> { "Master Rahool" };

		public override void DestinySetDefaults()
		{
			NPC.width = 26;
			NPC.height = 46;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				new FlavorTextBestiaryInfoElement("Mods.DestinyMod.Bestiary.Cryptarch")
			});
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			for (int playerCount = 0; playerCount < Main.maxPlayers; playerCount++)
			{
				Player player = Main.player[playerCount];
				if (!player.active || player.CountItem(ModContent.ItemType<CommonEngram>()) < 5)
				{
					continue;
				}

				return true;
			}
			return false;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = Language.GetTextValue("Mods.DestinyMod.Common.Decrypt");
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton)
			{
				shop = true;
			}
			else
			{
				Main.playerInventory = true;
				Main.npcChatText = string.Empty;
				ModContent.GetInstance<CryptarchUI>().UserInterface.SetState(new CryptarchUI());
			}
		}

		public override string GetChat()
		{
			if (BirthdayParty.PartyIsUp && Main.rand.NextBool(9))
			{
				return Language.GetTextValue("Mods.DestinyMod.Cryptarch.Party");
			}

			if (Main.eclipse && Main.rand.NextBool(9))
			{
				return Language.GetTextValue("Mods.DestinyMod.Cryptarch.Eclipse");
			}

			return Language.GetTextValue("Mods.DestinyMod.Cryptarch.Chatter_" + Main.rand.Next(1, 7));
		}

		public override void SetupShop(Chest shop, ref int nextSlot)
		{
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<CommonEngram>());
			shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 5);
			nextSlot++;

			if (NPC.downedBoss2)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<UncommonEngram>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 20);
				nextSlot++;
			}

			if (NPC.downedMechBossAny)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<RareEngram>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1);
				nextSlot++;
			}
		}

		public override void DrawTownAttackGun(ref float scale, ref int item, ref int closeness)
		{
			scale = 0.5f;
			item = ModContent.ItemType<Khvostov7G0X>();
			closeness = 20;
		}
	}
}