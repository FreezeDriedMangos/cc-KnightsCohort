using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
using KnightsCohort.Knight;
using KnightsCohort.Knight.Cards;
using Microsoft.Extensions.Logging;
using shockah;

namespace KnightsCohort
{
    public class MainManifest : IModManifest, ISpriteManifest, ICardManifest, ICharacterManifest, IDeckManifest, IAnimationManifest, IGlossaryManifest, IStatusManifest, IArtifactManifest, IStoryManifest
    {
        public static MainManifest Instance;

        public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[0];

        public DirectoryInfo? GameRootFolder { get; set; }
        public Microsoft.Extensions.Logging.ILogger? Logger { get; set; }
        public DirectoryInfo? ModRootFolder { get; set; }

        public string Name => "clay.KnightsCohort";

        public static Dictionary<string, ExternalSprite> sprites = new Dictionary<string, ExternalSprite>();
        public static Dictionary<string, ExternalAnimation> animations = new Dictionary<string, ExternalAnimation>();
        public static Dictionary<string, ExternalCard> cards = new Dictionary<string, ExternalCard>();
        public static Dictionary<string, ExternalStatus> statuses = new Dictionary<string, ExternalStatus>();
        public static Dictionary<string, ExternalGlossary> glossary = new Dictionary<string, ExternalGlossary>();
        public static Dictionary<string, ExternalDeck> decks = new Dictionary<string, ExternalDeck>();
        public static Dictionary<string, ExternalCharacter> characters = new Dictionary<string, ExternalCharacter>();

        public void BootMod(IModLoaderContact contact)
        {
            ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.dll"));
            ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.Harmony.dll"));

            Instance = this;
            var harmony = new Harmony(this.Name);
            harmony.PatchAll();
        }

        public void LoadManifest(ISpriteRegistry artRegistry)
        {
            var filenames = new string[] {
                "icons/honor",

                "character/knight_neutral_0",
                "character/knight_neutral_1",
                "character/knight_neutral_2",

                "frame_knight",
                "card_default_knight",
                "char_frame_knight",

                //"midrow/sword", // sprite exits in banilla
                "midrow/dagger",
                "midrow/excalibur",

                //"icons/missile_sword", // sprite exists in vanilla
                "icons/missile_dagger",
                "icons/missile_excalibur",
                "icons/vow_of_mercy",
                "icons/vow_of_adamancy",
                "icons/vow_of_teamwork",
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
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            characters["knight"] = new ExternalCharacter(
                Name + ".Knight",
                decks["knight"],
                sprites["char_frame_knight"],
                new Type[] { typeof(Knight.Cards.FightingChance), typeof(Knight.Cards.OffhandWeapon) },
                new Type[0],
                animations["neutral"],
                animations["mini"]
            );

            characters["knight"].AddNameLocalisation("Sir Ratzo");
            // TODO: write the description
            characters["knight"].AddDescLocalisation("<c=be9821>Sir Ratzo</c>\nSir Ratzo! <c=keyword>honor</c> and <c=keyword>vows</c>.");

            if (!registry.RegisterCharacter(characters["knight"])) throw new Exception("Sir Ratzo is lost! Could not register Sir Ratzo!");
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            var animationInfo = new Dictionary<string, IEnumerable<ExternalSprite>>();
            // these are the required animations
            animationInfo["neutral"] = new ExternalSprite[] { sprites["character/knight_neutral_0"], sprites["character/knight_neutral_1"], sprites["character/knight_neutral_2"] };
            //animationInfo["squint"] = new ExternalSprite[] { sprites["character/tucker_squint_1"], sprites["character/tucker_squint_2"], sprites["character/tucker_squint_3"], sprites["character/tucker_squint_4"] };
            //animationInfo["gameover"] = new ExternalSprite[] { sprites["character/tucker_death"] };
            animationInfo["mini"] = new ExternalSprite[] { sprites["character/knight_neutral_0"] };

            foreach (var kvp in animationInfo)
            {
                var animation = new ExternalAnimation(
                    Name + ".animations." + kvp.Key,
                    decks["knight"],
                    kvp.Key,
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
                "This missile is going to deal <c=damage>{0}</c> damage, piercing shields and armor."
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
            statuses[status].AddLocalisation(status, "Once your honor has matched the opponent's remaining hull and shield, they will flee the battle, leaving you victorious.");

            status = "vowOfMercy";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_mercy"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Mercy", "At the end of your turn, if you have not attacked this turn, gain 1 honor. Lose 1 Vow of Mercy.");

            status = "vowOfAdamancy";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_adamancy"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Adamancy", $"If you move by any effect, lose {VowsController.VOW_OF_ADAMANCY_HONOR} Honor for each stack of this vow, and lose all stacks of this vow.");

            status = "vowOfTeamwork";
            statuses[status] = new ExternalStatus(Name + ".statuses." + status, true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/vow_of_teamwork"], false);
            statusRegistry.RegisterStatus(statuses[status]);
            statuses[status].AddLocalisation("Vow of Teamwork", $"If you play two or more cards from the same crew member in one turn, lose {VowsController.VOW_OF_TEAMWORK_HONOR} honor for each stack of this vow, and lose all stacks of this vow.");
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
