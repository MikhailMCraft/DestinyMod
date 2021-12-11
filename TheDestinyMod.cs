using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using Terraria.World.Generation;
using ReLogic.Graphics;
using Terraria.GameContent.Generation;
using TheDestinyMod.NPCs.SepiksPrime;
using TheDestinyMod.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Linq;
using log4net;
using System.Reflection;

namespace TheDestinyMod
{
	public class TheDestinyMod : Mod
	{
        public static ModHotKey activateSuper;
        public static int CipherCustomCurrencyId;
        public static bool guardianGames = false;
        public static bool guardianGameError = false;
        public static int guardianWinner = 0;

        public static DynamicSpriteFont fontFuturaBold;
        public static DynamicSpriteFont fontFuturaBook;

        public static string currentSubworldID = string.Empty;

        internal UserInterface CryptarchUserInterface;
        internal SubclassUI SubclassUI;
        internal RaidSelectionUI RaidSelectionUI;
        internal ClassSelectionUI ClassSelectionUI;
        internal SuperChargeBar SuperResourceCharge;
        internal UserInterface raidInterface;

        private UserInterface superChargeInterface;
        private UserInterface subclassInterface;
        internal UserInterface classSelectionInterface;

        public static TheDestinyMod Instance { get; private set; }

        public static bool classSelecting;
        private bool wasJustCreating;

        public TheDestinyMod() {
            Instance = this;
        }

