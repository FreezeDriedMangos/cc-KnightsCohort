using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
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
        public static ExternalCharacter character;
        public static ExternalDeck deck;

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
                "icons/honor"
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
                //new ExternalCard(namePrefix + "Mutual Gain", typeof(MutualGain), sprites["cards/Mutual_Gain"], deck),
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
            //var sirRatzoColor = 0;
            //unchecked { sirRatzoColor = (int)0xffbe9821; }

            //deck = new ExternalDeck(
            //    Name + ".deck.SirRatzo",
            //    System.Drawing.Color.FromArgb(sirRatzoColor),
            //    System.Drawing.Color.Black,
            //    null, //sprites["cards/Sabotage"], // TODO
            //    null, //sprites["cards/Card_Border"], // TODO
            //    null
            //);
            //if (!registry.RegisterDeck(deck)) throw new Exception("Sir Ratzo has taken his deck on a quest, cannot proceeed.");
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            ////var realStartingCards = new Type[] { typeof(OverdriveMod), typeof(RecycleParts) };
            //// TODO: initialize realStartingCards like above
            //var realStartingCards = new Type[] { };
            //var allCards = cards.Values.Select(card => card.CardType).ToList();

            //character = new ExternalCharacter(
            //    Name + ".SirRatzo",
            //    deck,
            //    null, //sprites["character/tucker_border"], // TODO
            //    realStartingCards,
            //    new Type[0],
            //    animations["sirratzo_neutral"],
            //    animations["sirratzo_mini"]
            //);

            //character.AddNameLocalisation("Sir Ratzo");
            //// TODO: write the description
            //character.AddDescLocalisation("<c=be9821>Sir Ratzo</c>\nSir Ratzo! <c=keyword>honor</c> and <c=keyword>vows</c>.");

            //if (!registry.RegisterCharacter(character)) throw new Exception("Sir Ratzo is lost! Could not register Sir Ratzo!");
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            //var animationInfo = new Dictionary<string, IEnumerable<ExternalSprite>>();
            //// these are the required animations
            //animationInfo["sirratzo_neutral"] = new ExternalSprite[] { sprites["character/tucker_neutral_1"], sprites["character/tucker_neutral_2"], sprites["character/tucker_neutral_3"], sprites["character/tucker_neutral_4"] };
            //animationInfo["sirratzo_squint"] = new ExternalSprite[] { sprites["character/tucker_squint_1"], sprites["character/tucker_squint_2"], sprites["character/tucker_squint_3"], sprites["character/tucker_squint_4"] };
            //animationInfo["sirratzo_gameover"] = new ExternalSprite[] { sprites["character/tucker_death"] };
            //animationInfo["sirratzo_mini"] = new ExternalSprite[] { sprites["character/mini_tucker"] };

            //foreach (var kvp in animationInfo)
            //{
            //    var animation = new ExternalAnimation(
            //        Name+".animations."+kvp.Key,
            //        deck,
            //        kvp.Key,
            //        false,
            //        kvp.Value
            //    );
            //    animations[kvp.Key] = animation;

            //    if (!registry.RegisterAnimation(animation)) throw new Exception("Error registering animation " + kvp.Key);
            //}
        }

        public void LoadManifest(IGlossaryRegisty registry)
        {
            //RegisterGlossaryEntry(registry, "AReplay", sprites["icons/Replay"],
            //    "play twice",
            //    "Play all actions prior to the Play Twice action twice."
            //);
        }
        private void RegisterGlossaryEntry(IGlossaryRegisty registry, string itemName, ExternalSprite sprite, string displayName, string description)
        {
            var entry = new ExternalGlossary(Name + ".Glossary." + itemName, itemName, false, ExternalGlossary.GlossayType.action, sprite);
            entry.AddLocalisation("en", displayName, description);
            registry.RegisterGlossary(entry);
            glossary[entry.ItemName] = entry;
        }

        public void LoadManifest(IStatusRegistry statusRegistry)
        {
            var honorColor = 0;
            unchecked { honorColor = (int)0xfff1c442; }

            var honor = new ExternalStatus(Name + ".statuses.honor", true, System.Drawing.Color.FromArgb(honorColor), null, sprites["icons/honor"], false);
            statusRegistry.RegisterStatus(honor);
            honor.AddLocalisation("Honor", "Once your honor has matched the opponent's remaining hull and shield, they will flee the battle, leaving you victorious.");
            statuses["honor"] = honor;
        }

        public void LoadManifest(IArtifactRegistry registry)
        {
            //var antiqueMotor = new ExternalArtifact(Name + ".Artifacts.Antique_Motor", typeof(AntiqueMotor), sprites["icons/Antique_Motor"], ownerDeck: deck);
            //antiqueMotor.AddLocalisation("ANTIQUE MOTOR", "Gain 1 extra <c=energy>ENERGY</c> every turn. <c=downside>Gain 1</c> <c=status>FUEL LEAK</c> <c=downside>on the first turn</c>.");
            //registry.RegisterArtifact(antiqueMotor);

        }
    }
}
