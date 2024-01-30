using HarmonyLib;
using KnightsCohort.actions;
using KnightsCohort.Knight;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xna.Framework.Input;
using Shockah.Dracula;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Herbalist.Cards
{

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class MortarAndPestle : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<Card> selected = upgrade == Upgrade.A ? new() : new() { new HerbCard() { SerializedActions = new() { HerbActions.OXIDATION } } };
            int amount = upgrade == Upgrade.B ? 3 : 2;

            List<Tooltip> tooltips = new() {
                new TTGlossary(MainManifest.glossary["herbsearch"].Head, amount, "Deck"), 
                new TTGlossary(MainManifest.glossary[upgrade == Upgrade.A ? "combineHerbs" : "combineHerbsToxic"].Head, 1), 
            };
            if (upgrade != Upgrade.A) tooltips.Add(MainManifest.KokoroApi.GetOxidationStatusTooltip(s, s.ship));

            return new()
            {
               new ACombineHerbs() { disabled = flipped, amount = amount, selected = selected },
               new ATooltipDummy() {
                   disabled = flipped,
                   tooltips = tooltips, 
                   icons = new() { new Icon((Spr)MainManifest.sprites["icons/herb_bundle"].Id, amount, flipped ? Colors.disabledText : Colors.textMain), new Icon((Spr)MainManifest.sprites[upgrade == Upgrade.A ? "icons/mortar_and_pestle" : "icons/mortar_and_pestle_toxic"].Id, upgrade == Upgrade.A ? null : 1, flipped ? Colors.disabledText : Colors.textMain) } 
               },

               new AHerbCardSelect()
               {
                   disabled = !flipped,
                   browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                   browseAction = new AQueueImmediateOtherActions()
                   {
                       actions = new() { new ARemoveSelectedCardFromWhereverItIs(), new ASendSelectedCardToHand() }
                   }
               },
               new ATooltipDummy() {
                   disabled = !flipped,
                   tooltips = new() { new TTGlossary(MainManifest.glossary["moveCard"].Head, Enum.Parse<CardBrowse.Source>("Deck"), Enum.Parse<CardBrowse.Source>("Hand")) },
                   icons = new() { new Icon((Spr)MainManifest.sprites["icons/herb_bundle"].Id, 1, flipped ? Colors.textMain : Colors.disabledText), new Icon((Spr)MainManifest.sprites["icons/move_card"].Id, null, Colors.textMain), new Icon(Enum.Parse<Spr>("icons_dest_hand"), null, Colors.textMain), }
               },

               new ADummyAction(),
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, floppable = true };
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
                    browseSource = Enum.Parse<CardBrowse.Source>(upgrade == Upgrade.B ? "Deck" : "Hand"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = new()
                        {
                            //new ARemoveSelectedCardFromWhereverItIs(),
                            //new ASendSelectedCardToDiscard(),
                            new AApplySelectedHerbToEnemy(),
                        }
                    }
                },
                //new ATooltipDummy()
                //{
                //    tooltips = new() { new TTGlossary(MainManifest.glossary["exhaustSelected"].Head), new TTGlossary(MainManifest.glossary["herbExhaust"].Head), },
                //    icons = new() 
                //    {
                //        // todo: change to icons/herb_search
                //        new Icon((Spr)MainManifest.sprites["icons/herb_bundle"].Id, null, Colors.textMain),
                //        new Icon((Spr)MainManifest.sprites["icons/exhaust_selected_card"].Id, null, Colors.textMain),
                //    }
                //},
                new ATooltipDummy() { 
                    tooltips = new (){ new TTGlossary(MainManifest.glossary["burnHerb"].Head) },
                    icons = new()
                    {
                        new Icon((Spr)MainManifest.sprites[upgrade == Upgrade.B ? "icons/herb_bundle" : "icons/herb_in_hand"].Id, 1, Colors.textMain),
                        new Icon((Spr)MainManifest.sprites["icons/burn_herb"].Id, null, Colors.textMain),
                        //new Icon(Enum.Parse<Spr>("icons_outgoing"), null, Colors.textMain),
                    }
                },
                new ADummyAction(),
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 2 : 1, retain = upgrade == Upgrade.A };
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

        public virtual int GetNumCardsAdded(State s)
        {
            return upgrade switch
            {
                Upgrade.A => 2,
                Upgrade.B => 6,
                _ => 4
            };
        }

        public virtual bool GetExhausts(State s) => upgrade == Upgrade.A;
        public virtual bool GetSingleUse(State s) => upgrade != Upgrade.A;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            int numCards = GetNumCardsAdded(s);
            if (isDuringTryPlay)
            {
                List<CardAction> actions = new();
                for (int i = 0; i < numCards; i++) actions.Add(new AAddCard() { card = GenerateHerb(s) });
                return actions;
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


                List<CardAction> actions = new();
                for (int i = 0; i < numCards; i++) actions.Add(new ATooltipDummy() { icons = new() { new Icon(Enum.Parse<Spr>("icons_addCard"), null, Colors.textMain) } });
                if (actions.Count > 0) (actions[0] as ATooltipDummy).tooltips = tooltips;
                return actions;
            }
        }

        public override CardData GetData(State state)
        {
            return new() { cost = 0, description = $"Permanently add {GetNumCardsAdded(state)} random {GetHerbType()} herb cards to your deck.", singleUse = GetSingleUse(state), exhaust = GetExhausts(state) };
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
            List<CardAction> retval = new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = new()
                        {
                            new ARemoveSelectedCardFromWhereverItIs(),
                            new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = upgrade == Upgrade.None ? 3 : 5, targetPlayer = true }
                        }
                    }
                }
            };

            if (upgrade == Upgrade.B)
            {
                retval.Add(new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = new()
                        {
                            new ARemoveSelectedCardFromWhereverItIs(),
                            new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = upgrade == Upgrade.None ? 1 : 2, targetPlayer = true }
                        }
                    }
                });
            }

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Permanently remove 1 herb card from your deck. Gain 1 honor." };
        }
    }

    [HarmonyPatch]
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Forage : Card
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

        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (!isDuringTryPlay) { return new(); } // don't run the rng if it's not needed

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                    new AAddCard() { card = Util.GenerateRandomHerbCard(s) },
                    new AAddCard() { card = Util.GenerateRandomHerbCard(s) },
                    new AAddCard() { card = Util.GenerateRandomHerbCard(s) }
                };
            }

            return new()
            {
                new AAddCard() { card = Util.GenerateRandomHerbCard(s) }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { 
                cost = 1, 
                exhaust = upgrade == Upgrade.B, 
                recycle = upgrade == Upgrade.A,
                description = upgrade == Upgrade.B 
                    ? "Permanently gain 3 random herb cards."
                    : "Permanently gain a random herb card." 
            };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Consume : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> onSelectActions = new()
            {
                new AQueueImmediateSelectedCardActions(),
                new AQueueImmediateSelectedCardActions(),
                new ARemoveSelectedCardFromWhereverItIs()
            };

            if (upgrade == Upgrade.A)
            {
                onSelectActions.Insert(0, new AQueueImmediateSelectedCardActions());
            }

            List<CardAction> retval = new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = onSelectActions
                    }
                }
            };

            if (upgrade == Upgrade.B)
            {
                retval.Add(new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = onSelectActions
                    }
                });
            }

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = upgrade switch
            {
                Upgrade.None => "Permanently remove 1 herb card from your deck. Play its effects twice." ,
                Upgrade.A => "Permanently remove 1 herb card from your deck. Play its effects three times." ,
                Upgrade.B => "Permanently remove 2 herb cards from your deck. Play their effects twice." ,
            }};
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
                new ATooltipDummy() { tooltips = new()
                {
                    new TTText() { text = upgrade switch {
                        Upgrade.None => "On play, select an herb card in your deck. If this herb card has every action shown on this QUEST card, add a <c=card>Quest Reward</c> to your deck permanently. If the submitted card has no extra actions, gain an <c=card>Epic Quest Reward</c> instead.",
                        Upgrade.A    => "On play, select an herb card in your deck, and then remove a requirement of your choice from this QUEST. If the selected herb card has every action shown on this QUEST card (minus the one you chose to remove), add a <c=card>Quest Reward</c> to your deck permanently. If the submitted card has no extra actions, gain an <c=card>Epic Quest Reward</c> instead.",
                        Upgrade.B    => "On play, select an herb card in your deck. If this herb card has every action shown on this QUEST card, add an <c=card>Epic Quest Reward</c> to your deck permanently.",
                    }},
                    new TTText() { text = "The selected herb card will be removed from your deck PERMANENTLY."},
                    new TTCard() { card = new QuestReward() },
                    new TTCard() { card = new EpicQuestReward() },
                }},
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                    browseAction = upgrade == Upgrade.A 
                        ? new AEvaluateQuestSubmissionWithRequirementRemoval() { requirements = requirements, uuid = uuid }
                        : new AEvaluateQuestSubmission() { supersetCountsAsPerfect = upgrade == Upgrade.B, requirements = requirements, uuid = uuid }
                }
            };

            var mockActions = HerbCard.ParseSerializedActions(requirements);
            foreach(var action in mockActions)
            {
                actions.Add(ATooltipDummy.BuildStandIn(action, s));
            }
            actions.Add(new ADummyAction());
            if (upgrade != Upgrade.A) actions.Add(new ADummyAction());

            if (upgrade == Upgrade.A) actions.Add(new ADummyAction());

            return actions;
        }

        public override CardData GetData(State state)
        {
            // TODO: why does this not detect when I'm in the upgrade screen???
            //if (state.route is CardBrowse b)// && b.mode == CardBrowse.Mode.UpgradeCard)
            if (state.route is CardReward c && c.ugpradePreview is not null)
            {
                switch (upgrade)
                {
                    case Upgrade.A: return new() { cost = 0, singleUse = true, description = "Remove one requirement of your choice on submission" };
                    case Upgrade.B: return new() { cost = 0, singleUse = true, description = "Gain Epic Quest Reward even if your submission doesn't perfectly match the quest requirements" };
                }
            }
            return new() { cost = 0, singleUse = true };
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

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true)]
    public class QuestReward : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = 4 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = false };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true)]
    public class EpicQuestReward : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = 4 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, exhaust = false };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true)]
    public class Compassion : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.B)
            {
                return new()
                {
                    new ATooltipDummy() { icons = new() {
                        new Icon(Enum.Parse<Spr>("icons_x"), null, Colors.textMain),
                        new Icon((Spr)MainManifest.sprites["icons/equal_sign"].Id, null, Colors.textMain),
                        new Icon(Enum.Parse<Spr>("icons_outgoing"), null, Colors.textMain),
                        new Icon(Enum.Parse<Spr>("icons_corrode"), null, Colors.textMain),
                    } },
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = c.otherShip.Get(Enum.Parse<Status>("corrode")), xHint = 1 },
                    new AStatus() { status = Enum.Parse<Status>("corrode"), statusAmount = 0, mode = AStatusMode.Set, targetPlayer = false }
                };
            }

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
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 3 : 2 }
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
            if (upgrade == Upgrade.A)
            {
                var spendCorrodeAction = MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost
                    (
                        MainManifest.KokoroApi.ActionCosts.StatusResource
                        (
                            Enum.Parse<Status>("corrode"),
                            Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                            (Spr)MainManifest.sprites["icons/CorrodeCostUnsatisfied"].Id,
                            (Spr)MainManifest.sprites["icons/CorrodeCostSatisfied"].Id
                        ),
                        amount: 1
                    ),
                    new ADummyAction()
                );
                spendCorrodeAction.disabled = s.ship.Get((Status)MainManifest.statuses["honor"].Id) < 1 || s.ship.Get((Status)Enum.Parse<Status>("corrode")) < 1;
                spendCorrodeAction.disabled = s.route is Combat && spendCorrodeAction.disabled;

                var spendHonorToSpendCorrodeAction = MainManifest.KokoroApi.ActionCosts.Make
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
                    spendCorrodeAction
                );
                spendHonorToSpendCorrodeAction.disabled = spendCorrodeAction.disabled;

                return new()
                {
                    new AStatus() { status = (Status)MainManifest.KokoroApi.OxidationStatus.Id, targetPlayer = true, statusAmount = -4 },
                    spendHonorToSpendCorrodeAction
                };

            }

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
                MainManifest.KokoroApi.Actions.MakeContinued(continueId, new AStatus() { status = Enum.Parse<Status>("corrode"), targetPlayer = true, statusAmount = upgrade == Upgrade.B ? -2 : -1 }),
                MainManifest.KokoroApi.Actions.MakeContinued(continueId, new AStatus() { status = (Status)MainManifest.KokoroApi.OxidationStatus.Id, targetPlayer = true, statusAmount = -7 })
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 0 : 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BrewTea : Card
    {
        // TODO: for upgrade B, use Shockah's code (don't forget to thank him in the mod post! also thank rft for design help on halla, and everyone in the design sheet for help designing ratzo and bannerlady)
        // https://github.com/Shockah/Cobalt-Core-Mods/blob/dev/dracula/Dracula/Cards/BloodTapCard.cs
        // https://github.com/Shockah/Cobalt-Core-Mods/blob/dev/dracula/Dracula/ActionChoiceRoute.cs
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    new AHerbCardSelect()
                    {
                        browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                        browseAction = new ABrewChoiceTea()
                    }
                };
            }

            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                    browseAction = new ABrewTea()
                    {
                        increaseAmount = upgrade == Upgrade.B ? 2 : 1
                    }
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true, 
                description = upgrade switch {
                    Upgrade.None => "Choose herb in hand. Increase every effect by 1, remove one at random.",
                    Upgrade.A => "Choose herb in hand. Increase every effect by 1, remove one of your choice.",
                    Upgrade.B => "Choose herb in hand. Increase every effect by 2, remove one at random.",
                }
            };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FireAndSmoke : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = upgrade switch
            {
                Upgrade.None => new()
                {
                    new ARemoveSelectedCardFromWhereverItIs(),
                    new ASendSelectedCardToDiscard(),
                    new AApplySelectedHerbToEnemy(),
                    new AApplySelectedHerbToEnemy(),
                },
                Upgrade.A => new()
                {
                    new ARemoveSelectedCardFromWhereverItIs(),
                    new ASendSelectedCardToDiscard(),
                    new AApplySelectedHerbToEnemy(),
                    new AApplySelectedHerbToEnemy(),
                },
                Upgrade.B => new()
                {
                    new ARemoveSelectedCardFromWhereverItIs(),
                    new AApplySelectedHerbToEnemy(),
                    new AApplySelectedHerbToEnemy(),
                    new AApplySelectedHerbToEnemy(),
                    new AApplySelectedHerbToEnemy(),
                }
            };

            List<Icon> icons = new()
            {
                new Icon((Spr)MainManifest.sprites[upgrade == Upgrade.A ? "icons/herb_bundle" : "icons/herb_in_hand"].Id, 1, Colors.textMain),
                new Icon((Spr)MainManifest.sprites["icons/burn_herb"].Id, upgrade == Upgrade.B ? 4 : 2, Colors.textMain),
            };
            List<Tooltip> tooltips = new() { new TTGlossary(MainManifest.glossary["burnHerb"].Head) };

            if (upgrade == Upgrade.B)
            {
                tooltips.Add(new TTGlossary(MainManifest.glossary["moveCard"].Head, Enum.Parse<CardBrowse.Source>("DiscardPile"), "Destroy"));
                icons.Add(new Icon((Spr)MainManifest.sprites["icons/move_card"].Id, null, Colors.textMain));
                icons.Add(new Icon(Enum.Parse<Spr>("icons_singleUse"), null, Colors.textMain));
            }

            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>(upgrade == Upgrade.A ? "Deck" : "Hand"),
                    browseAction = new AQueueImmediateOtherActions()
                    {
                        actions = actions
                    }
                },
                new ATooltipDummy() { 
                    tooltips = tooltips,
                    icons = icons
                },
                new ADummyAction()
                //new ATooltipDummy() { tooltips = new (){ new TTGlossary(MainManifest.glossary["exhaustSelected"].Head), new TTGlossary(MainManifest.glossary["herbExhaust"].Head) } }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1,
                //description = upgrade switch
                //{
                //    Upgrade.None => "Select an herb in hand. Apply its effects to the enemy twice.",
                //    Upgrade.A => "Select an herb in hand. Apply its effects to the enemy twice, then return to hand.",
                //    Upgrade.B => "Select an herb in hand. Apply its effects to the enemy three times, then remove from deck."
                //}
            };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ChangeIngredients : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var count = c.hand.Where(card => card is HerbCard).Count();
            return new()
            {
                new ADiscardAllHerbsInHand(),
                new ADrawHerbCard() { amount = count }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 0 : 1, exhaust = upgrade == Upgrade.B ? false : true, description = "Discard all herbs in hand. Draw that many herbs." };
        }
    }


    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Cultivate : Card
    {
        // TODO: for upgrade B, use Shockah's code (don't forget to thank him in the mod post! also thank rft for design help on halla, and everyone in the design sheet for help designing ratzo and bannerlady)
        // https://github.com/Shockah/Cobalt-Core-Mods/blob/dev/dracula/Dracula/Cards/BloodTapCard.cs
        // https://github.com/Shockah/Cobalt-Core-Mods/blob/dev/dracula/Dracula/ActionChoiceRoute.cs
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                    browseAction = new ACultivate()
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 2,
                exhaust = upgrade != Upgrade.B,
                description = upgrade == Upgrade.A ? "Choose herb in deck. Double an action of your choice." : "Choose herb in hand. Double an action of your choice."
            };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Placebo : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (flipped)
            {
                return new()
                {
                    new AApplyToEnemyHerbCardOnTopOfDiscard()
                };
            }

            return new()
            {
                new APlayHerbCardOnTopOfDiscard()
            };
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = upgrade == Upgrade.A ? 0 : 1,
                floppable = upgrade == Upgrade.B,
                description = flipped
                    ? "Apply the topmost <c=heal>herb</c> in your <c=keyword>discard pile</c> to the enemy."
                    : "Play the topmost <c=heal>herb</c> in your <c=keyword>discard pile</c>." + (upgrade == Upgrade.B && state == DB.fakeState ? " (Flop: apply to enemy)" : "")
            };
        }
    }


    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Catalogue : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new ASelectCataloguedHerb()
                {
                    browseAction = new ASendSelectedCardToHand() { isGainingCard = true }
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 1,
                description = "Add a previously discovered herb card to your hand."
            };
        }
    }
}
