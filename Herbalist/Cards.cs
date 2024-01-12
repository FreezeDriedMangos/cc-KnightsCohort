using HarmonyLib;
using KnightsCohort.actions;
using KnightsCohort.Knight;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Herbalist.Cards
{

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class MortarAndPestle : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ACombineHerbs() { amount = 2, selected = new() { new HerbCard() { SerializedActions = new() { HerbActions.OXIDATION } } } },
               new ATooltipDummy() { tooltips = new(), icons = new() { new Icon((Spr)MainManifest.sprites["icons/herb_bundle_add_oxidize"].Id, 1, Colors.textMain) } }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Smolder : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                    browseAction = new AExhaustSelectedCard()
                },
                new ATooltipDummy()
                {
                    tooltips = new() {},
                    icons = new() 
                    {
                        // todo: change to icons/herb_search
                        new Icon((Spr)MainManifest.sprites["icons/herb_bundle"].Id, null, Colors.textMain),
                        new Icon(Enum.Parse<Spr>("icons_exhaust"), null, Colors.textMain),
                    }
                },
                new ADummyAction(),
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [HarmonyPatch]
    public class HerbPack : Card
    {
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TTGlossary), nameof(TTGlossary.BuildIconAndText))]
        public static void BuildIconAndText(TTGlossary __instance, ref (Spr? icon, string text) __result)
        {
            if (__instance is TTGlossaryNoDesc)
            {
                string[] keyparts = __instance.key.Split('.');
                string namecolor = keyparts[0];
                __result.text = $"<c={namecolor}>{Loc.T(namecolor + "." + keyparts[1] + ".name").ToUpperInvariant()}</c>";
            }
            
            if (__instance is TTGlossaryNoDescNameOverride ttgndno)
            {
                string[] keyparts = __instance.key.Split('.');
                string namecolor = keyparts[0];
                __result.text = $"<c={namecolor}>{ttgndno.nameOverride}</c>";
            }
        }

        public class TTGlossaryNoDesc : TTGlossary { public TTGlossaryNoDesc(string key, params object[]? vals) : base(key, vals) { } }
        public class TTGlossaryNoDescNameOverride : TTGlossary { public string nameOverride; public TTGlossaryNoDescNameOverride(string key, string name, params object[]? vals) : base(key, vals) { nameOverride = name; } }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (isDuringTryPlay)
            {
                return new()
                {
                   new AAddCard() { card = GenerateHerb(s) },
                   new AAddCard() { card = GenerateHerb(s) },
                   new AAddCard() { card = GenerateHerb(s) },
                   new AAddCard() { card = GenerateHerb(s) },
                };
            }
            else
            {
                List<Tooltip> tooltips = new()
                {
                    new TTText() { text = $"The randomly generated herb cards will have 2-3 of the following actions, with potential repeats: " }
                };

                foreach (HerbActions a in PossibleActions())
                {
                    TTGlossary ttg = HerbCard.ParseSerializedAction(a).GetTooltips(s).First() as TTGlossary;
                    //tooltips.Add(new TTGlossaryNoDesc(ttg.key, ttg.vals));
                    string name = HerbCard.GetName(a);
                    string suffix = name.StartsWith("Reduce") ? ".reduce" : "";
                    tooltips.Add(new TTGlossaryNoDescNameOverride(ttg.key + suffix, name, ttg.vals));
                    //MainManifest.Instance.Logger.LogInformation($"Adding tooltip on {GetHerbType()} for {a} : {HerbCard.GetName(a)}");
                }
                tooltips.Add(new TTDivider());

                return new()
                {
                    new ATooltipDummy() { icons = new() { new Icon(Enum.Parse<Spr>("icons_addCard"), null, Colors.textMain) } },
                    new ATooltipDummy() { icons = new() { new Icon(Enum.Parse<Spr>("icons_addCard"), null, Colors.textMain) } },
                    new ATooltipDummy() { icons = new() { new Icon(Enum.Parse<Spr>("icons_addCard"), null, Colors.textMain) } },
                    new ATooltipDummy() { icons = new() { new Icon(Enum.Parse<Spr>("icons_addCard"), null, Colors.textMain) },
                        tooltips = tooltips
                    }
                };
            }
        }

        public override CardData GetData(State state)
        {
            return new() { cost = 0, description = $"Permanently add 4 random {GetHerbType()} herb cards to your deck.", singleUse = true };
        }

        public virtual string GetHerbType() => "NULL";
        public virtual HerbCard GenerateHerb(State s) => new HerbCard();
        public virtual List<HerbActions> PossibleActions() => new();
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LeafPack : HerbPack
    {
        public override string GetHerbType() => "leaf";
        public override HerbCard GenerateHerb(State s) => HerbCard.Generate(s, new HerbCard_Leaf());
        public override List<HerbActions> PossibleActions() => HerbCard_Leaf.options;
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BarkPack : HerbPack
    {
        public override string GetHerbType() => "bark";
        public override HerbCard GenerateHerb(State s) => HerbCard.Generate(s, new HerbCard_Bark());
        public override List<HerbActions> PossibleActions() => HerbCard_Bark.options;
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SeedPack : HerbPack
    {
        public override string GetHerbType() => "seed";
        public override HerbCard GenerateHerb(State s) => HerbCard.Generate(s, new HerbCard_Seed());
        public override List<HerbActions> PossibleActions() => HerbCard_Seed.options;
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RootPack : HerbPack
    {
        public override string GetHerbType() => "root";
        public override HerbCard GenerateHerb(State s) => HerbCard.Generate(s, new HerbCard_Root());
        public override List<HerbActions> PossibleActions() => HerbCard_Root.options;
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ShroomPack : HerbPack
    {
        public override string GetHerbType() => "shroom";
        public override HerbCard GenerateHerb(State s) => HerbCard.Generate(s, new HerbCard_Shroom());
        public override List<HerbActions> PossibleActions() => HerbCard_Shroom.options;
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class PublishFindings : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = new()
                        {
                            new ARemoveSelectedCardFromWhereverItIs(),
                            new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true }
                        }
                    }
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Permanently remove 1 herb card from your deck. Gain 1 honor." };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Forage : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AAddCard() { card = Util.GenerateRandomHerbCard(s) }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Permanently gain a random herb card." };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Consume : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = new()
                        {
                            new AQueueImmediateSelectedCardActions(),
                            new AQueueImmediateSelectedCardActions(),
                            new ARemoveSelectedCardFromWhereverItIs()
                        }
                    }
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Permanently remove 1 herb card from your deck. Play its effects twice." };
        }
    }

    // TODO: overlay art (like abyssal visions)
    [HarmonyPatch]
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class QUEST : Card
    {
        public List<HerbActions> requirements;
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (requirements == null)
            {
                requirements = new List<HerbActions>();
                requirements.AddRange(Util.GenerateRandomHerbCard(s).SerializedActions);
                requirements.AddRange(Util.GenerateRandomHerbCard(s).SerializedActions);
            }

            var uuid = this.uuid;
            List<CardAction> actions = new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                    browseAction = new AEvaluateQuestSubmission()
                    {
                        requirements = requirements,
                        uuid = uuid
                    }
                }
            };

            var mockActions = HerbCard.ParseSerializedActions(requirements);
            foreach(var action in mockActions)
            {
                actions.Add(ATooltipDummy.BuildStandIn(action, s));
            }
            actions.Add(new ADummyAction());

            return actions;
        }

        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetShakeOffset))]
        public static void WiggleWiggle(Card __instance, ref Vec __result, G g)
        {
            if (__instance is not QUEST) return;

            double num = Mutil.Rand(Math.Round(g.state.time * 18.0) + __instance.seed);
            if (num < 0.1)
            {
                __result = __result + (new Vec(Mutil.Rand(num) - 0.5, Mutil.Rand(num + 0.1) - 0.5) * 3.0).round();
            }
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class QuestReward : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = 1 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true)]
    public class EpicQuestReward : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = 2 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true)]
    public class Compassion : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost
                    (
                        MainManifest.KokoroApi.ActionCosts.StatusResource
                        (
                            Enum.Parse<Status>("corrode"),
                            Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.EnemyWithOutgoingArrow,
                            (Spr)MainManifest.sprites["icons/CorrodeCostUnsatisfied"].Id,
                            (Spr)MainManifest.sprites["icons/CorrodeCostSatisfied"].Id
                        ),
                        amount: 1
                    ),
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = 2 }
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CallOnDebts : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Guid continueId;

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost
                    (
                        MainManifest.KokoroApi.ActionCosts.StatusResource
                        (
                            (Status)MainManifest.statuses["honor"].Id,
                            Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                            (Spr)MainManifest.sprites["icons/honor_cost_unsatisfied"].Id,
                            (Spr)MainManifest.sprites["icons/honor_cost"].Id
                        ),
                        amount: 1
                    ),
                    MainManifest.KokoroApi.Actions.MakeContinue(out continueId)
                ),
                MainManifest.KokoroApi.Actions.MakeContinued(continueId, new AStatus() { status = Enum.Parse<Status>("corrode"), targetPlayer = true, statusAmount = -1 }),
                MainManifest.KokoroApi.Actions.MakeContinued(continueId, new AStatus() { status = (Status)MainManifest.KokoroApi.OxidationStatus.Id, targetPlayer = true, statusAmount = -7 })
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BrewTea : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                    browseAction = new ABrewTea()
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Select an herb in hand. Increase every effect by 1 and then remove one at random." };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FireAndSmoke : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = new()
                        {
                            new AExhaustSelectedCard(),
                            new ARemoveSelectedCardFromWhereverItIs(),
                            new ASendSelectedCardToHand(),
                            new AExhaustSelectedCard(),
                        }
                    }
                },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Select an herb in hand. Exhaust it twice." };
        }
    }
}
