using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent.Events;
using Terraria.DataStructures;
using DestinyMod.Common.NPCs.NPCTypes;
using DestinyMod.Content.Items.Weapons.Ranged;
using DestinyMod.Content.Items.Equipables.Dyes;
using DestinyMod.Content.Items.Placeables;
using DestinyMod.Common.ModSystems;
using DestinyMod.Common.ModPlayers;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;

namespace DestinyMod.Content.NPCs.TownNPC
{
	[AutoloadHead]
	public class Zavala : GenericTownNPC
	{
		private bool hasClosedCeremony = false;

		public override void DestinySetStaticDefaults()
		{
			DisplayName.SetDefault("Titan Vanguard");
			Main.npcFrameCount[NPC.type] = 25;
			NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
			NPCID.Sets.AttackFrameCount[NPC.type] = 4;

			NPC.Happiness
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Hate)
				.SetNPCAffection(ModContent.NPCType<Drifter>(), AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
			;
		}

		public override List<string> SetNPCNameList() => new List<string> { "Zavala" };

		public override void DestinySetDefaults()
		{
			NPC.width = 18;
			NPC.height = 40;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				new FlavorTextBestiaryInfoElement("Mods.DestinyMod.Bestiary.Zavala")
			});
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money) => NPC.downedSlimeKing;

		public override string GetChat()
		{

			if (Main.rand.NextBool(100))
			{
				return "Whether we wanted it or not, we've stepped into a war with the Cabal on Mars. So let's get to taking out their command, one by one. Valus Ta'aurc. From what I can gather he commands the Siege Dancers from an Imperial Land Tank outside of Rubicon. He's well protected, but with the right team, we can punch through those defenses, take this beast out, and break their grip on Freehold.";
			}

			if (NPC.AnyDanger())
			{
				return Language.GetTextValue("Mods.DestinyMod.Zavala.Boss" + Main.rand.Next(1, 4));
			}

			List<string> dialogue = new List<string>();
			if (NPC.downedMoonlord)
			{
				dialogue.Add(Language.GetTextValue("Mods.DestinyMod.Zavala.AfterML"));
			}

			if (NPC.FindFirstNPC(ModContent.NPCType<Drifter>()) >= 0)
			{
				dialogue.Add(Language.GetTextValue("Mods.DestinyMod.Zavala.Chatter_1"));
			}

			if (BirthdayParty.PartyIsUp)
			{
				dialogue.Add(Language.GetTextValue("Mods.DestinyMod.Zavala.Party"));
			}

			if (Main.LocalPlayer.ZoneCorrupt || Main.LocalPlayer.ZoneCrimson)
			{
				dialogue.Add(Language.GetTextValue("Mods.DestinyMod.Zavala.Evil"));
			}

			if (Main.LocalPlayer.ZoneHallow)
			{
				dialogue.Add(Language.GetTextValue("Mods.DestinyMod.Zavala.Hallow"));
			}

			for (int dialogueCount = 2; dialogueCount < 6; dialogueCount++)
			{
				dialogue.Add(Language.GetTextValue("Mods.DestinyMod.Zavala.Chatter_" + dialogueCount));
			}
			return Main.rand.Next(dialogue);
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = Language.GetTextValue("Mods.DestinyMod.Common.Bounty");
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton)
			{
				shop = true;
			}
			else
			{
				NPCPlayer player = Main.LocalPlayer.GetModPlayer<NPCPlayer>();
				EntitySource_Gift source = new EntitySource_Gift(NPC);
				switch (player.ZavalaBountyProgress)
				{
					case 0:
						Main.npcChatText = Language.GetTextValue("Mods.DestinyMod.ZavalaBounty1");
						player.ZavalaBountyProgress = 1;
						break;

					case 1:
						if (player.ZavalaEnemies == 100)
						{
							Main.npcChatText = Language.GetTextValue("Mods.DestinyMod.Zavala.BountyComplete1");
							Main.LocalPlayer.QuickSpawnItem(source, ModContent.ItemType<TheThirdAxiom>());
							player.ZavalaBountyProgress = 2;
							player.ZavalaEnemies = 0;
						}
						else
						{
							Main.npcChatText = Language.GetTextValue("Mods.DestinyMod.Zavala.BountyProgress1", player.ZavalaEnemies);
						}
						break;

					case 2:
						Main.npcChatText = Language.GetTextValue("Mods.DestinyMod.Zavala.BountyRequisition2");
						player.ZavalaBountyProgress = 3;
						break;

					case 3:
						if (player.ZavalaEnemies == 50)
						{
							Main.npcChatText = Language.GetTextValue("Mods.DestinyMod.Zavala.BountyComplete2");
							Main.LocalPlayer.QuickSpawnItem(source, ModContent.ItemType<LastWord>());
							player.ZavalaBountyProgress = 4;
							player.ZavalaEnemies = 0;
						}
						else
						{
							Main.npcChatText = Language.GetTextValue("Mods.DestinyMod.Zavala.BountyProgress2", player.ZavalaBountyProgress);
						}
						break;

					case 4:
					default:
						Main.npcChatText = "I've got nothing for you right now, Guardian.";
						break;
				}
			}
		}

		public override void SetupShop(Chest shop, ref int nextSlot)
		{
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<SalvagersSalvo>());
			shop.item[nextSlot].shopCustomPrice = 500000;
			nextSlot++;
		}

		public override void DrawTownAttackGun(ref float scale, ref int item, ref int closeness)
		{
			scale = 0.7f;
			item = ModContent.ItemType<TheThirdAxiom>();
			closeness = 20;
		}
	}
}