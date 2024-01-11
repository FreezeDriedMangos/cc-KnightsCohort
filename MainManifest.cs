using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
using KnightsCohort.Bannerlady.Cards;
using KnightsCohort.Herbalist;
using KnightsCohort.Herbalist.Cards;
using KnightsCohort.Knight;
using KnightsCohort.Knight.Cards;
using Microsoft.Extensions.Logging;
using shockah;
using Shockah.Kokoro;

namespace KnightsCohort
{
    public class MainManifest : IModManifest, ISpriteManifest, ICardManifest, ICharacterManifest, IDeckManifest, IAnimationManifest, IGlossaryManifest, IStatusManifest, IArtifactManifest, IStoryManifest
    {
        public static readonly string MOD_NAMESPACE = "clay.KnightsCohort";
        public static MainManifest Instance;
        public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[] { new DependencyEntry<IModManifest>("Shockah.Kokoro", false) };

        public DirectoryInfo? GameRootFolder { get; set; }
        public Microsoft.Extensions.Logging.ILogger? Logger { get; set; }
        public DirectoryInfo? ModRootFolder { get; set; }

        public string Name => MOD_NAMESPACE;

        public static Dictionary<string, ExternalSprite> sprites = new Dictionary<string, ExternalSprite>();
        public static Dictionary<string, ExternalAnimation> animations = new Dictionary<string, ExternalAnimation>();
        public static Dictionary<string, ExternalCard> cards = new Dictionary<string, ExternalCard>();
        public static Dictionary<string, ExternalStatus> statuses = new Dictionary<string, ExternalStatus>();
        public static Dictionary<string, ExternalGlossary> glossary = new Dictionary<string, ExternalGlossary>();
        public static Dictionary<string, ExternalDeck> decks = new Dictionary<string, ExternalDeck>();
        public static Dictionary<string, ExternalCharacter> characters = new Dictionary<string, ExternalCharacter>();

        public static IKokoroApi KokoroApi = null!;
        internal static VowsRenderer VowsRenderer = null;

        public void BootMod(IModLoaderContact contact)
        {
            ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.dll"));
            ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.Harmony.dll"));

            Instance = this;
            var harmony = new Harmony(this.Name);
            harmony.PatchAll();

            KokoroApi = contact.GetApi<IKokoroApi>("Shockah.Kokoro")!;
            VowsRenderer = new();
        }

        public void LoadManifest(ISpriteRegistry artRegistry)
        {
            var filenames = new string[] {
                "icons/honor",
                "icons/honor_cost",
                "icons/honor_cost_unsatisfied",

                // knight
                "character/knight_neutral_0",
                "character/knight_neutral_1",
                "character/knight_neutral_2",
                "character/knight_mini",

                "frame_knight",
                "card_default_knight",
                "char_frame_knight",

                // bannerlady
                "character/bannerlady_neutral_1",
                "character/bannerlady_neutral_2",
                "character/bannerlady_neutral_3",
                "character/bannerlady_neutral_4",
                "character/bannerlady_squint_1",
                "character/bannerlady_squint_2",
                "character/bannerlady_squint_3",
                "character/bannerlady_squint_4",
                "character/bannerlady_gameover",
                "character/bannerlady_mini",

                "frame_bannerlady",
                "card_default_bannerlady",
                "char_frame_bannerlady",

                // herbalist
                "character/herbalist_neutral_1",
                "character/herbalist_neutral_2",
                "character/herbalist_neutral_3",
                "character/herbalist_neutral_4",
                "character/herbalist_squint_1",
                "character/herbalist_squint_2",
                "character/herbalist_squint_3",
                "character/herbalist_squint_4",
                "character/herbalist_mini",
                "frame_herb",
                "frame_herbalist",
                "card_default_herbalist",
                "char_frame_herbalist",

                // misc

                //"midrow/sword", // sprite exits in banilla
                "midrow/dagger",
                "midrow/excalibur",

                //"icons/missile_sword", // sprite exists in vanilla
                "icons/missile_dagger",
                "icons/missile_excalibur",
                "icons/oathbreaker",
                "icons/vow_of_mercy",
                "icons/vow_of_adamancy",
                "icons/vow_of_teamwork",
                "icons/vow_of_action",
                "icons/vow_of_courage", 
                "icons/vow_of_left",    
                "icons/vow_of_right",   
                "icons/vow_of_chivalry",
                "icons/vow_of_rest",
                "icons/vow_of_mega_rest",
                "icons/vow_of_poverty",
                "icons/vow_of_middling_income",
                "icons/vow_of_affluence",

                "midrow/banner_of_mercy",
                "midrow/banner_of_martyr",
                "midrow/banner_of_war",
                "midrow/banner_of_pirate",
                "midrow/banner_of_inspiration",
                "midrow/banner_of_shielding",
                "midrow/tattered_banner_of_inspiration",
                "midrow/tattered_banner_of_war",
                "midrow/tattered_banner_of_martyr",
                "midrow/tattered_banner_of_pirate",
                "midrow/arrow",
                "midrow/broadhead_arrow",

                "icons/charge",
                "icons/charge_directional",
                "icons/retreat",
                "icons/retreat_directional",
                "icons/flurry",
                "icons/shieldOfFaith",
                "icons/banner_mercy",
                "icons/banner_martyr",
                "icons/banner_war",
                "icons/banner_pirate",
                "icons/banner_inspiration",
                "icons/banner_shielding",
                "icons/tattered_banner_inspiration",
                "icons/tattered_banner_war",
                "icons/tattered_banner_martyr",
                "icons/tattered_banner_pirate",
                "icons/arrow",
                "icons/broadhead_arrow",

                "icons/dazed",
                "icons/blindness",
                "icons/herb_bundle",
                "icons/herb_bundle_add_oxidize",
                "icons/herb_search",
                "icons/CorrodeCostSatisfied",
                "icons/CorrodeCostUnsatisfied",
            };

            foreach (var filename in filenames) {
                var filepath = Path.Combine(ModRootFolder?.FullName ?? "", "sprites", Path.Combine(filename.Split('/'))+".png");
                var sprite = new ExternalSprite(Name+".sprites." +filename, new FileInfo(filepath));
                sprites[filename] = sprite;

                if (!artRegistry.RegisterArt(sprite)) throw new Exception("Error registering sprite " + filename);
            }
        }

