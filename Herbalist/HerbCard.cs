using HarmonyLib;
using KnightsCohort.actions;
using KnightsCohort.Herbalist.Artifacts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KnightsCohort.Herbalist.Cards.HerbPack;

namespace KnightsCohort.Herbalist
{
    public enum HerbActions
    {
        OXIDATION,
        TEMPSHIELD,
        OVERDRIVE,
        DAZED,
        BLINDNESS,
        STUNCHARGE,
        AUTODODGE_RIGHT,
        REMOVE_CORRODE,
        SHIELD,
        NEGATIVE_OVERDIVE,
        EVADE,
        HERMESBOOTS,
        INSTANTMOVE_LEFT,
        INSTANTMOVE_RIGHT,
        HEAL,
        HULLDAMAGE,
        HEAT,
        POWERDRIVE,
        NEGATIVE_OXIDATION,
        ENGINESTALL,
        INSTANTMOVE_RANDOM,
        ENGINELOCK,
        PAYBACK,
        FLUX,
        PARANOIA,
    }

    [HarmonyPatch]
    public class HerbCard : Card
    {
        public static readonly string CATALOGUED_HERBS_KEY = "cataloguedHerbs";

        protected virtual List<HerbActions> GenerateSerializedActions(State s) { return new(); }
        protected virtual string GetTypeName() { return "INVALID"; }
        protected virtual List<HerbActions> possibleOptions => new();
        public virtual bool IsRaw => false;
        public bool isPoultice = false;
        public bool isTea = false;
        public bool isCultivated = false;

        public List<HerbActions> PotentialActions = new();

        public List<HerbActions> SerializedActions = new();
        public string name;
        public bool revealed;
        public int revealTimer = 3;
        public HerbCard() { }

        public static HerbCard Generate(State s, HerbCard c)
        {
            c.SerializedActions = c.GenerateSerializedActions(s);
            c.name = GenerateFakeLatinWord(s) + " " + c.GetTypeName();
            c.PotentialActions = c.possibleOptions;
            c.revealed = s.EnumerateAllArtifacts().Where(a => a is FieldJournal).Any();

            return (HerbCard)c;
        }

        public static List<string> syllables = new List<string> { "ven", "lum", "aqu", "ter", "son", "lux", "nov", "mort", "vit", "fer", "grat", "vis", "sci", "cit" };
        public static List<string> prefixes = new List<string> { "exo", "intra", "circum", "trans", "pro", "sub" };
        public static List<string> suffixes = new List<string> { "us", "a", "um", "ium", "ito", "ensis" };
        private static string GenerateFakeLatinWord(State s)
        {
            string n = "";
            Rand rng = s.rngCurrentEvent;
            int numSyllables = 1 + (int)(rng.Next() * 2);

            for (int i = 0; i < numSyllables; i++) n += syllables.KnightRandom(rng);
            if (rng.Next() < 0.5) n = prefixes.KnightRandom(rng) + n;

            n =  n + suffixes.KnightRandom(rng);
            n = char.ToUpper(n[0]) + n.Substring(1);

            return n;
        }