        public override void Load() {
            activateSuper = RegisterHotKey("Activate Super", "U");
            CipherCustomCurrencyId = CustomCurrencyManager.RegisterCurrency(new ExoticCipher(ModContent.ItemType<Items.ExoticCipher>(), 30L));
            On.Terraria.Player.DropTombstone += Player_DropTombstone;
            On.Terraria.UI.ItemSlot.ArmorSwap += ItemSlot_ArmorSwap;
            On.Terraria.UI.ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick_ItemArray_int_int;
            On.Terraria.UI.ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_ItemArray_int_int;
            On.Terraria.GameContent.UI.Elements.UICharacterListItem.DrawSelf += UICharacterListItem_DrawSelf;
            On.Terraria.Main.DrawMouseOver += Main_DrawMouseOver;
            if (!Main.dedServ) {
                if (DestinyClientConfig.Instance.GuardianGamesConfig) {
                    try {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://DestinyModServer.mikhailmcraft.repl.co");
                        request.Method = "GET";
                        request.Headers["VERIFY-MOD"] = "a7rg53F435h4Ff2fhjWa33gH6j54ag2G";
                        request.Headers["DUPLICATE-CHECK"] = Steamworks.SteamUser.GetSteamID().ToString();
                        request.Timeout = 1500;

#pragma warning disable IDE0063
                        using (Stream s = request.GetResponse().GetResponseStream()) {
                            using (StreamReader sr = new StreamReader(s)) {
                                var jsonResponse = sr.ReadToEnd();
                                if (jsonResponse.Remove(2) == "ON") {
                                    guardianGames = true;
                                }
                                if (jsonResponse.Contains("T")) {
                                    guardianWinner = 1;
                                }
                                else if (jsonResponse.Contains("H")) {
                                    guardianWinner = 2;
                                }
                                else if (jsonResponse.Contains("W")) {
                                    guardianWinner = 3;
                                }
                            }
                        }
#pragma warning restore IDE0063
                    }
                    catch (Exception e) {
                        Logger.Error($"Failed to receive a response from the server: {e.Message}");
                        guardianGameError = true;
                    }
                }
                else {
                    guardianGameError = true;
                }

                Main.OnTick += Main_OnTick;

                fontFuturaBold = Main.fontMouseText;
                fontFuturaBook = Main.fontMouseText;

                if (FontExists("Fonts/FuturaBold"))
                    fontFuturaBold = GetFont("Fonts/FuturaBold");

                if (FontExists("Fonts/FuturaBook"))
                    fontFuturaBook = GetFont("Fonts/FuturaBook");

                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/SepiksPrime"), ModContent.ItemType<Items.Placeables.MusicBoxes.SepiksPrimeBox>(), ModContent.TileType<Tiles.MusicBoxes.SepiksPrimeBox>());

                GameShaders.Armor.BindShader(ModContent.ItemType<Items.Dyes.GambitDye>(), new ArmorShaderData(new Ref<Effect>(GetEffect("Effects/Dyes/Gambit")), "GambitDyePass")).UseColor(0, 1f, 0);
                GameShaders.Armor.BindShader(ModContent.ItemType<Items.Dyes.GuardianGamesDye>(), new ArmorShaderData(new Ref<Effect>(GetEffect("Effects/Dyes/GuardianGames")), "GuardianGamesDyePass")).UseColor(2f, 2f, 0f).UseSecondaryColor(2f, 0.25f, 0.35f);
                Ref<Effect> screenRef = new Ref<Effect>(GetEffect("Effects/Shaders/ShockwaveEffect"));
                Filters.Scene["TheDestinyMod:Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["TheDestinyMod:Shockwave"].Load();
                SubclassUI = new SubclassUI();
                SubclassUI.Activate();
                RaidSelectionUI = new RaidSelectionUI("Vault of Glass", DestinyWorld.clearsVOG, NPC.downedBoss3, "Skeletron");
                RaidSelectionUI.Activate();
                ClassSelectionUI = new ClassSelectionUI();
                ClassSelectionUI.Activate();
                subclassInterface = new UserInterface();
                subclassInterface.SetState(SubclassUI);
                CryptarchUserInterface = new UserInterface();
                SuperResourceCharge = new SuperChargeBar();
				superChargeInterface = new UserInterface();
				superChargeInterface.SetState(SuperResourceCharge);
                raidInterface = new UserInterface();
                classSelectionInterface = new UserInterface();
            }
            #region Translations
            int taxCollector = NPC.FindFirstNPC(NPCID.TaxCollector);
            ModTranslation text = CreateTranslation("AgentOfNine1");
            text.SetDefault("I may be here when you return.");
            text.AddTranslation(GameCulture.Spanish, "Talvez esté aqui cuando vuelvas.");
            text.AddTranslation(GameCulture.Polish, "Będę tutaj jak powrócisz.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine2");
            text.SetDefault("These are from the Nine.");
            text.AddTranslation(GameCulture.Spanish, "Esto es de los Nueve.");
            text.AddTranslation(GameCulture.Polish, "Oni są z tej dziewiątki.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine3");
            text.SetDefault("My will is not my own. Is yours?");
            text.AddTranslation(GameCulture.Spanish, "Mi voluntad no es mia. ¿Y la tuya?");
            text.AddTranslation(GameCulture.Polish, "Moja wola nie jest moją własną. A twoja?");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine4");
            text.SetDefault("The Traveler's song echoes on.");
            text.AddTranslation(GameCulture.Spanish, "La canción del Viajero resuena.");
            text.AddTranslation(GameCulture.Polish, "Pieśń Wędrowcy rozbrzmiewa echem.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine5");
            text.SetDefault("I bring a message from the Nine.");
            text.AddTranslation(GameCulture.Spanish, "Traigo un mensaje de los Nueve.");
            text.AddTranslation(GameCulture.Polish, "Przybywam z wiadomością od dziewiątki.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine6");
            text.SetDefault("Do not be alarmed. I know no reason to cause you harm.");
            text.AddTranslation(GameCulture.Spanish, "No te asustes. No conozco ninguna razón para causarte daño.");
            text.AddTranslation(GameCulture.Polish, "Nie przejmuj się. Nie mam powodów żeby cię skrzywdzić.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine7");
            text.SetDefault("To do what you say, is to speak in a language of pure meaning.");
            text.AddTranslation(GameCulture.Spanish, "Hacer lo que dices, es hablar un idioma de puro significado.");
            text.AddTranslation(GameCulture.Polish, "Robienie tego co mówisz to mówienie w języku o czystym znaczeniu.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine8");
            text.SetDefault("Do not go looking for the Nine. They will come to you.");
            text.AddTranslation(GameCulture.Spanish, "No busques a los Nueve. Ellos vendran a ti.");
            text.AddTranslation(GameCulture.Polish, "Nie szukaj dziewiątki. To ona znajdzie ciebie.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine9");
            text.SetDefault("You must stop eating salted popcorn.");
            text.AddTranslation(GameCulture.Spanish, "Deberias dejar de comer palomitas de maíz saladas.");
            text.AddTranslation(GameCulture.Polish, "Musisz przestać jeść ten słony popcorn.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine10");
            text.SetDefault("I am here for a reason, I just...cannot remember it.");
            text.AddTranslation(GameCulture.Spanish, "Estoy aqui por una razón, pero yo... no puedo recordarla.");
            text.AddTranslation(GameCulture.Polish, "Jestem tutaj z powodu tylko nie pamiętam jakiego.");
            AddTranslation(text);
            text = CreateTranslation("AgentOfNine11");
            text.SetDefault("So much Light here, I suppose I feel...pain.");
            text.AddTranslation(GameCulture.Spanish, "Tanta Luz aquí, supongo que siento... dolor.");
            AddTranslation(text);
            text = CreateTranslation("Cryptarch1");
            text.SetDefault("A party, you say? I'm much too busy decrypting these Vex artifacts, thank you.");
            text.AddTranslation(GameCulture.Polish, "Impreza powiadasz? Jestem bardziej zajęty deszyfrowaniem tych artefaktów od Vexów, dziękuje.");
            text.AddTranslation(GameCulture.Spanish, "¿Una escuadra, dices? Estoy muy ocupado desencriptando estos artefactos Vex, gracias.");
            AddTranslation(text);
            text = CreateTranslation("Cryptarch2");
            text.SetDefault("Ah! This lack of light makes it hard to do anything around here.");
            text.AddTranslation(GameCulture.Polish, "Przez brak światła sprawia, że trudno tu co kolwiek zrobić.");
            text.AddTranslation(GameCulture.Spanish, "¡Ah! la falta de luz hace mas dificil hacer cualquier cosa por aquí.");
            AddTranslation(text);
            text = CreateTranslation("Cryptarch3");
            text.SetDefault("Vex encryption. Unbreakable? Ha, so they say.");
            text.AddTranslation(GameCulture.Polish, "Szyfr Vexów nie do złamania? Ha tylko tak mówią.");
            text.AddTranslation(GameCulture.Spanish, "Encriptación vex. ¿Irrompible? Ja, eso dicen.");
            AddTranslation(text);
            text = CreateTranslation("Cryptarch4");
            text.SetDefault("What have you got for me, Guardian?");
            text.AddTranslation(GameCulture.Polish, "Co masz dla mnie, Strażniku?");
            text.AddTranslation(GameCulture.Spanish, "¿Que me trajiste, Guardián?");
            AddTranslation(text);
            text = CreateTranslation("Cryptarch5");
            text.SetDefault("Rasputin's fingerprints are all over this data. He doesn't even care if we know.");
            text.AddTranslation(GameCulture.Polish, "Odciski Rasputina są wszędzie. Go nie obchodzi że my wiemy.");
            text.AddTranslation(GameCulture.Spanish, "Las huellas de Rasputin estan sobre todos estos datos. Ni siquiera le importa si nosotros lo sabemos.");
            AddTranslation(text);
            text = CreateTranslation("Cryptarch6");
            text.SetDefault("What challenges have you brought today, Guardian?");
            text.AddTranslation(GameCulture.Polish, "Jakie wyzwanie przyniosłeś dzisiaj, Strażniku?");
            text.AddTranslation(GameCulture.Spanish, "¿Que desafio me trajiste hoy, Guardián?");
            AddTranslation(text);
            text = CreateTranslation("Cryptarch7");
            text.SetDefault("These are forgeries. Someone is wasting our time!");
            text.AddTranslation(GameCulture.Polish, "To są podróbki. Ktoś marnuje nasz czas!");
            text.AddTranslation(GameCulture.Spanish, "Estas son falsificaciones. ¡Alguien nos está haciendo perder el tiempo!");
            AddTranslation(text);
            text = CreateTranslation("Cryptarch8");
            text.SetDefault("Hmm, this one is labeled \"Bigm55\"...yes, Guardian?");
            text.AddTranslation(GameCulture.Spanish, "Hmm, esta está etiquetada como \"Bigm55\"... ¿Si, Guardián?");
            AddTranslation(text);
            text = CreateTranslation("RelicShard");
            text.SetDefault("Relic Shards have begun to grow!");
            text.AddTranslation(GameCulture.Polish, "Odłamki reliktów zaczęły rosnąć!");
            text.AddTranslation(GameCulture.Spanish, "¡Fragmentos de Reliquia empezaron a aparecer!");
            AddTranslation(text);
            text = CreateTranslation("Zavala1");
            text.SetDefault("Guardian! This is your chance to take down this profound threat, do not get distracted!");
            text.AddTranslation(GameCulture.Polish, "Strażniku! to jest twoja szansa na pokonanie tego gruntownego zagrożenia, nie rozpraszaj się!");
            text.AddTranslation(GameCulture.Spanish, "¡Guardián! ¡Esta es tu oportunidad para acabar con esta gran amenaza, no te distraigas!");
            AddTranslation(text);
            text = CreateTranslation("Zavala2");
            text.SetDefault("Get back out there, Guardian, and eliminate this threat!");
            text.AddTranslation(GameCulture.Polish, "Wracaj tam strażniku i pozbądź się tego zagrożenia!");
            text.AddTranslation(GameCulture.Spanish, "¡Sal afuera, Guardián, y elimina esta amenaza!");
            AddTranslation(text);
            text = CreateTranslation("Zavala3");
            text.SetDefault("Guardian, you can do this. I believe in you, as does the rest of the Vanguard.");
            text.AddTranslation(GameCulture.Polish, "Uda ci się strażniku, wierzę w ciebie tak samo jak cała reszta Straży przedniej.");
            text.AddTranslation(GameCulture.Spanish, "Guardián, puedes hacer esto. Creo en ti, como el resto de la Vanguardia.");
            AddTranslation(text);
            text = CreateTranslation("Zavala4");
            text.SetDefault("Guardian, you've slain some of the worst enemies the City and the Vanguard have ever seen, and for that, I thank you.");
            text.AddTranslation(GameCulture.Polish, "Strażniku, zabiłeś jednych z najgorszych wrogów jakich Miasto i Straż Przednia kiedykolwiek widziały i za to, Dziękuje tobie.");
            text.AddTranslation(GameCulture.Spanish, "Guardián, eliminaste a unos de los peores enemigos que la Ciudad y la Vanguardia hayan visto, y por eso, te agradezco.");
            AddTranslation(text);
            text = CreateTranslation("Zavala5");
            text.SetDefault("Guardian, it's been brought to my attention that you may be partaking in...unsolicited activities. As long as it's for the good of the City.");
            text.AddTranslation(GameCulture.Polish, "Strażniku, zwrócono mi uwagę że możesz brać udział w niedozwolonych zajęć. O ile to dla dobra miasta.");
            text.AddTranslation(GameCulture.Spanish, "Guardián, me llamó la atencion que tal vez estes completando... actividades no solicitadas. Mientras que sea por el bien de la Ciudad.");
            AddTranslation(text);
            text = CreateTranslation("Zavala6");
            text.SetDefault("I didn't authorize any party...but I guess we can take advantage of the moment while it lasts.");
            text.AddTranslation(GameCulture.Polish, "Nie autoryzowałem żadnej imprezy… ale myślę, że możemy wykorzystać ten moment, dopóki trwa.");
            text.AddTranslation(GameCulture.Spanish, "No autorizé ninguna escuadra... pero supongo que podriamos tomar provecho del momento mientras dura.");
            AddTranslation(text);
            text = CreateTranslation("Zavala7");
            text.SetDefault("The Darkness is extremely strong here, Guardian.");
            text.AddTranslation(GameCulture.Polish, "Ciemność jest tutaj wysoce silna Strażniku.");
            text.AddTranslation(GameCulture.Spanish, "La Oscuridad es demasiado fuerte aqui, Guardián.");
            AddTranslation(text);
            text = CreateTranslation("Zavala8");
            text.SetDefault("Do you feel that, Guardian? The Traveler's raw energies, scattered across this land.");
            text.AddTranslation(GameCulture.Polish, "Czujesz to Strażniku? Surowa energia Podróżnika, została rozproszona po całej tej krainie.");
            text.AddTranslation(GameCulture.Spanish, "¿Sientes eso, Guardián? La energia pura del Viajero, esparcida por esa tierra.");
            AddTranslation(text);
            text = CreateTranslation("Zavala9");
            text.SetDefault("Guardian.");
            text.AddTranslation(GameCulture.Polish, "Strażnik.");
            text.AddTranslation(GameCulture.Spanish, "Guardián.");
            AddTranslation(text);
            text = CreateTranslation("Zavala10");
            text.SetDefault("The Traveler graces us, Guardian.");
            text.AddTranslation(GameCulture.Polish, "Podróżnik łaskawi nas, Strażniku.");
            text.AddTranslation(GameCulture.Spanish, "El Viajero nos agradece, Guardián.");
            AddTranslation(text);
            text = CreateTranslation("Zavala11");
            text.SetDefault("Let us begin.");
            text.AddTranslation(GameCulture.Spanish, "Empezemos.");
            text.AddTranslation(GameCulture.Polish, "Zacznijmy zatem.");
            AddTranslation(text);
            text = CreateTranslation("Zavala12");
            text.SetDefault("Report, Guardian.");
            text.AddTranslation(GameCulture.Spanish, "Reportese, Guardián.");
            text.AddTranslation(GameCulture.Polish, "Raportuj, Strażniku.");
            AddTranslation(text);
            text = CreateTranslation("Zavala13");
            text.SetDefault("The Darkness approaches, Guardian. We must be ready.");
            text.AddTranslation(GameCulture.Spanish, "La Oscuridad se acerca, Guardián. Debemos estar preparados.");
            text.AddTranslation(GameCulture.Polish, "Ciemność nadchodzi strażniku. Musimy być gotowi.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaBounty1");
            text.SetDefault("I'm still setting up shop, Guardian, but you're eager to get out there, aren't you? Alright, let's see what I can find...\nI need you to kill 100 Zombies, then report back to me for further orders.");
            text.AddTranslation(GameCulture.Spanish, "Sigo preparando la tienda, Guardián, pero estas ansioso por salir afuera, no? está bien, veamos que puedo encotrar...\nNecesito que elimines 100 Zombis, luego reportate aquí para próximas ordenes.");
            text.AddTranslation(GameCulture.Polish, "Nadal zakładam sklep, Strażniku ale nie możesz się doczekać żeby wyjść, prawda? W porządku, zobaczymy co uda mi się znaleźć...\nMusisz zabić 100 Zombie a następnie zgłoś się do mnie w celu uzyskania dalszych rozkazów.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaBounty2");
            text.SetDefault("Excellent work, Guardian! Take this as my appreciation for your hard work.");
            text.AddTranslation(GameCulture.Spanish, "¡Excelente trabajo, Guardián!");
            text.AddTranslation(GameCulture.Polish, "Znakomita robota Strażniku! Weź to jako zapłatę za twoją ciężka prace.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaBounty3");
            text.SetDefault("I've got another bounty for you, Guardian. The Dungeon is an evil place, filled with servants of the Darkness.\nI need you to slay 50 Skeletons to purge this infestation.");
            text.AddTranslation(GameCulture.Spanish, "La mazmorra es un lugar vil, lleno de sirvientes de la Oscuridad. Necesito que elimines 50 Esqueletos para purgar esta infestación.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaBounty4");
            text.SetDefault("I can feel the settling of the Darkness from here. You've done well, Guardian.");
            text.AddTranslation(GameCulture.Spanish, "Puedo sentir el desvanecimiento de la Oscuridad desde aquí. Hiciste un buen trabajo, Guardián.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaKilled1");
            text.SetDefault("Let's see here, Guardian. You've killed {0}/100 Zombies.");
            text.AddTranslation(GameCulture.Spanish, "Veamos, Guardián. Eliminaste {0}/100 Zombis.");
            text.AddTranslation(GameCulture.Polish, "Zobaczmy, Strażniku. Jak dotychczas zabiłeś {0}/100 Zombie.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaKilled2");
            text.SetDefault("Let's see here, Guardian. You've killed {0}/50 Skeletons.");
            text.AddTranslation(GameCulture.Spanish, "Veamos aquí, Guardian. Has matado (number)/50 Esqueletos.");
            text.AddTranslation(GameCulture.Polish, "Zobaczmy, Strażniku. Jak dotychczas zabiłeś {0}/50 Skeleton.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaGG");
            text.SetDefault("The Guardian Games are on, Guardian! Show everyone which class is the greatest.");
            text.AddTranslation(GameCulture.Spanish, "¡Los juegos de Guardianes han comenzado, Guardián! Muestrales a todos cual clase es la mejor.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaGGTitanWin");
            text.SetDefault("Through a fierce competition and defying all odds, my Titans have punched their way to victory in this year's Guardian Games. Make no mistake; the determination of both the Warlocks and Hunters were admirable. To commemorate, here is some Titan-themed decor. Stay strong, Guardian.");
            text.AddTranslation(GameCulture.Spanish, "A través de una dura competición y desafiando todas las posibilidades, mis Titanes golpearon hasta la victoria en los Juegos de Guardianes este año. No te equivoques, la determinación de los Hechiceros y Cazadores fue admirable. Para conmemorar, aquí tienes un poco de decoración de Titán. Mantenete fuerte, Guardián.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaGGHunterWin");
            text.SetDefault("Through a fierce competition and defying all odds, the Hunters have stealthily taken the win in this year's Guardian Games. Make no mistake; the determination of the Hunters was admirable, but us Titans will achieve victory next year. To commemorate, here is some Hunter-themed decor. Stay strong, Guardian.");
            text.AddTranslation(GameCulture.Spanish, "A través de una dura competición y desafiando todas las posibilidades, los Cazadores tomaron sigilosamente la victoria en los Juegos de Guardianes este año. No te equivoques, la determinación de los Hechiceros fue admirable, pero nosotros los Titanes alcanzaremos la victoria el proximo año. Para conmemorar, aquí tienes un poco de decoración de Cazador. Mantente fuerte, Guardián.");
            AddTranslation(text);
            text = CreateTranslation("ZavalaGGWarlockWin");
            text.SetDefault("Through a fierce competition and defying all odds, the Warlocks have outsmarted every other class in this year's Guardian Games. Make no mistake; the determination of the Warlocks was admirable, but us Titans will achieve victory next year. To commemorate, here is some Warlock-themed decor. Stay strong, Guardian.");
            text.AddTranslation(GameCulture.Spanish, "A través de una dura competición y desafiando todas las posibilidades, los Hechiceros han burlado las demas clases en los Juegos de Guardianes este año. No te equivoques, la determinación de los Cazadores fue admirable, pero nosotros los Titanes alcanzaremos la victoria el proximo año. Para conmemorar, aquí tienes un poco de decoración de Hechicero. Mantente fuerte, Guardián.");
            AddTranslation(text);
            text = CreateTranslation("Drifter1");
            text.SetDefault("How you livin'?");
            text.AddTranslation(GameCulture.Spanish, "¿Qué tal?");
            text.AddTranslation(GameCulture.Polish, "Jak się miewasz?");
            AddTranslation(text);
            text = CreateTranslation("Drifter2");
            text.SetDefault("Get me those Motes and I'll make you rich, {0}, I promise.");
            text.AddTranslation(GameCulture.Spanish, "Traeme esas Motas y te haré rico, {0}, lo prometo");
            text.AddTranslation(GameCulture.Polish, "Daj mi te okruchy a ja zrobię z ciebie {0}, Obiecuje.");
            AddTranslation(text);
            text = CreateTranslation("Drifter3");
            text.SetDefault("Call me Drifter.");
            text.AddTranslation(GameCulture.Spanish, "Llamame Vagabundo.");
            text.AddTranslation(GameCulture.Polish, "Zwij mnie dryfterem.");
            AddTranslation(text);
            text = CreateTranslation("Drifter4");
            text.SetDefault("Ready to bang knuckles?");
            text.AddTranslation(GameCulture.Spanish, "¿Listo para chocar nudillos?");
            text.AddTranslation(GameCulture.Polish, "Gotowy na obijanie kostek?");
            AddTranslation(text);
            text = CreateTranslation("Drifter5");
            text.SetDefault("Transmat firing!");
            text.AddTranslation(GameCulture.Spanish, "¡Teletransportación lista!");
            text.AddTranslation(GameCulture.Polish, "Wypalanie Transmatów!");
            AddTranslation(text);
            text = CreateTranslation("Drifter6");
            text.SetDefault("Let's be bad guys.");
            text.AddTranslation(GameCulture.Spanish, "Seamos los chicos malos.");
            text.AddTranslation(GameCulture.Polish, "Bądźmy złymi chłopcami.");
            AddTranslation(text);
            text = CreateTranslation("Drifter7");
            text.SetDefault("Motes of Light have always been a thing. Motes of Dark? I had to make 'em. One day, I may have to answer for that.");
            text.AddTranslation(GameCulture.Spanish, "Las Motas de Luz siempre existieron. ¿Motas de Oscuridad? Tengo que crearlas. Algún día, quizás tenga la respuesta a eso.");
            AddTranslation(text);
            text = CreateTranslation("Drifter8");
            text.SetDefault("I see you lookin' at me like I'm nuts. You think all this is for nothing? That I do this cause I like it? You don't know the half of it.");
            text.AddTranslation(GameCulture.Spanish, "Veo que me miras como si estuviese loco. ¿Piensas que hago esto por nada?¿Que hago esto porqué quiero? No sabes nada.");
            AddTranslation(text);
            text = CreateTranslation("Drifter9");
            text.SetDefault("Ah, all the stars in heaven! I am so... hungry.");
            text.AddTranslation(GameCulture.Spanish, "¡Ah, todas las estrellas en el cielo! Estoy tan... hambriento.");
            AddTranslation(text);
            text = CreateTranslation("Drifter10");
            text.SetDefault("You think 'cause you released some fancy spirits, you're too good for me? Come on. Drifter needs his Motes.");
            text.AddTranslation(GameCulture.Spanish, "Piensas que porque liberaste algunos espiritus raros, ¿Eres demasiado bueno para mi? Vamos. El Vagabundo necesita sus Motas.");
            AddTranslation(text);
            text = CreateTranslation("Drifter11");
            text.SetDefault("Light isn't the only source of power out here.");
            text.AddTranslation(GameCulture.Spanish, "La Luz no es la única fuente de poder ahí afuera.");
            AddTranslation(text);
            text = CreateTranslation("Drifter12");
            text.SetDefault("Light, Dark... Let me tell you, the only thing that matters is the hand holding the gun.");
            text.AddTranslation(GameCulture.Spanish, "Luz, Oscuridad... Dejame decirte, lo unico que importa es la mano que sostenga el arma.");
            AddTranslation(text);
            text = CreateTranslation("Drifter13");
            text.SetDefault("What happened? Did the Taken Take the sun or somethin'?!");
            text.AddTranslation(GameCulture.Spanish, "¿Qué ocurrió? ¡¿Acaso los poseidos poseyeron el sol o algo?!");
            AddTranslation(text);
            text = CreateTranslation("Drifter14");
            text.SetDefault("Hey, {0}, wanna top off this party with some Gambit?");
            text.AddTranslation(GameCulture.Spanish, "Hey, {0}, ¿quieres animar esta escuadra con algo de gambito?");
            AddTranslation(text);
            text = CreateTranslation("Drifter15");
            text.SetDefault("Oh, no, no, no, that CREEP is here again. When will him and these...\"Nine\" stop botherin' us, {0}?");
            text.AddTranslation(GameCulture.Spanish, "Oh, no, no, no, ese EXTRAÑO esta aqui devuelta. ¿Cuando el y esos... \"Nueve\" dejaran de molestarnos, {0}?");
            AddTranslation(text);
            text = CreateTranslation("Drifter16");
            text.SetDefault("Gah, what's with {0}, {1}? He reminds me of ol' Zavala back at the Tower...what do you mean Zavala is here?!");
            text.AddTranslation(GameCulture.Spanish, "Gah, ¿que pasa con {0}, {1}? Me recuerda al viejo Zavala allá en la Torre... ¡¿Qué quieres decir con que Zavala está aquí ? !");
            AddTranslation(text);
            text = CreateTranslation("Drifter17");
            text.SetDefault("Gah, what's with {0}, {1}? He reminds me of ol' Zavala back at the Tower, always killing all the fun...");
            text.AddTranslation(GameCulture.Spanish, "Gah, ¿que pasa con {0}, {1}? Me recuerda al viejo Zavala allá en la Torre, siempre matando la diversión...");
            AddTranslation(text);
            text = CreateTranslation("Drifter18");
            text.SetDefault("We're being invaded! Find them before they find us.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes1");
            text.SetDefault("Thank you, {0}. I'll do something real special with these Motes, trust.");
            text.AddTranslation(GameCulture.Spanish, "Gracias, {0}. Haré algo realmente especial con estas motas, créeme.");
            text.AddTranslation(GameCulture.Polish, "Dziękuję {0}. Zrobię z tymi okruchami coś naprawde specialnego Zaufaj mi.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes2");
            text.SetDefault("Ooh, that's quite the haul you have there, thank you very much. I'ma do something real special with these Motes, somethin' that'll make you shiver.");
            text.AddTranslation(GameCulture.Spanish, "Ooh, ese es todo el botin que tienes ahí, muchas gracias. Haré algo muy especial con estas Motas, algo que te hará temblar.");
            text.AddTranslation(GameCulture.Polish, "Oo To niezła zdobycz, Dziękuję bardzo. zrobię coś naprawdę wyjątkowego z tymi okruchami, coś, co przyprawi Cię o dreszcze.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes3");
            text.SetDefault("Thanks for the Motes, {0}.");
            text.AddTranslation(GameCulture.Spanish, "Gracias por las Motas, {0}.");
            text.AddTranslation(GameCulture.Polish, "Dziękuję za okruchy {0}.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes4");
            text.SetDefault("Nice work. The line between Light and Dark's gettin' thinner every day. Keep walking it.");
            text.AddTranslation(GameCulture.Spanish, "Buen trabajo. La linea entre Luz y Oscuridad se vuelve mas fina cada día. Sigue así.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes5");
            text.SetDefault("Keep choosing the winning side, kid, and you'll keep on winning. Simple as that.");
            text.AddTranslation(GameCulture.Spanish, "Sigue eligiendo el lado ganador, muchacho, y seguirás ganando. Tan simple como eso.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes6");
            text.SetDefault("Hey {0}, thanks for the Motes. I said I'd make you rich, and I intend to keep that promise, unlike some others... Here, take this.");
            text.AddTranslation(GameCulture.Spanish, "Hey {0}, gracias por las Motas. He dicho que te haria rico, e intento mantener mi promesa, no como otros... aqui, ten esto.");
            text.AddTranslation(GameCulture.Polish, "Witaj {0}, dziękuję za okruchy. Mówiłem że zrobię cię bogatym i zamierzam dotrzymać tej obietnicy, nie tak jak niektórzy... Prosze weź to.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes7");
            text.SetDefault("Motes? Motes! Speaking of Motes, I've gotta way for you to get more, faster. This is for you, {0}.");
            text.AddTranslation(GameCulture.Spanish, "¿Motas? ¡Motas! Hablando de motas, conozco una forma de que consigas más, más rapido. esto es para ti, {0}.");
            text.AddTranslation(GameCulture.Polish, "Okruchy? Okruchy! Mówiąc okruchach mam dla ciebie sposób jak zdobyć więcej i to szybciej. To jest dla ciebie, {0}.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes8");
            text.SetDefault("I'll never get sick of Motes. Never. Anyways, here's your reward.");
            text.AddTranslation(GameCulture.Spanish, "I'll never get sick of Motes. Never. Anyways, here's your reward.");
            text.AddTranslation(GameCulture.Polish, "I'll never get sick of Motes. Never. Anyways, here's your reward.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes9");
            text.SetDefault("Mmm, Motes...ooh, I think you're gonna like this, {0}. Free merchandise, on the house!");
            text.AddTranslation(GameCulture.Spanish, "Mmm, Motas... ooh, pienso que esto te va a gustar, {0}. ¡Mercaderia gratis, en la casa!");
            text.AddTranslation(GameCulture.Polish, "Mmm okruchy, o myślę że to Ci się spodoba, {0}. Darmowy towar na koszt tego domu.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes10");
            text.SetDefault("Thanks for the Motes! By the way, I found these parts lying around. Maybe you could put 'em to good use, {0}?");
            text.AddTranslation(GameCulture.Spanish, "¡Gracias por las Motas! Por cierto, encontré estas partes por ahí. ¿Quizás podrias darle un buen uso, {0}?");
            text.AddTranslation(GameCulture.Polish, "Dzięki za okruchy! przy okazji znalazłem tę części po niewierające się tutaj. Może ty wykorzystasz je w sposób jaki zostały do tego stworzone {0}?");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes11");
            text.SetDefault("These are some nice Motes...oh yeah, I found this Ghost on the ground. Not sure what use it may be to you, but, it's yours now, {0}.");
            text.AddTranslation(GameCulture.Spanish, "Estas son unas buenas Motas... oh cierto, encontré esta... cosa, el el suelo. no estoy seguro de que es, pero ahora es tuyo, {0}.");
            text.AddTranslation(GameCulture.Polish, "To są niezłe okruchy, a znalazłem to leżące na ziemi. Nie jestem pewien co to jest ale teraz należy to do ciebie {0}.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes12");
            text.SetDefault("Motes, {0}, Motes! Gotta unique weapon I hand-crafted just for you. Take care of it.");
            text.AddTranslation(GameCulture.Spanish, "¡Motas, {0}, Motas! Tengo un arma que hice para ti, cuidala.");
            text.AddTranslation(GameCulture.Polish, "Motes, {0}, Motes! Gotta unique weapon I hand-crafted just for you. Take care of it.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes13");
            text.SetDefault("{0}, you gotta have those Motes on you! Come back when you got some.");
            text.AddTranslation(GameCulture.Spanish, "{0}, ¡tienes que tener las Motas contigo! Vuelve cuando tengas algunas.");
            text.AddTranslation(GameCulture.Polish, "{0}, you gotta have those Motes on you! Come back when you got some.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes14");
            text.SetDefault("Thanks for the...huh? You don't have any Motes for me to unload off 'ya!");
            text.AddTranslation(GameCulture.Spanish, "Gracias por las... ¿Heh? ¡No tienes ningunas Motas para desocuparme de ti!");
            text.AddTranslation(GameCulture.Polish, "Thanks for the...huh? You don't have any Motes for me to unload off 'ya!");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes15");
            text.SetDefault("Hey, you gotta have Motes to deposit! You tryna cheat me? Just kidding, {0}.");
            text.AddTranslation(GameCulture.Spanish, "¡Hey, tienes que depositar las Motas en el deposito! ¿Estás intentando estafarme? Es broma, {0}.");
            text.AddTranslation(GameCulture.Polish, "Hey, you gotta have Motes to deposit! You tryna cheat me? Just kidding, {0}.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes16");
            text.SetDefault("Aw man, {0}, you haven't deposited any Motes yet!");
            text.AddTranslation(GameCulture.Spanish, "¡Cielos, {0}, no depositaste Motas todavia!");
            text.AddTranslation(GameCulture.Polish, "O człowieku, {0}, jeszcze nie zdepozytowałeś jeszcze żadnych okruchów.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes17");
            text.SetDefault("Woo, you've deposited {0} Motes! This is one heckuva collection, {1}.");
            text.AddTranslation(GameCulture.Spanish, "¡Woo, depositaste {0} Motas! Esta es una gran collección, {1}.");
            text.AddTranslation(GameCulture.Polish, "Woo, Zdepozytowałeś {0} okruchy. to jest jedna cholernie duża kolekcja {1}.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes18");
            text.SetDefault("You've deposited {0} Motes so far, {1}. Not bad.");
            text.AddTranslation(GameCulture.Spanish, "Depositaste {0} Motas por el momento, {1}. Nada mal.");
            text.AddTranslation(GameCulture.Polish, "Zdepozytowałeś {0} jak narazie, {1}. Nieźle.");
            AddTranslation(text);
            text = CreateTranslation("DrifterMotes19");
            text.SetDefault("You've deposited a total of {0} Motes, {1}.");
            text.AddTranslation(GameCulture.Spanish, "Depositaste un total de {0} Motas, {1}.");
            text.AddTranslation(GameCulture.Polish, "Zdepozytowałeś w sumie {0} Okruchów, {1}.");
            AddTranslation(text);
            text = CreateTranslation("Brother");
            text.SetDefault("brother");
            text.AddTranslation(GameCulture.Spanish, "hermano");
            text.AddTranslation(GameCulture.Polish, "bracie");
            AddTranslation(text);
            text = CreateTranslation("Sister");
            text.SetDefault("sister");
            text.AddTranslation(GameCulture.Spanish, "hermana");
            text.AddTranslation(GameCulture.Polish, "siostro");
            AddTranslation(text);
            text = CreateTranslation("Bounty");
            text.SetDefault("Bounty");
            text.AddTranslation(GameCulture.Spanish, "Recompensa");
            text.AddTranslation(GameCulture.Polish, "Nagroda");
            AddTranslation(text);
            text = CreateTranslation("Decrypt");
            text.SetDefault("Decrypt");
            text.AddTranslation(GameCulture.Spanish, "Desencriptar");
            text.AddTranslation(GameCulture.Polish, "Odszyfruj"); //taken off G translate
            AddTranslation(text);
            text = CreateTranslation("GiveMotes");
            text.SetDefault("Give Motes");
            text.AddTranslation(GameCulture.Polish, "Daj okruchy");
            text.AddTranslation(GameCulture.Spanish, "Dar Motas");
            AddTranslation(text);
            text = CreateTranslation("CheckMotes");
            text.SetDefault("Check Motes");
            text.AddTranslation(GameCulture.Polish, "Sprawdź okruchy");
            text.AddTranslation(GameCulture.Spanish, "Inspeccionar Motas");
            AddTranslation(text);
            text = CreateTranslation("SuperCharge");
            text.SetDefault("Super charged!");
            text.AddTranslation(GameCulture.Spanish, "¡Súper cargada!");
            AddTranslation(text);
            text = CreateTranslation("SuperInventory");
            text.SetDefault("You must have an inventory slot free to activate your super.");
            text.AddTranslation(GameCulture.Spanish, "Debes tener un espacio del inventario vacio para activar tu súper.");
            AddTranslation(text);
            text = CreateTranslation("VaultOfGlass");
            text.SetDefault("Vault of Glass");
            text.AddTranslation(GameCulture.Spanish, "Vault of Glass");
            AddTranslation(text);
            #endregion
        }

        private void Main_DrawMouseOver(On.Terraria.Main.orig_DrawMouseOver orig, Main self) {
            Rectangle mousePos = new Rectangle((int)(Main.mouseX + Main.screenPosition.X), (int)(Main.mouseY + Main.screenPosition.Y), 1, 1);
            List<int> TypesToNotDraw = new List<int>()
            {
                ModContent.NPCType<NPCs.Vex.VaultOfGlass.EncounterBox>(),
                ModContent.NPCType<NPCs.Vex.VaultOfGlass.DetainmentBubble>()
            };
            if (Main.npc.FirstOrDefault(npc => TypesToNotDraw.Contains(npc.type) && new Rectangle((int)npc.Bottom.X - npc.frame.Width / 2, (int)npc.Bottom.Y - npc.frame.Height, npc.frame.Width, npc.frame.Height).Intersects(mousePos)) == null) {
                orig.Invoke(self);
            }
        }

        private void UICharacterListItem_DrawSelf(On.Terraria.GameContent.UI.Elements.UICharacterListItem.orig_DrawSelf orig, Terraria.GameContent.UI.Elements.UICharacterListItem self, SpriteBatch spriteBatch) {
            orig.Invoke(self, spriteBatch);
            if (!DestinyClientConfig.Instance.CharacterClassLabels)
                return;
            float width = self.GetInnerDimensions().X + self.GetInnerDimensions().Width;
            Vector2 vector4 = new Vector2(self.GetDimensions().X + 100f, self.GetInnerDimensions().Y + 59f);
            Texture2D texture = ModContent.GetTexture("Terraria/UI/InnerPanelBackground");
            float num = width - vector4.X - 380f;
            spriteBatch.Draw(texture, vector4, new Rectangle(0, 0, 8, texture.Height), Color.White);
            spriteBatch.Draw(texture, new Vector2(vector4.X + 8f, vector4.Y), new Rectangle(8, 0, 8, texture.Height), Color.White, 0f, Vector2.Zero, new Vector2((num - 16f) / 8f, 1f), SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, new Vector2(vector4.X + num - 8f, vector4.Y), new Rectangle(16, 0, 8, texture.Height), Color.White);
            string classType = "None";
            switch (((Terraria.IO.PlayerFileData)self.GetType().GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self)).Player.DestinyPlayer().classType) {
                case DestinyClassType.Titan:
                    classType = "Titan";
                    break;
                case DestinyClassType.Hunter:
                    classType = "Hunter";
                    break;
                case DestinyClassType.Warlock:
                    classType = "Warlock";
                    break;
            }
            vector4 += new Vector2(num * 0.5f - Main.fontMouseText.MeasureString(classType).X * 0.5f, 3f);
            Utils.DrawBorderString(spriteBatch, classType, vector4, Color.White);
        }