        public void LoadManifest(ICardRegistry registry)
        {
            // GOAL:
            // 21 cards
            // 9 common, 7 uncommon, 5 rare
            var namePrefix = Name + ".cards.";
            var cardDefinitions = new ExternalCard[]
            {
                new ExternalCard(namePrefix + "Fighting Chance", typeof(FightingChance), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Offhand Weapon", typeof(OffhandWeapon), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Claymore", typeof(Claymore), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Footwork", typeof(Footwork), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Unmoving Faith", typeof(UnmovingFaith), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Fix Your Form", typeof(FixYourForm), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Excalibur", typeof(Excalibur), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Teamwork", typeof(Teamwork), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Cheap Shot", typeof(CheapShot), sprites["card_default_knight"], decks["knight"]),
                //new ExternalCard(namePrefix + "Honorable Strike", typeof(HonorableStrike), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Riposte", typeof(RiposteCard), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Financial Advice", typeof(FinancialAdvice), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Knight's Rest", typeof(KnightsRest), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Unrelenting Oath", typeof(UnrelentingOath), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Truce", typeof(Truce), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Shield Bash", typeof(ShieldBash), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Free Hit", typeof(FreeHit), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Handicap", typeof(Handicap), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Oathbreaker", typeof(Oathbreaker), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Friendly Duel", typeof(FriendlyDuel), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Challenge", typeof(Challenge), sprites["card_default_knight"], decks["knight"]),
                new ExternalCard(namePrefix + "Shield of Honor", typeof(ShieldOfHonor), sprites["card_default_knight"], decks["knight"]),






                new ExternalCard(namePrefix + "Remembered Glory", typeof(RememberedGlory), sprites["card_default_bannerlady"], decks["bannerlady"]),
                //new ExternalCard(namePrefix + "Charge Ahead", typeof(ChargeAhead), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Lead from the Front", typeof(LeadFromTheFront), sprites["card_default_bannerlady"], decks["bannerlady"]),
                //new ExternalCard(namePrefix + "Bow and Arrow", typeof(BowAndArrow), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Pity", typeof(BannerladyPity), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Bodyguard", typeof(BannerladyBodyguard), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Rest and Reprieve", typeof(RestAndReprieve), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Valiant Charge", typeof(ValiantCharge), sprites["card_default_bannerlady"], decks["bannerlady"]),
                //new ExternalCard(namePrefix + "Archery Training", typeof(ArcheryTraining), sprites["card_default_bannerlady"], decks["bannerlady"]),
                //new ExternalCard(namePrefix + "Platemail Piercer", typeof(PlatemailPiercer), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Telegraph", typeof(BannerladyTelegraph), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Martyrdom", typeof(Martyrdom), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Is It War?", typeof(IsItWar), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Covered Retreat", typeof(CoveredRetreat), sprites["card_default_bannerlady"], decks["bannerlady"]),
                //new ExternalCard(namePrefix + "Master of Archery", typeof(MasterOfArchery), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "False Flag", typeof(FalseFlag), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Shield of Faith", typeof(ShieldOfFaith), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Raise Morale", typeof(RaiseMorale), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Cover Me", typeof(CoverMe), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Deadly Conviction", typeof(DeadlyConviction), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Diplomatic Immunity", typeof(DiplomaticImmunity), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Desperate Measures", typeof(BannerladyDesperateMeasures), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Lifted Burdens", typeof(LiftedBurdens), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Rally", typeof(BannerladyRally), sprites["card_default_bannerlady"], decks["bannerlady"]),
                new ExternalCard(namePrefix + "Sharp Eyes", typeof(SharpEyes), sprites["card_default_bannerlady"], decks["bannerlady"]),



                new ExternalCard(namePrefix + "Literally Doesn't Exist", typeof(HerbCard), sprites["card_default_herbalist"], decks["herbs"]),
                new ExternalCard(namePrefix + "Safe to Drink. Probably.", typeof(HerbCard_Tea), sprites["card_default_herbalist"], decks["herbs"]),
                new ExternalCard(namePrefix + "Poultice", typeof(HerbCard_Poultice), sprites["card_default_herbalist"], decks["herbs"]),
                new ExternalCard(namePrefix + "Mystery Leaf", typeof(HerbCard_Leaf), sprites["card_default_herbalist"], decks["herbs"]),
                new ExternalCard(namePrefix + "Mystery Bark", typeof(HerbCard_Bark), sprites["card_default_herbalist"], decks["herbs"]),
                new ExternalCard(namePrefix + "Mystery Seed", typeof(HerbCard_Seed), sprites["card_default_herbalist"], decks["herbs"]),
                new ExternalCard(namePrefix + "Mystery Root", typeof(HerbCard_Root), sprites["card_default_herbalist"], decks["herbs"]),
                new ExternalCard(namePrefix + "Dont Eat This", typeof(HerbCard_Shroom), sprites["card_default_herbalist"], decks["herbs"]),

                new ExternalCard(namePrefix + "Leaf Pack", typeof(LeafPack), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Bark Pack", typeof(BarkPack), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Seed Pack", typeof(SeedPack), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Root Pack", typeof(RootPack), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Shroom Pack", typeof(ShroomPack), sprites["card_default_herbalist"], decks["herbalist"]),

                new ExternalCard(namePrefix + "Mortar & Pestle", typeof(MortarAndPestle), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Smolder", typeof(Smolder), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Publish Findings", typeof(PublishFindings), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Forage", typeof(Forage), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Consume", typeof(Consume), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "QUEST", typeof(QUEST), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Quest Reward", typeof(QuestReward), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Epic Quest Reward", typeof(EpicQuestReward), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Compassion", typeof(Compassion), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Call on Debts", typeof(CallOnDebts), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Brew Tea", typeof(BrewTea), sprites["card_default_herbalist"], decks["herbalist"]),
                new ExternalCard(namePrefix + "Fire and Smoke", typeof(FireAndSmoke), sprites["card_default_herbalist"], decks["herbalist"]),
            };
            
            foreach(var card in cardDefinitions)
            {
                var name = card.GlobalName.Split('.').LastOrDefault() ?? "FAILED TO FIND NAME";
                card.AddLocalisation(name);
                registry.RegisterCard(card);
                cards[name] = card;
            }
        }

        public void LoadManifest(IDeckRegistry registry)
        {
            var knightColor = 0;
            unchecked { knightColor = (int)0xffbe9821; }

            decks["knight"] = new ExternalDeck(
                Name + ".deck.Knight",
                System.Drawing.Color.FromArgb(knightColor),
                System.Drawing.Color.Black,
                sprites["card_default_knight"],
                sprites["frame_knight"],
                null
            );
            if (!registry.RegisterDeck(decks["knight"])) throw new Exception("Sir Ratzo has taken his deck on a quest, cannot proceeed.");


            decks["bannerlady"] = new ExternalDeck(
                Name + ".deck.Bannerlady",
                System.Drawing.Color.FromArgb(knightColor),
                System.Drawing.Color.Black,
                sprites["card_default_bannerlady"],
                sprites["frame_bannerlady"],
                null
            );
            if (!registry.RegisterDeck(decks["bannerlady"])) throw new Exception("Dame Emily has taken her deck on campaign, cannot proceeed.");


            decks["herbalist"] = new ExternalDeck(
                Name + ".deck.Herbalist",
                System.Drawing.Color.FromArgb(knightColor),
                System.Drawing.Color.White,
                sprites["card_default_herbalist"],
                sprites["frame_herbalist"],
                null
            );
            if (!registry.RegisterDeck(decks["herbalist"])) throw new Exception("Dame Halla has taken her deck and wandered off into the forest, cannot proceeed.");


            decks["herbs"] = new ExternalDeck(
                Name + ".deck.Herbs",
                System.Drawing.Color.FromArgb(knightColor),
                System.Drawing.Color.White,
                sprites["card_default_herbalist"],
                sprites["frame_herb"],
                null
            );
            if (!registry.RegisterDeck(decks["herbs"])) throw new Exception("It's the end. Climate change has eliminated all herbs in existence. Cannot proceed.");
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            characters["knight"] = new ExternalCharacter(
                Name + ".Knight",
                decks["knight"],
                sprites["char_frame_knight"],
                new Type[] { typeof(Knight.Cards.FightingChance), typeof(Knight.Cards.OffhandWeapon) },
                new Type[0],
                animations["knight.neutral"],
                animations["knight.mini"]
            );

            characters["knight"].AddNameLocalisation("Sir Ratzo");
            // TODO: write the description
            characters["knight"].AddDescLocalisation("<c=be9821>Sir Ratzo</c>\nSir Ratzo! <c=keyword>honor</c> and <c=keyword>vows</c>.");

            if (!registry.RegisterCharacter(characters["knight"])) throw new Exception("Sir Ratzo is lost! Could not register Sir Ratzo!");


            characters["bannerlady"] = new ExternalCharacter(
                Name + ".Bannerlady",
                decks["bannerlady"],
                sprites["char_frame_bannerlady"],
                new Type[] { typeof(Bannerlady.Cards.RememberedGlory), typeof(Bannerlady.Cards.LiftedBurdens) },
                new Type[0],
                animations["bannerlady.neutral"],
                animations["bannerlady.mini"]
            );

            characters["bannerlady"].AddNameLocalisation("Dame Emily");
            // TODO: write the description
            characters["bannerlady"].AddDescLocalisation("<c=be9821>Dame Emily</c>\nThe Bannerlady, Dame Emily! <c=keyword>glory</c> and <c=keyword>banners</c>.");

            if (!registry.RegisterCharacter(characters["bannerlady"])) throw new Exception("Dame Emily is lost! Could not register Dame Emily!");


            characters["herbalist"] = new ExternalCharacter(
                Name + ".Herbalist",
                decks["herbalist"],
                sprites["char_frame_herbalist"],
                new Type[] { typeof(Herbalist.Cards.MortarAndPestle), typeof(Herbalist.Cards.Smolder), typeof(Herbalist.Cards.LeafPack) },
                new Type[0],
                animations["herbalist.neutral"],
                animations["herbalist.mini"]
            );

            characters["herbalist"].AddNameLocalisation("Dame Halla");
            // TODO: write the description
            characters["herbalist"].AddDescLocalisation("<c=be9821>Dame Halla</c>\nThe Herbalist, Dame Halla! <c=keyword>herbs</c> and <c=keyword>honor</c>.");

            if (!registry.RegisterCharacter(characters["herbalist"])) throw new Exception("Dame Halla is lost! Could not register Dame Halla!");
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            var animationInfo = new Dictionary<string, IEnumerable<ExternalSprite>>();
            // these are the required animations
            animationInfo["knight.neutral"] = new ExternalSprite[] { sprites["character/knight_neutral_0"], sprites["character/knight_neutral_1"], sprites["character/knight_neutral_2"] };
            //animationInfo["knight.squint"] = new ExternalSprite[] { sprites["character/tucker_squint_1"], sprites["character/tucker_squint_2"], sprites["character/tucker_squint_3"], sprites["character/tucker_squint_4"] };
            //animationInfo["knight.gameover"] = new ExternalSprite[] { sprites["character/tucker_death"] };
            animationInfo["knight.mini"] = new ExternalSprite[] { sprites["character/knight_mini"] };

            animationInfo["bannerlady.neutral"] = new ExternalSprite[] { sprites["character/bannerlady_neutral_1"], sprites["character/bannerlady_neutral_2"], sprites["character/bannerlady_neutral_3"], sprites["character/bannerlady_neutral_4"] };
            animationInfo["bannerlady.squint"] = new ExternalSprite[] { sprites["character/bannerlady_squint_1"], sprites["character/bannerlady_squint_2"], sprites["character/bannerlady_squint_3"], sprites["character/bannerlady_squint_4"] };
            animationInfo["bannerlady.gameover"] = new ExternalSprite[] { sprites["character/bannerlady_gameover"] };
            animationInfo["bannerlady.mini"] = new ExternalSprite[] { sprites["character/bannerlady_mini"] };

            animationInfo["herbalist.neutral"] = new ExternalSprite[] { sprites["character/herbalist_neutral_1"], sprites["character/herbalist_neutral_2"], sprites["character/herbalist_neutral_3"], sprites["character/herbalist_neutral_4"] };
            animationInfo["herbalist.mini"] = new ExternalSprite[] { sprites["character/herbalist_mini"] };

            foreach (var kvp in animationInfo)
            {
                var parts = kvp.Key.Split('.');
                var deckname = parts[0];
                var animationname = parts[1];

                var animation = new ExternalAnimation(
                    Name + ".animations." + kvp.Key,
                    decks[deckname],
                    animationname,
                    false,
                    kvp.Value
                );
                animations[kvp.Key] = animation;

                if (!registry.RegisterAnimation(animation)) throw new Exception("Error registering animation " + kvp.Key);
            }
        }

        public void LoadManifest(IGlossaryRegisty registry)
        {
            RegisterGlossaryEntry(registry, "missileDagger", sprites["icons/missile_dagger"],
                "DAGGER",
                "This missile is going to deal <c=damage>{0}</c> damage."
            );
            RegisterGlossaryEntry(registry, "missileSword", sprites["icons/missile_dagger"],
                "SWORD",
                "This missile is going to deal <c=damage>{0}</c> damage."
            );
            RegisterGlossaryEntry(registry, "missileExcalibur", sprites["icons/missile_excalibur"],
                "EXCALIBUR",
                "This missile is going to deal damage equal to the origin ship's honor."
            );

            RegisterGlossaryEntry(registry, "missileArrow", sprites["icons/arrow"],
                "ARROW",
                "This missile is going to deal <c=damage>{0}</c> damage, piercing shields and armor."
            );
            RegisterGlossaryEntry(registry, "missileBroadheadArrow", sprites["icons/broadhead_arrow"],
                "BROADHEAD ARROW",
                "This missile is going to deal <c=damage>{0}</c> damage, piercing shields and armor."
            );

            RegisterGlossaryEntry(registry, "charge", sprites["icons/charge"],
                "CHARGE",
                "Move your ship's center {0} towards the enemy ship's center."
            );
        }
        private void RegisterGlossaryEntry(IGlossaryRegisty registry, string itemName, ExternalSprite sprite, string displayName, string description)
        {
            var entry = new ExternalGlossary(Name + ".Glossary." + itemName, itemName, false, ExternalGlossary.GlossayType.action, sprite);
            entry.AddLocalisation("en", displayName, description);
            registry.RegisterGlossary(entry);
            glossary[itemName] = entry;
        }

        public void LoadManifest(IStatusRegistry statusRegistry)
        {
            var honorColor = 0;
            unchecked { honorColor = (int)0xfff1c442; }

            var status = "honor";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/honor"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Honor", "Once your honor has matched the opponent's remaining hull and shield, they will flee the battle, leaving you victorious.");

            status = "oathbreaker";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/oathbreaker"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Oathbreaker", "Whenever you lose honor, gain an equal amount of temp shield.");