        public static Dictionary<int, HerbCard> GetCatalogue(State s)
        {
            Dictionary<int, HerbCard> catalogue = null;
            MainManifest.KokoroApi.TryGetExtensionData<Dictionary<int, HerbCard>>(s, HerbCard.CATALOGUED_HERBS_KEY, out catalogue);

            if (catalogue == null)
            {
                MainManifest.KokoroApi.SetExtensionData<Dictionary<int, HerbCard>>(s, HerbCard.CATALOGUED_HERBS_KEY, new());
                return MainManifest.KokoroApi.GetExtensionData<Dictionary<int, HerbCard>>(s, HerbCard.CATALOGUED_HERBS_KEY);
            }
            return catalogue;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetFullDisplayName))]
        public static void GetFullDisplayNamePatch(Card __instance, ref string __result) 
        {
            if (__instance is not HerbCard herb) return;
            __result = herb.name;
        }
        public override void AfterWasPlayed(State state, Combat c) { revealed = true; }
        
        public static CardAction ParseSerializedAction(HerbActions serialized, int count = 1) 
        {
            switch (serialized)
            {
                case HerbActions.OXIDATION:         return new AStatus() { status = (Status)MainManifest.KokoroApi.OxidationStatus.Id, statusAmount =  count, targetPlayer = true };
                case HerbActions.NEGATIVE_OXIDATION:return new AStatus() { status = (Status)MainManifest.KokoroApi.OxidationStatus.Id, statusAmount = -count, targetPlayer = true };
                case HerbActions.TEMPSHIELD:        return new AStatus() { status = Enum.Parse<Status>("tempShield"),                  statusAmount =  count, targetPlayer = true };
                case HerbActions.OVERDRIVE:         return new AStatus() { status = (Status)MainManifest.statuses["herberdrive"].Id,   statusAmount =  count, targetPlayer = true };
                case HerbActions.DAZED:             return new AStatus() { status = (Status)MainManifest.statuses["dazed"].Id,         statusAmount =  count, targetPlayer = true };
                case HerbActions.BLINDNESS:         return new AStatus() { status = (Status)MainManifest.statuses["blindness"].Id,     statusAmount =  count, targetPlayer = true };
                case HerbActions.STUNCHARGE:        return new AStatus() { status = Enum.Parse<Status>("stunCharge"),                  statusAmount =  count, targetPlayer = true };
                case HerbActions.AUTODODGE_RIGHT:   return new AStatus() { status = Enum.Parse<Status>("autododgeRight"),              statusAmount =  count, targetPlayer = true };
                case HerbActions.REMOVE_CORRODE:    return new AStatus() { status = Enum.Parse<Status>("corrode"),                     statusAmount = -count, targetPlayer = true };
                case HerbActions.SHIELD:            return new AStatus() { status = Enum.Parse<Status>("shield"),                      statusAmount =  count, targetPlayer = true };
                case HerbActions.NEGATIVE_OVERDIVE: return new AStatus() { status = Enum.Parse<Status>("overdrive"),                   statusAmount = -count, targetPlayer = true };
                case HerbActions.EVADE:             return new AStatus() { status = Enum.Parse<Status>("evade"),                       statusAmount =  count, targetPlayer = true };
                case HerbActions.HERMESBOOTS:       return new AStatus() { status = Enum.Parse<Status>("hermes"),                      statusAmount =  count, targetPlayer = true };
                case HerbActions.HEAT:              return new AStatus() { status = Enum.Parse<Status>("heat"),                        statusAmount =  count, targetPlayer = true };
                case HerbActions.POWERDRIVE:        return new AStatus() { status = Enum.Parse<Status>("powerdrive"),                  statusAmount =  count, targetPlayer = true };
                case HerbActions.ENGINESTALL:       return new AStatus() { status = Enum.Parse<Status>("engineStall"),                 statusAmount =  count, targetPlayer = true };
                case HerbActions.ENGINELOCK:        return new AStatus() { status = Enum.Parse<Status>("lockdown"),                    statusAmount =  count, targetPlayer = true };
                case HerbActions.PAYBACK:           return new AStatus() { status = Enum.Parse<Status>("payback"),                     statusAmount =  count, targetPlayer = true };
                case HerbActions.FLUX:              return new AStatus() { status = (Status)MainManifest.statuses["tempSherb"].Id,     statusAmount =  count, targetPlayer = true };
                case HerbActions.PARANOIA:          return new AStatus() { status = (Status)MainManifest.statuses["paranoia"].Id,      statusAmount =  count, targetPlayer = true };

                case HerbActions.INSTANTMOVE_LEFT:  return new AMove() { dir = -count, targetPlayer = true };
                case HerbActions.INSTANTMOVE_RIGHT: return new AMove() { dir =  count, targetPlayer = true };
                case HerbActions.INSTANTMOVE_RANDOM:return new AMove() { dir =  count, isRandom=true, targetPlayer = true };

                case HerbActions.HEAL:              return new AHeal() { healAmount = count, targetPlayer = true };
                case HerbActions.HULLDAMAGE:        return new AHurt() { hurtAmount = count, targetPlayer = true };
            }

            throw new Exception("Unknown herb action passed: " + serialized);
        }

        public static string GetName(HerbActions serialized)
        {
            switch (serialized)
            {
                case HerbActions.OXIDATION:         return "Oxidation";
                case HerbActions.NEGATIVE_OXIDATION:return "Reduce Oxidation";
                case HerbActions.TEMPSHIELD:        return "Temp Shield";
                case HerbActions.OVERDRIVE:         return "Herberdrive";
                case HerbActions.DAZED:             return "Dazed";
                case HerbActions.BLINDNESS:         return "Blindness";
                case HerbActions.STUNCHARGE:        return "Stun Charge";
                case HerbActions.AUTODODGE_RIGHT:   return "Autododge Right";
                case HerbActions.REMOVE_CORRODE:    return "Reduce Corrode";
                case HerbActions.SHIELD:            return "Shield";
                case HerbActions.NEGATIVE_OVERDIVE: return "Reduce Overdrive";
                case HerbActions.EVADE:             return "Evade";
                case HerbActions.HERMESBOOTS:       return "Hermes Boots";
                case HerbActions.HEAT:              return "Heat";
                case HerbActions.POWERDRIVE:        return "Powerdrive";
                case HerbActions.ENGINESTALL:       return "Engine Stall";
                case HerbActions.ENGINELOCK:        return "Engine Lock";
                case HerbActions.PAYBACK:           return "Payback";
                case HerbActions.FLUX:              return "Temp Sherb";
                case HerbActions.PARANOIA:          return "Paranoia";

                case HerbActions.INSTANTMOVE_LEFT:  return "Instant Move";
                case HerbActions.INSTANTMOVE_RIGHT: return "Instant Move";
                case HerbActions.INSTANTMOVE_RANDOM:return "Random Instant Move";

                case HerbActions.HEAL:              return "Heal";
                case HerbActions.HULLDAMAGE:        return "Hull Damage";
            }

            throw new Exception("Unknown herb action passed: " + serialized);
        }

        public static Dictionary<HerbActions, int> CountSerializedActions(List<HerbActions> SerializedActions)
        {
            Dictionary<HerbActions, int> actionCounts = new();
            foreach (var serializedAction in SerializedActions)
            {
                if (!actionCounts.ContainsKey(serializedAction)) actionCounts[serializedAction] = 0;
                actionCounts[serializedAction]++;
            }
            return actionCounts;
        }

        public static List<CardAction> ParseSerializedActions(List<HerbActions> SerializedActions)
        {
            Dictionary<HerbActions, int> actionCounts = CountSerializedActions(SerializedActions);
            return actionCounts.Keys.Select(ha => ParseSerializedAction(ha, actionCounts[ha])).ToList();
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> herbActions = ParseSerializedActions(SerializedActions);
            List<CardAction> actions = new();
            
            bool singleUseOverriden = this.singleUseOverride.HasValue && this.singleUseOverride.Value == false;
            if (!singleUseOverriden) actions.Insert(0, new ACompletelyRemoveCard() { uuid = this.uuid, skipHandCheck = true });
            else                     actions.Insert(0, new ASendSelectedCardToDiscard() { selectedCard = this });
            
            actions.AddRange(herbActions);
            actions.Add(new ADummyAction());
            
            if (isDuringTryPlay && !revealed)
            {
                revealed = true;
                //actions.Add(new AAddCard() { destination = CardDestination.Hand, card = this }); // don't single use remove this card just yet
                actions.Insert(0, new ADisplayCard() { uuid = this.uuid });
                actions.Add(new ADummyAction());
            }

            return actions;
        }

        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 0,
                description = revealed ? null : $"Reveal in {revealTimer} more draws, or on play/apply to enemy",
                art = revealed ? null : Enum.Parse<Spr>("cards_OwnerMissing"),
                singleUse = !isDuringTryPlay,
            };
        }
        public override void OnDraw(State s, Combat c)
        {
            revealTimer--;
            if (revealTimer < 0) revealed = true;
        }

        public void OnExhausted(State s, Combat c)
        {
            //revealed = true;
            //var actions = this.GetActionsOverridden(s, c);
            //foreach (var action in actions)
            //{
            //    if (action is AStatus astatus) astatus.targetPlayer = !astatus.targetPlayer;
            //    if (action is AMove amove) amove.targetPlayer = !amove.targetPlayer;
            //    if (action is AHeal aheal) aheal.targetPlayer = !aheal.targetPlayer;
            //    if (action is AHurt ahurt) ahurt.targetPlayer = !ahurt.targetPlayer;

            //    c.Queue(Mutil.DeepCopy(action));
            //}
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetAllTooltips))]
        public static bool HarmonyPatch_HideTooltips(Card __instance, ref IEnumerable<Tooltip> __result, G g, State s, bool showCardTraits = true)
        {
            if (__instance is not HerbCard herb) return true;
            if (herb.revealed) return true;

            var tooltips = new List<Tooltip>()
            {
                new TTText() { text = $"The actions on this card will be permanently revealed after playing, exhausting, or combining." },
                new TTDivider(),
                new TTText() { text = $"This herb card has 2-3 of the following actions, with potential repeats: " }
            };

            foreach (HerbActions a in herb.PotentialActions)
            {
                TTGlossary ttg = HerbCard.ParseSerializedAction(a).GetTooltips(s).First() as TTGlossary;
                //tooltips.Add(new TTGlossaryNoDesc(ttg.key, ttg.vals));
                string name = HerbCard.GetName(a);
                string suffix = name.StartsWith("Reduce") ? ".reduce" : "";
                tooltips.Add(new TTGlossaryNoDescNameOverride(ttg.key + suffix, name, ttg.vals));
            }
            __result = tooltips;




            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void AfterPlayIfHerb(Combat __instance, bool __result, State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            if (__result && card is HerbCard herb)// && herb.IsRaw)
            {
                // record that this herb existed
                if (!herb.GetDataWithOverrides(s).temporary)
                {
                    var cataloguedHerbs = GetCatalogue(s);
                    cataloguedHerbs[herb.uuid] = herb;
                    MainManifest.KokoroApi.SetExtensionData<Dictionary<int, HerbCard>>(s, CATALOGUED_HERBS_KEY, cataloguedHerbs);
                }

                // herb bag
                bool hasHerbBag = s.EnumerateAllArtifacts().Where(a => a is HerbBag).Any();
                if (hasHerbBag) __instance.Queue(new ADrawCard() { count = 1 });
            }
        }

        private static bool isDuringTryPlay = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void TrackPlaying(State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            isDuringTryPlay = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void StopTrackingPlaying(State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            isDuringTryPlay = false;
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new Upgrade[0], dontOffer = true, unreleased = true)]
    public class HerbCard_Leaf : HerbCard
    {
        public static List<HerbActions> options => new()
        {
            HerbActions.FLUX,
            HerbActions.OVERDRIVE,
            HerbActions.DAZED,
            //HerbActions.BLINDNESS,
            HerbActions.PARANOIA,
            HerbActions.STUNCHARGE,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
        };
        protected override List<HerbActions> possibleOptions => options;
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Leaf"; }
        public override bool IsRaw => true;
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new Upgrade[0], dontOffer = true, unreleased = true)]
    public class HerbCard_Bark : HerbCard
    {
        public static List<HerbActions> options = new()
        {
            HerbActions.AUTODODGE_RIGHT,
            HerbActions.ENGINESTALL,
            HerbActions.TEMPSHIELD,
            HerbActions.TEMPSHIELD,
            HerbActions.NEGATIVE_OXIDATION,
            HerbActions.NEGATIVE_OXIDATION,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
        };
        protected override List<HerbActions> possibleOptions => options;
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Bark"; }
        public override bool IsRaw => true;
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new Upgrade[0], dontOffer = true, unreleased = true)]
    public class HerbCard_Root : HerbCard
    {
        public static List<HerbActions> options = new()
        {
            HerbActions.SHIELD,
            HerbActions.TEMPSHIELD,
            HerbActions.TEMPSHIELD,
            HerbActions.BLINDNESS,
            HerbActions.NEGATIVE_OVERDIVE,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
        };
        protected override List<HerbActions> possibleOptions => options;
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Root"; }
        public override bool IsRaw => true;
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new Upgrade[0], dontOffer = true, unreleased = true)]
    public class HerbCard_Seed : HerbCard
    {
        public static List<HerbActions> options = new()
        {
            HerbActions.EVADE,
            HerbActions.EVADE,
            HerbActions.EVADE,
            HerbActions.HERMESBOOTS,
            HerbActions.INSTANTMOVE_LEFT,
            HerbActions.INSTANTMOVE_RIGHT,
            HerbActions.INSTANTMOVE_RIGHT,
            HerbActions.DAZED,
            HerbActions.DAZED,
            HerbActions.INSTANTMOVE_RANDOM,
            HerbActions.INSTANTMOVE_RANDOM,
            HerbActions.INSTANTMOVE_RANDOM,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
        };
        protected override List<HerbActions> possibleOptions => options;
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Seed"; }
        public override bool IsRaw => true;
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new Upgrade[0], dontOffer = true, unreleased = true)]
    public class HerbCard_Shroom : HerbCard
    {
        public static List<HerbActions> options = new()
        {
            HerbActions.HEAL,
            HerbActions.HULLDAMAGE,
            HerbActions.HEAT,
            HerbActions.POWERDRIVE,
            HerbActions.BLINDNESS,
            HerbActions.PARANOIA,
            HerbActions.PAYBACK,
            HerbActions.ENGINELOCK,
        };
        protected override List<HerbActions> possibleOptions => options;
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            // shrooms can only generate 1 positive effect max
            HashSet<HerbActions> groupCappedToOne = new() { HerbActions.HEAL, HerbActions.POWERDRIVE, HerbActions.PAYBACK };

            Rand rng = s.rngActions;
            int count = 3; // rng.Next() < 0.5 ? 3 : 4;
            List<HerbActions> workingOptions = new(options);
            List<HerbActions> retval = new();
            for(int i = 0; i < count; i++)
            {
                var action = workingOptions.KnightRandom(rng);
                retval.Add(action);
                if (groupCappedToOne.Contains(action))
                {
                    workingOptions = workingOptions.Where(a => !groupCappedToOne.Contains(a)).ToList();
                }
            }

            return retval;
        }

        protected override string GetTypeName() { return "Shroom"; }
        public override bool IsRaw => true;
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new Upgrade[0], dontOffer = true, unreleased = true)]
    public class HerbCard_Poultice : HerbCard
    {
        public HerbCard_Poultice() { name = ""; }
        protected override string GetTypeName() { return "Poultice"; }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new Upgrade[0], dontOffer = true, unreleased = true)]
    public class HerbCard_Tea : HerbCard
    {
        public HerbCard_Tea() { name = ""; }
        protected override string GetTypeName() { return "Tea"; }
    }
}