        private void ItemSlot_RightClick_ItemArray_int_int(On.Terraria.UI.ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            DestinyPlayer player = Main.LocalPlayer.DestinyPlayer();
            if (!Main.LocalPlayer.armor.IndexInRange(slot)) {
                orig.Invoke(inv, context, slot);
                return;
            }
            if (Main.LocalPlayer.armor[slot].modItem is IClassArmor armor) {
                if (armor.ArmorClassType() != player.classType && DestinyClientConfig.Instance.RestrictClassItems && context == 9) {
                    return;
                }
            }
            orig.Invoke(inv, context, slot);
        }

        private void Main_OnTick() {
            void SetUI() {
                classSelectionInterface.SetState(ClassSelectionUI);
                Main.menuMode = 888;
                Main.MenuUI.SetState(ClassSelectionUI);
                classSelecting = true;
            }
            if (Main.menuMode == 1) {
                classSelecting = false;
                if (wasJustCreating) {
                    SetUI();
                }
            }
            if (Main.menuMode == 2 && !classSelecting) {
                SetUI();
            }
            wasJustCreating = Main.menuMode == 2;
        }

        public override void Unload() {
            activateSuper = null;
            Instance = null;
            NPCs.Town.AgentOfNine.shopItems.Clear();
            NPCs.Town.AgentOfNine.itemPrices.Clear();
            NPCs.Town.AgentOfNine.itemCurrency.Clear();
            if (!Main.dedServ)
                Main.OnTick -= Main_OnTick;
        }