            status = "vowOfMercy";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_mercy"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Mercy", $"At the end of your turn, if you have not attacked this turn, gain {VowsController.VOW_OF_MERCY_HONOR} honor. Lose 1 Vow of Mercy.");

            status = "vowOfAdamancy";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_adamancy"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Adamancy", $"Gain {{0}} honor at the start of your turn. If you move by any effect, lose all stacks of this vow.");

            status = "vowOfTeamwork";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_teamwork"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Teamwork", $"Gain {{0}} honor at the start of your turn. If you play two or more cards from the same crew member in one turn, lose all stacks of this vow.");

            status = "vowOfAction";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_action"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Action", $"Gain {{0}} honor at the start of your turn. If you gain shield by any effect, lose all stacks of this vow.");

            status = "vowOfCourage";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_courage"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Courage", $"If you are hit by the opponent's cannons, gain {VowsController.VOW_OF_COURAGE_HONOR} honor for each stack of this vow. Lose 1 stack of this vow at the start of your turn.");

            status = "vowOfLeft";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_left"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of the Left Hand", $"Gain {{0}} honor at the start of your turn. If you move right by any effect or play your rightmost card, lose all stacks of this vow.");

            status = "vowOfRight";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_right"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of the Right Hand", $"Gain {{0}} honor at the start of your turn. If you move left by any effect or play your leftmost card, lose all stacks of this vow.");

            status = "vowOfChivalry";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_chivalry"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Chivalry", $"Gain {{0}} honor at the start of your turn. If you hit your opponent in a weak or brittle spot with a drone or your cannon, lose all stacks of this vow.");

            status = "vowOfRest";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_rest"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Rest", $"Gain {{0}} honor at the start of your turn. If end your turn with less than 1 energy, lose all stacks of this vow.");
            
            status = "vowOfMegaRest";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_mega_rest"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Mega Rest", $"Gain {{0}} honor at the start of your turn. If end your turn with less than 2 energy, lose all stacks of this vow.");

            status = "vowOfPoverty";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_poverty"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Poverty", $"Gain {{0}} honor at the start of your turn. If you play a card that costs 0 energy, lose all stacks of this vow.");

            status = "vowOfMiddlingIncome";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_middling_income"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Middling Income", $"Gain {{0}} honor at the start of your turn. If you play a card that costs 1 energy, lose all stacks of this vow.");

            status = "vowOfAffluence";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_affluence"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Affluence", $"Gain {{0}} honor at the start of your turn. If you play a card that costs 2 energy, lose all stacks of this vow.");

            status = "flurry";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/flurry"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Flurry", $"Launch 1 arrow and droneshift right 1 at the end of every turn.");

            status = "shieldOfFaith";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/shieldOfFaith"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Shield of Faith", $"Banners block shots. Decrease by 1 at the start of every turn.");


            status = "dazed";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/dazed"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Dazed", $"Flip direction of movement for all effects. Lose one stack at the start of your turn.");

            status = "blindness";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/blindness"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Blindness", $"Card titles and actions do not render. On opponent, causes random movement. Lose one stack at the start of your turn.");
        }

        public void LoadManifest(IArtifactRegistry registry)
        {
            //var antiqueMotor = new ExternalArtifact(Name + ".Artifacts.Antique_Motor", typeof(AntiqueMotor), sprites["icons/Antique_Motor"], ownerDeck: deck);
            //antiqueMotor.AddLocalisation("ANTIQUE MOTOR", "Gain 1 extra <c=energy>ENERGY</c> every turn. <c=downside>Gain 1</c> <c=status>FUEL LEAK</c> <c=downside>on the first turn</c>.");
            //registry.RegisterArtifact(antiqueMotor);

        }

        public void LoadManifest(IStoryRegistry storyRegistry)
        {
            storyRegistry.RegisterStory(new ExternalStory(
                "clay.KnightsCohort.Honorable_Win",
                node: new StoryNode()
                {
                    type = Enum.Parse<NodeType>("event"),
                    never = true,
                    oncePerCombat = true,
                },
                instructions: new List<object>()
                {
                    new ExternalStory.ExternalSaySwitch(new()
                    {
                        new ExternalStory.ExternalSay()
                        {
                            Who="comp",
                            What="Goodbye!"
                        },
                        new ExternalStory.ExternalSay()
                        {
                            Who="comp",
                            What="Good fight!"
                        },
                        new ExternalStory.ExternalSay()
                        {
                            Who="comp",
                            What="See ya later!"
                        },
                        new ExternalStory.ExternalSay()
                        {
                            Who="comp",
                            What="See you next loop I guess!"
                        }
                    })
                }
            ));
        }
    }
}
