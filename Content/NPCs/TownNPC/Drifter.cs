using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;
using DestinyMod.Content.Items.Materials;
using DestinyMod.Common.NPCs.NPCTypes;
using DestinyMod.Content.Items.Weapons.Ranged;
using DestinyMod.Common.ModPlayers;
using DestinyMod.Content.Items.Equipables.Pets;
using DestinyMod.Content.Items.Consumables.Potions;
using DestinyMod.Content.Items.Equipables.Dyes;

namespace DestinyMod.Content.NPCs.Town
{
	[AutoloadHead]
	public class Drifter : GenericTownNPC
	{
		public override void DestinySetStaticDefaults() => DisplayName.SetDefault("Dredgen");

		public override void AutomaticSetDefaults()
		{
			NPC.width = 26;
			NPC.height = 46;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			for (int playerCount = 0; playerCount < Main.maxPlayers; playerCount++)
			{
				Player player = Main.player[playerCount];
				if (!player.active || player.CountItem(ModContent.ItemType<MoteOfDark>()) < 5)
				{
					continue;
				}

				return true;
			}
			return false;
		}

		public override string TownNPCName() => "The Drifter";

		public override string GetChat()
		{
			int taxCollector = NPC.FindFirstNPC(NPCID.TaxCollector);
			bool xur = NPC.AnyNPCs(ModContent.NPCType<AgentOfNine>());
			string gender = Main.LocalPlayer.Male ? Language.GetTextValue("Mods.TheDestinyMod.Common.Brother") : Language.GetTextValue("Mods.TheDestinyMod.Common.Sister");
			List<string> dialogue = new List<string>();
			if (taxCollector >= 0)
			{
				string taxCollectorName = Main.npc[taxCollector].GivenName;
				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Chatter_" + (NPC.AnyNPCs(ModContent.NPCType<Zavala>()) ? 14 : 15), taxCollectorName, gender));
			}

			if (xur)
			{
				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Chatter_13", gender));
			}

			if (BirthdayParty.PartyIsUp && Main.rand.NextBool(6))
			{
				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Party", gender));
			}

			if (Main.eclipse)
			{
				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Eclipse"));
			}

			if (Main.hardMode)
			{
				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Chatter_10"));
				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Chatter_11"));
				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Chatter_12"));
			}

			if (Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
			{
				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Invasion"));
			}

			for (int dialogueCount = 1; dialogueCount < 9; dialogueCount++)
			{
				if (dialogueCount == 2)
				{
					dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Chatter_2", gender));
					continue;
				}

				dialogue.Add(Language.GetTextValue("Mods.TheDestinyMod.Drifter.Chatter_" + dialogueCount));
			}

			return Main.rand.Next(dialogue);
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("Mods.TheDestinyMod.Common.GiveMotes");
			button2 = Language.GetTextValue("Mods.TheDestinyMod.Common.CheckMotes");
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			Player player = Main.LocalPlayer;
			NPCPlayer npcPlayer = player.GetModPlayer<NPCPlayer>();
			string gender = player.Male ? Language.GetTextValue("Mods.TheDestinyMod.Common.Brother") : Language.GetTextValue("Mods.TheDestinyMod.Common.Sister");
			if (firstButton)
			{
				if (player.HasItem(ModContent.ItemType<MoteOfDark>()))
				{
					int playerMoteCount = player.CountItem(ModContent.ItemType<MoteOfDark>());
					if (playerMoteCount < 25)
					{
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.Motes1", gender);
					}
					else if (playerMoteCount > 25 && playerMoteCount < 50)
					{
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.Motes2");
					}
					else
					{
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.Motes" + Main.rand.Next(3, 6), gender);
					}

					int oldMoteCount = npcPlayer.MotesGiven;
					npcPlayer.MotesGiven += playerMoteCount;
					foreach (Item item in player.inventory)
					{
						if (item.type == ModContent.ItemType<MoteOfDark>())
						{
							item.TurnToAir();
						}
					}

					SoundEngine.PlaySound(SoundID.Grab);
					int checkImpact = (npcPlayer.MotesGiven - oldMoteCount) / 10;
					int checkMoney = (npcPlayer.MotesGiven - oldMoteCount) / 25;
					if (checkImpact >= 1 && npcPlayer.MotesGiven > 20)
					{
						player.QuickSpawnItem(ModContent.ItemType<ImpactShard>(), checkImpact);
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesRepeatable" + Main.rand.Next(1, 3), gender);
					}

					if (checkMoney >= 1 && npcPlayer.MotesGiven > 20)
					{
						player.QuickSpawnItem(ItemID.GoldCoin, checkImpact * 20);
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MoneyRepeatable" + Main.rand.Next(1, 3), gender);
					}

					if (npcPlayer.MotesGiven > 10 && oldMoteCount < 10)
					{
						player.QuickSpawnItem(ItemID.GoldCoin, 5);
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesReward1", gender);
					}

					if (npcPlayer.MotesGiven > 20 && oldMoteCount < 20)
					{
						player.QuickSpawnItem(ModContent.ItemType<ImpactShard>(), 5);
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesReward2", gender);
					}

					if (npcPlayer.MotesGiven > 30 && oldMoteCount < 30)
					{
						player.QuickSpawnItem(ModContent.ItemType<BottomDollar>());
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesReward3");
					}

					if (npcPlayer.MotesGiven > 40 && oldMoteCount < 40)
					{
						player.QuickSpawnItem(ModContent.ItemType<GambitDye>(), 3);
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesReward4", gender);
					}

					if (npcPlayer.MotesGiven > 50 && oldMoteCount < 50)
					{
						player.QuickSpawnItem(ModContent.ItemType<GunsmithMaterials>(), 100);
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesReward5", gender);
					}

					if (npcPlayer.MotesGiven > 60 && oldMoteCount < 60)
					{
						player.QuickSpawnItem(ModContent.ItemType<Ghost>());
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesReward6", gender);
					}

					if (npcPlayer.MotesGiven > 70 && oldMoteCount < 70)
					{
						player.QuickSpawnItem(ModContent.ItemType<TrinarySystem>());
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesReward7", gender);
					}

					if (npcPlayer.MotesGiven > 100 && oldMoteCount < 100)
					{
						player.QuickSpawnItem(ModContent.ItemType<BorrowedTime>());
						Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.MotesReward8", gender);
					}
					return;
				}
				else
				{
					switch (Main.rand.Next(3))
					{
						case 1:
							Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.NoMotes1", char.ToUpper(gender[0]) + gender.Substring(1));
							break; ;

						case 2:
							Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.NoMotes2");
							break;

						default:
							Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.NoMotes3", gender);
							break;
					}
				}

				return;
			}

			if (npcPlayer.MotesGiven <= 0)
			{
				Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.CheckMotes1", gender);
			}
			else if (npcPlayer.MotesGiven < 100)
			{
				Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.CheckMotes4", npcPlayer.MotesGiven, gender);
			}
			else if (npcPlayer.MotesGiven < 200)
			{
				Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.CheckMotes3", npcPlayer.MotesGiven, gender);
			}
			else
			{
				Main.npcChatText = Language.GetTextValue("Mods.TheDestinyMod.Drifter.CheckMotes2", npcPlayer.MotesGiven, gender);
			}
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 20;
			knockback = 4f;

			if (Main.hardMode)
			{
				damage = 40;
			}

			if (NPC.downedMechBossAny)
			{
				damage = 60;
			}
		}

		public override void DrawTownAttackGun(ref float scale, ref int item, ref int closeness)
		{
			scale = 0.5f;
			item = ModContent.ItemType<BottomDollar>();
			closeness = 20;
		}
	}
}