        private void ItemSlot_LeftClick_ItemArray_int_int(On.Terraria.UI.ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            DestinyPlayer player = Main.LocalPlayer.DestinyPlayer();
            if (Main.mouseItem.modItem is IClassArmor armor) {
                if (armor.ArmorClassType() != player.classType && DestinyClientConfig.Instance.RestrictClassItems && context == 8) {
                    return;
                }
            }
            orig.Invoke(inv, context, slot);
        }

        private Item ItemSlot_ArmorSwap(On.Terraria.UI.ItemSlot.orig_ArmorSwap orig, Item item, out bool success) {
            success = false;
            DestinyPlayer player = Main.LocalPlayer.DestinyPlayer();
            if (item.modItem is IClassArmor armor) {
                if (armor.ArmorClassType() != player.classType && DestinyClientConfig.Instance.RestrictClassItems) {
                    return item;
                }
            }
            return orig.Invoke(item, out success);
        }

        private void Player_DropTombstone(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, NetworkText deathText, int hitDirection) {
            if (currentSubworldID == string.Empty) {
                orig.Invoke(self, coinsOwned, deathText, hitDirection);
            }
        }

        public override void UpdateUI(GameTime gameTime) {
            subclassInterface?.Update(gameTime);
            superChargeInterface?.Update(gameTime);
            CryptarchUserInterface?.Update(gameTime);
            raidInterface?.Update(gameTime);
            classSelectionInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1) {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "TheDestinyMod: Subclass UI",
                    delegate {
                        if (Main.playerInventory) {
                            subclassInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
            if (inventoryIndex != -1) {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "TheDestinyMod: Cryptarch UI",
                    delegate {
                        CryptarchUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"TheDestinyMod: Super Charge UI",
					delegate {
						superChargeInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1) {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "TheDestinyMod: Raid Selection UI",
                    delegate {
                        raidInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void PreSaveAndQuit() {
            DestinyWorld.oraclesTimesRefrained = 0;
            DestinyWorld.oraclesKilledOrder = 1;
        }

        public override void Close() {
            void EndMusic(int soundSlot) {
                if (Main.music.IndexInRange(soundSlot)) {
                    var check = Main.music[soundSlot];
                    if (check != null && check.IsPlaying) {
                        Main.music[soundSlot].Stop(AudioStopOptions.Immediate);
                    }
                }
            }
            EndMusic(GetSoundSlot(SoundType.Music, "Sounds/Music/SepiksPrime"));
            base.Close();
        }

        public override void PostSetupContent() {
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            Mod subworldLibrary = ModLoader.GetMod("SubworldLibrary");
            Mod census = ModLoader.GetMod("Census");
            bossChecklist?.Call(
                "AddBoss",
                5.5f,
                ModContent.NPCType<SepiksPrime>(),
                this,
                "Sepiks Prime",
                (Func<bool>)(() => DestinyWorld.downedPrime),
                ModContent.ItemType<Items.Summons.GuardianSkull>(),
                new List<int> {
                    ModContent.ItemType<Items.Placeables.SepiksPrimeTrophy>(),
                    ModContent.ItemType<Items.Vanity.SepiksPrimeMask>(),
                    ModContent.ItemType<Items.Placeables.MusicBoxes.SepiksPrimeBox>()
                },
                new List<int> {
                    ModContent.ItemType<Items.Weapons.Summon.ServitorStaff>()
                },
                $"Use a [i:{ModContent.ItemType<Items.Summons.GuardianSkull>()}]",
                "Sepiks Prime retreats back into the House of Devils' lair..."
            );
            subworldLibrary?.Call(
                "Register",
                ModContent.GetInstance<TheDestinyMod>(),
                "Vault of Glass",
                600,
                700,
                VaultOfGlassGenPass(),
                (Action)VaultOfGlassLoad,
                (Action)VaultOfGlassUnload,
                null,
                false
            );
            census?.Call(
                "TownNPCCondition",
                ModContent.NPCType<NPCs.Town.Drifter>(),
                "Have 5 Motes of Dark in your inventory"
            );
            census?.Call(
                "TownNPCCondition",
                ModContent.NPCType<NPCs.Town.Zavala>(),
                "Defeat King Slime"
            );
            census?.Call(
                "TownNPCCondition",
                ModContent.NPCType<NPCs.Town.Cryptarch>(),
                "Have 1 Common Engram in your inventory"
            );
            census?.Call(
                "TownNPCCondition",
                ModContent.NPCType<NPCs.Town.AgentOfNine>(),
                "Traveling Merchant-like NPC that has a 1/10 chance to visit in Hardmode"
            );
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority) {
            if (Main.gameMenu || !Main.LocalPlayer.active)
                return;

            if (currentSubworldID == "TheDestinyMod_Vault of Glass") {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/VoGAmbience");
                priority = MusicPriority.BiomeHigh;
            }
        }

        public static List<GenPass> VaultOfGlassGenPass() {
            Mod subworldLibrary = ModLoader.GetMod("SubworldLibrary");
            List<GenPass> list = new List<GenPass>
            {
			    new PassLegacy("Adjusting",
                delegate (GenerationProgress progress)
                {
                    progress.Message = "Adjusting world levels";
				    Main.worldSurface = 50;
				    Main.rockLayer = 150;
                    Main.spawnTileX = 273;
                    Main.spawnTileY = 273;
                },
                1f),
			    new PassLegacy("TemplarWell",
                delegate (GenerationProgress progress)
                {
                    progress.Message = "Templar's Well";
                    DestinyHelper.StructureHelperGenerateStructure(new Vector2(100, 200), "TemplarsWell");
                },
                1f)
		    };
            subworldLibrary.Call("DrawUnderworldBackground", false);
            return list;
        }

        public static void VaultOfGlassLoad() {
            Main.LocalPlayer.noBuilding = true;
            Main.dayTime = true;
            Main.time = 10000;
            Main.mapEnabled = false;
        }

        public static void VaultOfGlassUnload() {
            currentSubworldID = string.Empty;
            Main.mapEnabled = true;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            DestinyModMessageType type = (DestinyModMessageType)reader.ReadByte();
            switch (type) {
                case DestinyModMessageType.SepiksPrime:
                    if (Main.npc[reader.ReadInt32()].modNPC is SepiksPrime prime && prime.npc.active) {
						prime.HandlePacket(reader);
					}
					break;
                default:
                    Logger.Error($"TheDestinyMod Packet Handler: Encountered unknown packet of type {type}");
                    break;
            }
        }
    }

    internal enum DestinyModMessageType : byte
    {
        SepiksPrime
    }

    /// <summary>Represents a Destiny class (not a language <see langword="class"/>).</summary>
    public enum DestinyClassType : byte
    {
        None,
        Titan,
        Hunter,
        Warlock
    }

    public enum DestinyDamageType : byte
    {
        None,
        Arc,
        Void,
        Solar,
        Stasis
    }
}