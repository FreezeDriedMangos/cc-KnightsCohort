using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        NOSHIELD,
        HEAT,
        POWERDRIVE,
        NEGATIVE_OXIDATION
    }

    [HarmonyPatch]
    public class HerbCard : Card
    {
        protected virtual List<HerbActions> GenerateSerializedActions(State s) { return new(); }
        protected virtual string GetTypeName() { return "INVALID"; }

        public List<HerbActions> SerializedActions = new();
        public string name;
        public bool revealed;
        public HerbCard() { }

        public static HerbCard Generate(State s, HerbCard c)
        {
            c.SerializedActions = c.GenerateSerializedActions(s);
            c.name = GenerateFakeLatinWord(s) + " " + c.GetTypeName();
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
                case HerbActions.OVERDRIVE:         return new AStatus() { status = Enum.Parse<Status>("overdrive"),                   statusAmount =  count, targetPlayer = true };
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
                case HerbActions.NOSHIELD:          return new AStatus() { status = Enum.Parse<Status>("shield"),                      statusAmount = 0,      targetPlayer = true, mode = Enum.Parse<AStatusMode>("Set") };

                case HerbActions.INSTANTMOVE_LEFT:  return new AMove() { dir = -count, targetPlayer = true };
                case HerbActions.INSTANTMOVE_RIGHT: return new AMove() { dir =  count, targetPlayer = true };

                case HerbActions.HEAL:              return new AHeal() { healAmount = count, targetPlayer = true };
                case HerbActions.HULLDAMAGE:        return new AHurt() { hurtAmount = count, targetPlayer = true };
            }

            throw new Exception("Unknown herb action passed: " + serialized);
        }

        public static List<CardAction> ParseSerializedActions(List<HerbActions> SerializedActions)
        {
            Dictionary<HerbActions, int> actionCounts = new();
            foreach (var serializedAction in SerializedActions)
            {
                if (!actionCounts.ContainsKey(serializedAction)) actionCounts[serializedAction] = 0;
                actionCounts[serializedAction]++;
            }
            return actionCounts.Keys.Select(ha => ParseSerializedAction(ha, actionCounts[ha])).ToList();
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            return ParseSerializedActions(SerializedActions);
        }

        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 0,
                description = revealed ? null : " ",
                art = revealed ? null : Enum.Parse<Spr>("cards_OwnerMissing"),
            };
        }

        public void OnExhausted(State s, Combat c)
        {
            revealed = true;
            var actions = this.GetActionsOverridden(s, c);
            foreach (var action in actions)
            {
                if (action is AStatus astatus) astatus.targetPlayer = !astatus.targetPlayer;
                if (action is AMove amove) amove.targetPlayer = !amove.targetPlayer;
                if (action is AHeal aheal) aheal.targetPlayer = !aheal.targetPlayer;
                if (action is AHurt ahurt) ahurt.targetPlayer = !ahurt.targetPlayer;

                c.Queue(Mutil.DeepCopy(action));
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetAllTooltips))]
        public static bool HarmonyPatch_HideTooltips(Card __instance, ref IEnumerable<Tooltip> __result, G g, State s, bool showCardTraits = true)
        {
            if (__instance is not HerbCard herb) return true;
            if (herb.revealed) return true;

            // TODO: add a tooltip for what herb cards do - the whole "actions reveal on play and actions apply to opponent on exhaust" deal
            __result = new List<Tooltip>();

            return false;
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new Upgrade[0], dontOffer = true)]
    public class HerbCard_Leaf : HerbCard
    {
        static List<HerbActions> options = new()
        {
            HerbActions.TEMPSHIELD,
            HerbActions.OVERDRIVE,
            HerbActions.DAZED,
            HerbActions.BLINDNESS,
            HerbActions.NEGATIVE_OXIDATION,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
        };
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Leaf"; }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new Upgrade[0], dontOffer = true)]
    public class HerbCard_Bark : HerbCard
    {
        static List<HerbActions> options = new()
        {
            HerbActions.STUNCHARGE,
            HerbActions.AUTODODGE_RIGHT,
            HerbActions.DAZED,
            HerbActions.REMOVE_CORRODE,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
        };
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Bark"; }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new Upgrade[0], dontOffer = true)]
    public class HerbCard_Root : HerbCard
    {
        static List<HerbActions> options = new()
        {
            HerbActions.SHIELD,
            HerbActions.SHIELD,
            HerbActions.TEMPSHIELD,
            HerbActions.TEMPSHIELD,
            HerbActions.TEMPSHIELD,
            HerbActions.BLINDNESS,
            HerbActions.NEGATIVE_OVERDIVE,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
        };
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Root"; }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new Upgrade[0], dontOffer = true)]
    public class HerbCard_Seed : HerbCard
    {
        static List<HerbActions> options = new()
        {
            HerbActions.EVADE,
            HerbActions.EVADE,
            HerbActions.EVADE,
            HerbActions.EVADE,
            HerbActions.HERMESBOOTS,
            HerbActions.INSTANTMOVE_LEFT,
            HerbActions.INSTANTMOVE_RIGHT,
            HerbActions.INSTANTMOVE_RIGHT,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
            HerbActions.OXIDATION,
        };
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Seed"; }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new Upgrade[0], dontOffer = true)]
    public class HerbCard_Shroom : HerbCard
    {
        static List<HerbActions> options = new()
        {
            HerbActions.HEAL,
            HerbActions.HULLDAMAGE,
            HerbActions.NOSHIELD,
            HerbActions.HEAT,
            HerbActions.POWERDRIVE,
            HerbActions.BLINDNESS,
        };
        protected override List<HerbActions> GenerateSerializedActions(State s)
        {
            Rand rng = s.rngActions;
            if (rng.Next() < 0.5) return new() { options.KnightRandom(rng), options.KnightRandom(rng), };
            else return new() { options.KnightRandom(rng), options.KnightRandom(rng), options.KnightRandom(rng), };
        }

        protected override string GetTypeName() { return "Shroom"; }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new Upgrade[0], dontOffer = true)]
    public class HerbCard_Poultice : HerbCard
    {
        public HerbCard_Poultice() { name = ""; }
        protected override string GetTypeName() { return "Poultice"; }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new Upgrade[0], dontOffer = true)]
    public class HerbCard_Tea : HerbCard
    {
        public HerbCard_Tea() { name = ""; }
        protected override string GetTypeName() { return "Tea"; }
    }
}
