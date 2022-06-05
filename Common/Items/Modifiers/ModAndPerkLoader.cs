using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace DestinyMod.Common.Items.Modifiers
{
    public class ModAndPerkLoader : ILoadable
    {
        public static int PerkTypeReserver { get; private set; }

        public static int ModTypeReserver { get; private set; }

        public static int CatalystTypeReserver { get; private set; }

        public static IList<ItemPerk> ItemPerks { get; private set; }

        public static IList<ItemMod> ItemMods { get; private set; }

        public static IList<ItemCatalyst> ItemCatalysts { get; private set; }

        public static IDictionary<string, ItemPerk> ItemPerksByName { get; private set; }

        public static IDictionary<string, ItemMod> ItemModsByName { get; private set; }

        public static IDictionary<string, ItemCatalyst> ItemCatalystsByName { get; private set; }

        public void Load(Mod mod)
        {
            ModTypeReserver = 0;
            ItemMods = new List<ItemMod>();
            ItemModsByName = new Dictionary<string, ItemMod>();

            PerkTypeReserver = 0;
            ItemPerks = new List<ItemPerk>();
            ItemPerksByName = new Dictionary<string, ItemPerk>();

            CatalystTypeReserver = 0;
            ItemCatalysts = new List<ItemCatalyst>();
            ItemCatalystsByName = new Dictionary<string, ItemCatalyst>();

            foreach (Type type in mod.Code.GetTypes())
            {
                if (type.IsAbstract || type.GetConstructor(Array.Empty<Type>()) == null)
                {
                    continue;
                }

                if (type.IsSubclassOf(typeof(ItemMod)))
                {
                    ItemMod itemMod = Activator.CreateInstance(type) as ItemMod;
                    string internalName = type.Name;
                    itemMod.Load(ref internalName);
                    itemMod.Type = ModTypeReserver++;
                    itemMod.Name = internalName;
                    itemMod.SetDefaults();
                    ItemMods.Add(itemMod);
                    ItemModsByName.Add(itemMod.Name, itemMod);
                    ContentInstance.Register(itemMod);
                }

                if (type.IsSubclassOf(typeof(ItemPerk)))
                {
                    ItemPerk itemPerk = Activator.CreateInstance(type) as ItemPerk;
                    string internalName = type.Name;
                    itemPerk.Load(ref internalName);
                    itemPerk.Type = PerkTypeReserver++;
                    itemPerk.Name = internalName;
                    itemPerk.SetDefaults();
                    ItemPerks.Add(itemPerk);
                    ItemPerksByName.Add(itemPerk.Name, itemPerk);
                    ContentInstance.Register(itemPerk);
                }

                if (type.IsSubclassOf(typeof(ItemCatalyst)))
                {
                    ItemCatalyst itemCatalyst = Activator.CreateInstance(type) as ItemCatalyst;
                    string internalName = type.Name;
                    itemCatalyst.Load(ref internalName);
                    itemCatalyst.Type = CatalystTypeReserver++;
                    itemCatalyst.Name = internalName;
                    itemCatalyst.SetDefaults();
                    ItemCatalysts.Add(itemCatalyst);
                    ItemCatalystsByName.Add(itemCatalyst.Name, itemCatalyst);
                    ContentInstance.Register(itemCatalyst);
                }
            }
        }

        public void Unload()
        {
            ItemMods?.Clear();
            ItemModsByName?.Clear();
            ItemPerks?.Clear();
            ItemPerksByName?.Clear();
            ItemCatalysts?.Clear();
            ItemCatalystsByName?.Clear();
        }
    }
}
