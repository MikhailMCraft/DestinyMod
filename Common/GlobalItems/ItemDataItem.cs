﻿using DestinyMod.Common.GlobalProjectiles;
using DestinyMod.Common.Items;
using DestinyMod.Common.Items.Modifiers;
using DestinyMod.Common.ModPlayers;
using DestinyMod.Content.Items.Mods;
using DestinyMod.Content.Items.Perks.Weapon.Barrels;
using DestinyMod.Content.Items.Perks.Weapon.KillTrackers;
using DestinyMod.Content.Items.Perks.Weapon.Magazines;
using DestinyMod.Content.Items.Weapons.Ranged;
using DestinyMod.Content.Items.Weapons.Ranged.Hakke;
using DestinyMod.Content.UI.ItemDetails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestinyMod.Common.GlobalItems
{
    /// <summary>Controls and contains the light level/perk system on normal items.</summary>
    public class ItemDataItem : GlobalItem
    {
        public int LightLevel = -1;

        public int Range = -1;

        public int Stability = -1; // Spread

        public int Recoil = -1; // Recoil

        public int EnemiesKilled;

        public int PlayersKilled;

        /// <summary>
        /// The list of perks that are currently active on this weapon.
        /// </summary>
        public IList<ItemPerk> ActivePerks;

        /// <summary>
        /// The list of mods socketed on this item.
        /// </summary>
        public IList<ItemMod> ItemMods;

        public int ItemCatalyst = -1;

        public Item Shader;

        /// <summary>
        /// The list of perk columns this weapon has (barrels, traits, etc.).
        /// </summary>
        public IList<ItemPerkPool> PerkPool;

        public IEnumerable<ModifierBase> AllItemModifiers(Player player = null)
        {
            List<ModifierBase> modifiers = new List<ModifierBase>();

            if (ActivePerks != null)
            {
                modifiers.AddRange(ActivePerks);
            }

            if (ItemMods != null)
            {
                modifiers.AddRange(ItemMods);
            }

            if (player != null && ItemCatalyst >= 0)
            {
                ItemDataPlayer itemDataPlayer = player.GetModPlayer<ItemDataPlayer>();
                ItemCatalyst catalyst = itemDataPlayer.CatalystData[ItemCatalyst];

                if (catalyst.IsDiscovered)
                {
                    modifiers.Add(catalyst);
                }
            }
            return modifiers;
        }

        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }

        public override void SetDefaults(Item item)
        {
            if (ItemData.ItemDatasByID != null && ItemData.ItemDatasByID.TryGetValue(item.type, out ItemData itemData))
            {
                if (LightLevel < itemData.DefaultLightLevel)
                {
                    LightLevel = itemData.DefaultLightLevel;
                }

                Range = itemData.DefaultRange;
                Stability = itemData.DefaultStability;
                Recoil = itemData.DefaultRecoil;

                ItemCatalyst = itemData.ItemCatalyst;

                if (itemData.InterpretLightLevel == null)
                {
                    float defaultDamageMultiplier = (float)(Math.Pow(itemData.DefaultLightLevel - 1350, 2) / 500f) + 1f;
                    float currentDamageMultiplier = (float)(Math.Pow(LightLevel - 1350, 2) / 500f) + 1f;
                    item.damage = (int)(item.damage * (currentDamageMultiplier / defaultDamageMultiplier));
                }
                else
                {
                    itemData.InterpretLightLevel(LightLevel);
                }

                if (Main.LocalPlayer != null && Main.LocalPlayer.TryGetModPlayer(out ItemDataPlayer _))
                {
                    foreach (ModifierBase modifiers in AllItemModifiers(Main.LocalPlayer)) // Maybe not LocalPlayer?
                    {
                        modifiers?.SetItemDefaults(item);
                    }
                }
                else
                {
                    foreach (ModifierBase modifiers in AllItemModifiers())
                    {
                        modifiers?.SetItemDefaults(item);
                    }
                }

                if (PerkPool == null && itemData.GeneratePerkPool != null)
                {
                    PerkPool = itemData.GeneratePerkPool();
                    PerkPool.Add(new ItemPerkPool("Tracker", ModContent.GetInstance<EmptyTracker>(), ModContent.GetInstance<EnemyTracker>()));
                    ActivePerks = new List<ItemPerk>();
                    foreach (ItemPerkPool perkPoolType in PerkPool)
                    {
                        ActivePerks.Add(perkPoolType.Perks[0]);
                    }
                }

                if (ItemMods == null)
                {
                    ItemMods = new List<ItemMod>();
                    for (int modIndexer = 0; modIndexer < itemData.MaximumModCount; modIndexer++)
                    {
                        ItemMods.Add(ModContent.GetInstance<NullMod>());
                    }
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (LightLevel < ItemData.MinimumLightLevel)
            {
                return;
            }

            TooltipLine lightLevelTooltip = new TooltipLine(DestinyMod.Instance, "LightLevel", "Power: " + LightLevel.ToString()); // To Do: Fancy icons when different classes get implemented
            TooltipLine nameTooltip = tooltips.FirstOrDefault(tooltip => tooltip.Mod == "Terraria" && tooltip.Name == "ItemName");

            if (nameTooltip != null)
            {
                int nameIndex = tooltips.IndexOf(nameTooltip);
                tooltips.Insert(nameIndex + 1, lightLevelTooltip);
            }
            else
            {
                tooltips.Add(lightLevelTooltip);
            }

            TooltipLine killsTooltip = new TooltipLine(DestinyMod.Instance, "KillTracker", "Nothing");
            TooltipLine knockbackTooltip = tooltips.FirstOrDefault(tooltip => tooltip.Mod == "Terraria" && tooltip.Name == "Knockback");
            if (ActivePerks.Any(perk => perk.DisplayName == "Enemy Tracker"))
            {
                killsTooltip = new TooltipLine(DestinyMod.Instance, "KillTracker", "Enemies Defeated: " + EnemiesKilled);
            }
            else if (ActivePerks.Any(perk => perk.DisplayName == "Crucible Tracker"))
            {
                killsTooltip = new TooltipLine(DestinyMod.Instance, "KillTracker", "Opposing Players Defeated: " + EnemiesKilled);
            }

            if (knockbackTooltip != null && killsTooltip.Text != "Nothing")
            {
                int nameIndex = tooltips.IndexOf(knockbackTooltip);
                tooltips.Insert(nameIndex + 1, killsTooltip);
            }
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            ItemDataPlayer itemDataPlayer = player.GetModPlayer<ItemDataPlayer>();

            if (itemDataPlayer.Stability >= 0)
            {
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians((100 - itemDataPlayer.Stability) * 0.2f));
            }

            if (itemDataPlayer.Recoil >= 0)
            {
                itemDataPlayer.OldWeaponBounce = itemDataPlayer.WeaponUseBounce;
                float recoilInRadians = MathHelper.ToRadians(itemDataPlayer.WeaponUseBounce);
                if (player.direction < 0)
                {
                    recoilInRadians *= -1;
                }
                velocity = velocity.RotatedBy(recoilInRadians);

                player.itemRotation = velocity.ToRotation();

                if (player.direction < 0)
                {
                    player.itemRotation = MathHelper.WrapAngle(player.itemRotation + MathHelper.Pi);
                }

                float recoilAdjustment = Recoil * 0.05f + item.useTime;
                if (Recoil % 2 != 0)
                {
                    recoilAdjustment *= -1;
                }
                itemDataPlayer.WeaponUseBounce += recoilAdjustment;
                itemDataPlayer.ResetBounceTimer = 0;
                itemDataPlayer.ResetBounceThreshold = item.useAnimation * 2;
                Main.NewText($"Range: {itemDataPlayer.Range} | Stability: {itemDataPlayer.Stability} | Recoil: {itemDataPlayer.Recoil}");
            }
        }

        /*public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Recoil >= 0)
            {
                ItemDataPlayer itemDataPlayer = player.GetModPlayer<ItemDataPlayer>();

                itemDataPlayer.OldWeaponBounce = itemDataPlayer.WeaponUseBounce;
                float recoilInRadians = MathHelper.ToRadians(itemDataPlayer.WeaponUseBounce);
                if (player.direction < 0)
                {
                    recoilInRadians *= -1;
                }
                velocity = velocity.RotatedBy(recoilInRadians);

                player.itemRotation = velocity.ToRotation();

                if (player.direction < 0)
                {
                    player.itemRotation = MathHelper.WrapAngle(player.itemRotation + MathHelper.Pi);
                }

                float recoilAdjustment = Recoil * 0.05f + item.useTime;
                if (Recoil % 2 != 0)
                {
                    recoilAdjustment *= -1;
                }
                itemDataPlayer.WeaponUseBounce += recoilAdjustment;
                itemDataPlayer.ResetBounceTimer = 0;
                itemDataPlayer.ResetBounceThreshold = item.useAnimation * 2;
                //Main.NewText("Current Recoil: " + itemDataPlayer.WeaponUseBounce + " Old Recoil: " + itemDataPlayer.OldWeaponBounce + " Delta: " + recoilAdjustment);
            }

            return true;

            int recVal = ItemData.CalculateRecoil(Recoil);
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(recVal / 10));
            if (newVel.Y > Recoil)
            {
                newVel.Y = 0; // Why? - Plan is to have item spread according to the pinned recoil summary in developer chat
            }
            Projectile.NewProjectile(source, position, newVel, type, damage, knockback, player.whoAmI);
            return false;
        }*/

        #region Drawing

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Shader != null && Shader.dye > 0)
            {
                // Thank you old me for doing EnergySword :dorime:
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix); 
                GameShaders.Armor.GetShaderFromItemId(Shader.type).Apply(item);
            }

            return true;
        }

        // death
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Shader != null && Shader.dye > 0)
            {
                // Thank you old me for doing EnergySword :dorime:
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
            }
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (Shader != null && Shader.dye > 0)
            {
                // Thank you old me for doing EnergySword :dorime:
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                GameShaders.Armor.GetShaderFromItemId(Shader.type).Apply(item);
            }

            return true;
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (Shader != null && Shader.dye > 0)
            {
                // Thank you old me for doing EnergySword :dorime:
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        #endregion

        #region I/O

        public override void SaveData(Item item, TagCompound tag)
        {
            tag.Add("LightLevel", LightLevel);
            tag.Add("EnemiesKilled", EnemiesKilled);
            tag.Add("PlayersKilled", PlayersKilled);

            if (ActivePerks != null)
            {
                tag.Add("ItemPerks", ActivePerks.Select(itemPerk => itemPerk == null ? "Null" : itemPerk.Name).ToList());
            }

            if (ItemMods != null && ItemMods.Count > 0)
            {
                tag.Add("ItemMods", ItemMods.Select(mod => mod == null ? ModContent.GetInstance<NullMod>().Name : mod.Name).ToList());
            }

            if (PerkPool != null && PerkPool.Count > 0)
            {
                tag.Add("PerkPool", PerkPool.Select(perkPool => perkPool.Perks.Select(perk => perk.Name).ToList()).ToList());
                tag.Add("PerkPoolNames", PerkPool.Select(perkPool => perkPool.TypeName).ToList());
            }

            if (Shader != null)
            {
                tag.Add("Shader", ItemIO.Save(Shader));
            }
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            LightLevel = tag.Get<int>("LightLevel");
            EnemiesKilled = tag.Get<int>("EnemiesKilled");
            PlayersKilled = tag.Get<int>("PlayersKilled");

            if (tag.ContainsKey("ItemPerks"))
            {
                ActivePerks = new List<ItemPerk>();
                List<string> savedPerks = tag.Get<List<string>>("ItemPerks");
                foreach (string perk in savedPerks)
                {
                    if (perk == "Null")
                    {
                        ActivePerks.Add(null);
                        continue;
                    }

                    if (ModAndPerkLoader.ItemPerksByName.TryGetValue(perk, out ItemPerk itemPerk))
                    {
                        if (itemPerk.IsInstanced)
                        {
                            ActivePerks.Add(ItemPerk.CreateInstance(itemPerk.Name));
                        }
                        else
                        {
                            ActivePerks.Add(itemPerk);
                        }
                    }
                    else
                    {
                        ActivePerks.Add(null);
                    }
                }
            }

            if (tag.ContainsKey("ItemMods"))
            {
                ItemMods = new List<ItemMod>();
                List<string> itemModsSaved = tag.Get<List<string>>("ItemMods");
                foreach (string modName in itemModsSaved)
                {
                    if (modName == "Null")
                    {
                        ItemMods.Add(ModContent.GetInstance<NullMod>());
                        continue;
                    }

                    if (ModAndPerkLoader.ItemModsByName.TryGetValue(modName, out ItemMod itemMod))
                    {
                        ItemMods.Add(itemMod);
                    }
                    else
                    {
                        ItemMods.Add(ModContent.GetInstance<NullMod>());
                    }
                }
            }

            if (tag.ContainsKey("PerkPool") && tag.ContainsKey("PerkPoolNames"))
            {
                PerkPool = new List<ItemPerkPool>();
                List<List<string>> perkPoolsSaved = tag.Get<List<List<string>>>("PerkPool");
                List<string> perkPoolNamesSaved = tag.Get<List<string>>("PerkPoolNames");
                int index = 0;
                foreach (List<string> perkPool in perkPoolsSaved)
                {
                    List<ItemPerk> perks = new List<ItemPerk>();
                    foreach (string perk in perkPool)
                    {
                        if (!ModAndPerkLoader.ItemPerksByName.TryGetValue(perk, out ItemPerk itemPerk))
                        {
                            continue;
                        }

                        perks.Add(itemPerk);
                    }

                    PerkPool.Add(new ItemPerkPool(perkPoolNamesSaved[index], perks.ToArray()));
                    index++;
                }
            }

            if (tag.ContainsKey("Shader"))
            {
                Shader = ItemIO.Load(tag.Get<TagCompound>("Shader"));
            }

            SetDefaults(item);
        }

        #endregion
    }

    public class TestLightLevelCommand : ModCommand
    {
        public override string Command => "TLL";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
            {
                Main.NewText("Arg length too short");
                return;
            }

            if (!int.TryParse(args[0], out int lightLevel))
            {
                Main.NewText("Arg 1 not int");
                return;
            }

            if (ItemData.ItemDatasByID.TryGetValue(ModContent.ItemType<HakkeAutoRifle>(), out ItemData hakkeAutoRifleData))
            {
                hakkeAutoRifleData.GenerateItem(Main.item[caller.Player.QuickSpawnItem(new EntitySource_WorldEvent(), hakkeAutoRifleData.ItemType)], caller.Player, lightLevel);
                hakkeAutoRifleData.GenerateItem(Main.item[caller.Player.QuickSpawnItem(new EntitySource_WorldEvent(), hakkeAutoRifleData.ItemType)], caller.Player, lightLevel);

                /*caller.Player.QuickSpawnItem(new EntitySource_WorldEvent(),
                    hakkeAutoRifleData.GenerateItem(caller.Player, lightLevel, new List<ItemPerkPool>()
                    {
                        new ItemPerkPool("Barrels", ModContent.GetInstance<ArrowheadBrake>(), ModContent.GetInstance<BarrelShroud>(), ModContent.GetInstance<ChamberedCompensator>()),
                        new ItemPerkPool("Traits", ModContent.GetInstance<Frenzy>(), ModContent.GetInstance<HighCaliberRounds>())
                    })
                );

                caller.Player.QuickSpawnItem(new EntitySource_WorldEvent(),
                    hakkeAutoRifleData.GenerateItem(caller.Player, lightLevel, new List<ItemPerkPool>()
                    {
                        new ItemPerkPool("Barrels", ModContent.GetInstance<ArrowheadBrake>()),
                        new ItemPerkPool("Traits", ModContent.GetInstance<Frenzy>(), ModContent.GetInstance<HighCaliberRounds>())
                    })
                );*/
            }

            if (ItemData.ItemDatasByID.TryGetValue(ModContent.ItemType<NoTimeToExplain>(), out ItemData mtteData))
            {
                caller.Player.QuickSpawnItem(new EntitySource_WorldEvent(), 
                    mtteData.GenerateItem(caller.Player, lightLevel, new List<ItemPerkPool>()
                    {
                        new ItemPerkPool("Barrels", ModifierBase.CreateInstanceOf<ArrowheadBrake>()),
                        new ItemPerkPool("Traits", ModifierBase.CreateInstanceOf<HighCaliberRounds>())
                    })
                );
            }
        }
    }
}
