using HarmonyLib;
using KnightsCohort.actions;
using KnightsCohort.Knight;
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

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LeafPack : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Leaf()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Leaf()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Leaf()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Leaf()) },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, description = "Permanently add 4 random leaf herb cards to your deck.", singleUse = true };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BarkPack : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Bark()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Bark()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Bark()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Bark()) },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, description = "Permanently add 4 random bark herb cards to your deck.", singleUse = true };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SeedPack : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Seed()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Seed()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Seed()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Seed()) },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, description = "Permanently add 4 random seed herb cards to your deck.", singleUse = true };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RootPack : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Root()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Root()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Root()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Root()) },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, description = "Permanently add 4 random root herb cards to your deck.", singleUse = true };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ShroomPack : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Shroom()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Shroom()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Shroom()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Shroom()) },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, description = "Permanently add 4 random shroom herb cards to your deck.", singleUse = true };
        }
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
                    browseAction = new ADelegateAction() { onBegin = (instance, g, s, c) =>
                    {
                        if (instance.selectedCard == null) return;
                        s.RemoveCardFromWhereverItIs(instance.selectedCard.uuid);
                        c.QueueImmediate(new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true });
                    }}
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
                    browseAction = new ADelegateAction() { onBegin = (instance, g, s, c) =>
                    {
                        if (instance.selectedCard == null) return;
                        c.Queue(instance.selectedCard.GetActionsOverridden(s, c));
                        c.Queue(instance.selectedCard.GetActionsOverridden(s, c));
                        s.RemoveCardFromWhereverItIs(instance.selectedCard.uuid);
                    }}
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Permanently remove 1 herb card from your deck. Play its effects twice." };
        }
    }


    // TODO: wiggle (like abyssal visions)
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
                    browseAction = new ADelegateAction() { onBegin = (instance, g, s, c) =>
                    {
                        if (instance.selectedCard == null) return;
                        var goalActions = SetizeSerializedHerbActions(requirements);
                        var submittedActions = SetizeSerializedHerbActions((instance.selectedCard as HerbCard).SerializedActions);

                        bool allRequirementsMet = submittedActions.Fast_AllAreIn(goalActions);
                        bool requirementsPerfectlyMet = submittedActions.Count == goalActions.Count;

                        if (allRequirementsMet && requirementsPerfectlyMet) { c.QueueImmediate(new AAddCard() { card = new EpicQuestReward(), amount=1, destination = Enum.Parse<CardDestination>("Hand") }); }
                        else if (allRequirementsMet) { c.QueueImmediate(new AAddCard() { card = new QuestReward(), amount=1, destination = Enum.Parse<CardDestination>("Hand") }); }

                        s.RemoveCardFromWhereverItIs(uuid);
                    }}
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

        public static HashSet<string> SetizeSerializedHerbActions(List<HerbActions> ha)
        {
            Dictionary<HerbActions, int> counts = new();
            HashSet<string> retval = new HashSet<string>();
            foreach(HerbActions action in  ha)
            {
                counts.TryAdd(action, 0);
                counts[action]++;
                retval.Add(action + ":" + counts[action]);
            }
            return retval;
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
                            (Spr)MainManifest.sprites["icons/honor"].Id, // TODO: make an unsatisfied honor cost sprite
                            (Spr)MainManifest.sprites["icons/honor"].Id  // TODO: make a satisfied honor cost sprite (corrode drop with a minus on top of it)
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
                    browseAction = new ADelegateAction() { onBegin = (instance, g, s, c) =>
                    {
                        if (instance.selectedCard == null) return;
                        HerbCard herb = instance.selectedCard as HerbCard;
                        HashSet<HerbActions> actions = new HashSet<HerbActions>(herb.SerializedActions);
                        int removalIndex = (int)(s.rngActions.NextUint() % actions.Count);
                        actions.Remove(actions.ToList()[removalIndex]);

                        HerbCard_Tea tea = new HerbCard_Tea();
                        tea.SerializedActions = new(herb.SerializedActions);
                        tea.SerializedActions.AddRange(actions);
                        tea.name =  herb.name.Split(' ')[0] + " Tea";
                        tea.revealed = true;
                        c.QueueImmediate(new AAddCard() { card = tea, destination = Enum.Parse<CardDestination>("Hand")});
                    }}
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
                    browseAction = new ADelegateAction() { onBegin = (instance, g, s, c) =>
                    {
                        if (instance.selectedCard == null) return;
                        c.Queue(new AExhaustOtherCard() { uuid = instance.selectedCard.uuid });
                        c.Queue(new ADelegateAction() { onBegin = (instance2, g2, s2, c2) =>
                        {
                            s2.RemoveCardFromWhereverItIs(instance.selectedCard.uuid);
                            c2.SendCardToHand(s, instance.selectedCard);
                        }});
                        c.Queue(new AExhaustOtherCard() { uuid = instance.selectedCard.uuid });
                    }}
                },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Select an herb in hand. Exhaust it twice." };
        }
    }
}
