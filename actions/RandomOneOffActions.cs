using FMOD;
using FSPRO;
using HarmonyLib;
using KnightsCohort.Herbalist;
using KnightsCohort.Herbalist.Cards;
using Microsoft.Extensions.Logging;
using Shockah.Dracula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class AQueueImmediateOtherActions : CardAction
    {
        public List<CardAction> actions = new();

        public override void Begin(G g, State s, Combat c)
        {
            actions.ForEach(a => a.selectedCard = selectedCard);
            c.QueueImmediate(actions);
        }
    }
    public class ARemoveSelectedCardFromWhereverItIs : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            s.RemoveCardFromWhereverItIs(selectedCard.uuid);
        }
    }
    public class ASendSelectedCardToHand : CardAction
    {
        public bool isGainingCard = false;
        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            c.SendCardToHand(s, selectedCard);

            if (isGainingCard)
            {
                foreach (Artifact item in g.state.EnumerateAllArtifacts())
                {
                    item.OnPlayerRecieveCardMidCombat(g.state, c, selectedCard);
                }
            }
        }
    }
    public class ASendSelectedCardToDiscard : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            c.SendCardToDiscard(s, selectedCard);
        }
    }

    public class AApplySelectedHerbToEnemy : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null || selectedCard is not HerbCard herb) return;

            // record that this herb existed
            if (!herb.GetDataWithOverrides(s).temporary)
            { 
                var cataloguedHerbs = HerbCard.GetCatalogue(s);
                cataloguedHerbs[herb.uuid] = herb;
                MainManifest.KokoroApi.SetExtensionData<Dictionary<int, HerbCard>>(s, HerbCard.CATALOGUED_HERBS_KEY, cataloguedHerbs);
            }

            // queue actual actions
            var actions = herb.GetActionsOverridden(s, c);
            foreach (var action in actions)
            {
                if (action is AStatus astatus) astatus.targetPlayer = !astatus.targetPlayer;
                if (action is AMove amove) amove.targetPlayer = !amove.targetPlayer;
                if (action is AHeal aheal) aheal.targetPlayer = !aheal.targetPlayer;
                if (action is AHurt ahurt) ahurt.targetPlayer = !ahurt.targetPlayer;
            }

            bool singleUseOverriden = herb.singleUseOverride.HasValue && herb.singleUseOverride.Value == false;
            if (!singleUseOverriden) actions.Insert(0, new ACompletelyRemoveCard() { uuid = selectedCard.uuid, skipHandCheck = true });
            else                     actions.Insert(0, new ASendSelectedCardToDiscard() { selectedCard = selectedCard });

            if (!herb.revealed)
            {
                herb.revealed = true;
                actions.Insert(0, new ADisplayCard() { uuid = selectedCard.uuid });
            }
            c.QueueImmediate(actions);


            //s.RemoveCardFromWhereverItIs(selectedCard.uuid);
        }
    }

    public class ABrewChoiceTea : CardAction
    {
        public List<List<CardAction>>? Choices;

        public List<List<CardAction>> GetChoices(State s, Combat c)
        {
            if (selectedCard is not HerbCard herb) return new();

            HashSet<HerbActions> herbActions = new HashSet<HerbActions>(herb.SerializedActions);
            Dictionary<HerbActions, int> herbActionCounts = HerbCard.CountSerializedActions(herb.SerializedActions);
            List<List<CardAction>> retval = new();
            foreach (var action in herbActions)
            {
                var cardaction = new ABrewTea() { chosenRemoval = action, iconCount = herbActionCounts[action] };
                cardaction.selectedCard = selectedCard;
                retval.Add(new() { cardaction });
            }
            return retval;
        }

        public override Route? BeginWithRoute(G g, State s, Combat c) => new ActionChoiceRoute
        {
            Title = "effect to remove",
            Choices = Choices ?? GetChoices(s, c)
        };

        public override List<Tooltip> GetTooltips(State s)
        {
            return new();
        }
    }

    public class ABrewTea : CardAction
    {
        public int increaseAmount = 1;
        public HerbActions? chosenRemoval = null;
        public int iconCount;

        public override Icon? GetIcon(State s)
        {
            if (chosenRemoval == null) return null;
            return HerbCard.ParseSerializedAction((HerbActions)chosenRemoval, iconCount).GetIcon(s);
        }

        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            if (selectedCard is not HerbCard herb) return;

            // choose an action type to remove
            HashSet<HerbActions> actions = new HashSet<HerbActions>(herb.SerializedActions);
            var removeAction = chosenRemoval ?? actions.ToList()[Math.Abs(s.rngActions.NextInt()) % actions.Count];

            // build action counts for the tea card
            var actionCounts = HerbCard.CountSerializedActions(herb.SerializedActions);
            actionCounts.Keys.ToList().ForEach(a => actionCounts[a] += increaseAmount);
            actionCounts.Remove(removeAction);

            // make tea card
            HerbCard_Tea tea = new HerbCard_Tea();
            tea.SerializedActions = new();
            foreach (var kvp in actionCounts) for (int i = 0; i < kvp.Value; i++) tea.SerializedActions.Add(kvp.Key);
            tea.name = herb.name.Split(' ')[0] + " Tea";
            tea.revealed = true;
            tea.isPoultice = herb.isPoultice;
            tea.isCultivated = herb.isCultivated;
            tea.isTea = true;

            // remove the herb and add the tea
            s.RemoveCardFromWhereverItIs(selectedCard.uuid);
            c.QueueImmediate(new AAddCard() { card = tea, destination = Enum.Parse<CardDestination>("Hand") });
        }
    }

    public class ACultivate : CardAction
    {
        public List<List<CardAction>>? Choices;

        public List<List<CardAction>> GetChoices(State s, Combat c)
        {
            if (selectedCard is not HerbCard herb) return new();

            HashSet<HerbActions> herbActions = new HashSet<HerbActions>(herb.SerializedActions);
            Dictionary<HerbActions, int> herbActionCounts = HerbCard.CountSerializedActions(herb.SerializedActions);
            List<List<CardAction>> retval = new();
            foreach (var action in herbActions)
            {
                var cardaction = new ADouble() { chosenDouble = action, iconCount = herbActionCounts[action] };
                cardaction.selectedCard = selectedCard;
                retval.Add(new() { cardaction });
            }
            return retval;
        }

        public override Route? BeginWithRoute(G g, State s, Combat c) => new ActionChoiceRoute
        {
            Title = "effect to double",
            Choices = Choices ?? GetChoices(s, c)
        };

        public override List<Tooltip> GetTooltips(State s)
        {
            return new();
        }
    }
    public class ADouble : CardAction
    {
        public HerbActions chosenDouble;
        public int iconCount;

        public override Icon? GetIcon(State s)
        {
            return HerbCard.ParseSerializedAction(chosenDouble, iconCount).GetIcon(s);
        }

        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            if (selectedCard is not HerbCard herb) return;

            // build action counts for the tea card
            var actionCounts = HerbCard.CountSerializedActions(herb.SerializedActions);
            actionCounts[chosenDouble] *= 2;

            // make cultivar card
            HerbCard newHerb = new HerbCard();
            newHerb.SerializedActions = new();
            foreach (var kvp in actionCounts) for (int i = 0; i < kvp.Value; i++) newHerb.SerializedActions.Add(kvp.Key);
            newHerb.name = herb.name.Split(' ')[0] + " Cultivar";
            newHerb.revealed = true;

            newHerb.isPoultice = herb.isPoultice;
            newHerb.isTea = herb.isTea;
            newHerb.isCultivated = true;

            // remove the herb and add the cultivar
            s.RemoveCardFromWhereverItIs(selectedCard.uuid);
            c.QueueImmediate(new AAddCard() { card = newHerb, destination = Enum.Parse<CardDestination>("Hand") });
        }
    }


    public class AEvaluateQuestSubmissionWithRequirementRemoval : CardAction
    {
        public List<List<CardAction>>? Choices;
        public List<HerbActions> requirements;
        public int uuid;

        public List<List<CardAction>> GetChoices(State s, Combat c)
        {
            HashSet<HerbActions> herbActions = new HashSet<HerbActions>(requirements);
            Dictionary<HerbActions, int> herbActionCounts = HerbCard.CountSerializedActions(requirements);
            List<List<CardAction>> retval = new();
            foreach (var action in herbActions)
            {
                retval.Add(new() { new AEvaluateQuestSubmissionWithOmission() 
                { 
                    requirements = requirements.Where(r => r != action).ToList(), 
                    omit = action, 
                    iconCount = herbActionCounts[action],
                    uuid = uuid,
                    selectedCard = selectedCard
                }});
            }
            return retval;
        }

        public override Route? BeginWithRoute(G g, State s, Combat c) => new ActionChoiceRoute
        {
            Title = "effect to remove",
            Choices = Choices ?? GetChoices(s, c)
        };

        public override List<Tooltip> GetTooltips(State s)
        {
            return new();
        }
    }

    public class AEvaluateQuestSubmissionWithOmission : AEvaluateQuestSubmission
    {
        public HerbActions omit;
        public int iconCount;
        public override Icon? GetIcon(State s)
        {
            return HerbCard.ParseSerializedAction(omit, iconCount).GetIcon(s);
        }
    }

    public class AEvaluateQuestSubmission : CardAction
    {
        public List<HerbActions> requirements;
        public int uuid;
        public bool supersetCountsAsPerfect;
        public static HashSet<string> SetizeSerializedHerbActions(List<HerbActions> ha)
        {
            Dictionary<HerbActions, int> counts = new();
            HashSet<string> retval = new HashSet<string>();
            foreach (HerbActions action in ha)
            {
                counts.TryAdd(action, 0);
                counts[action]++;
                retval.Add(action + ":" + counts[action]);
            }
            return retval;
        }

        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            var goalActions = SetizeSerializedHerbActions(requirements);
            var submittedActions = SetizeSerializedHerbActions((selectedCard as HerbCard).SerializedActions);

            bool allRequirementsMet = submittedActions.Fast_AllAreIn(goalActions);
            bool requirementsPerfectlyMet = supersetCountsAsPerfect || submittedActions.Count == goalActions.Count;

            if (allRequirementsMet && requirementsPerfectlyMet) { c.QueueImmediate(new AAddCard() { card = new EpicQuestReward(), amount = 1, destination = Enum.Parse<CardDestination>("Hand") }); }
            else if (allRequirementsMet) { c.QueueImmediate(new AAddCard() { card = new QuestReward(), amount = 1, destination = Enum.Parse<CardDestination>("Hand") }); }

            s.RemoveCardFromWhereverItIs(uuid);
        }
    }
    public class AQueueImmediateSelectedCardActions : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            c.QueueImmediate(selectedCard.GetActionsOverridden(s, c));
        }
    }

    public class ADiscardAllHerbsInHand : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            var herbs = c.hand.Where(card => card is HerbCard).ToList();
            foreach (var herb in herbs) c.SendCardToDiscard(s, herb);
        }
    }

    public class ADrawHerbCard : CardAction
    {
        public int amount;

        public override void Begin(G g, State s, Combat c)
        {
            bool flag = false;
            int num = 0;
            for (int i = 0; i < amount; i++)
            {
                if (c.hand.Count >= 10)
                {
                    c.PulseFullHandWarning();
                    break;
                }
                if (s.deck.Count == 0)
                {
                    foreach (Card item in c.discard)
                    {
                        s.SendCardToDeck(item, doAnimation: true);
                    }
                    c.discard.Clear();
                    s.ShuffleDeck(isMidCombat: true);
                    if (s.deck.Count() == 0)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    Audio.Play(Event.CardHandling);
                    flag = true;
                }

                bool drawn = false;
                for (int j = 0; j < s.deck.Count; j++)
                {
                    if (s.deck[j] is not HerbCard) continue;
                    c.DrawCardIdx(s, j).waitBeforeMoving = (double)i * 0.09;
                    drawn = true;
                    break;
                }
                if (!drawn) break;
                num++;
            }
            foreach (Artifact item2 in s.EnumerateAllArtifacts())
            {
                item2.OnDrawCard(s, c, num);
            }
        }
    }

    public class ANullRandomIntent_Paranoia : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            var parts = c.otherShip.parts.Where(p => p.intent != null).ToList();
            parts.KnightRandom(s.rngActions).intent = null;
        }
    }

    public class ARedNumberStatus : AStatus
    {
        public override Icon? GetIcon(State s)
        {
            var icon = base.GetIcon(s);
            if (icon == null) return null;

            return new Icon(icon.Value.path, icon.Value.number, Colors.redd, icon.Value.flipY);
        }
    }

    public class APlayHerbCardOnTopOfDiscard : CardAction 
    {
        public override void Begin(G g, State s, Combat c)
        {
            timer = 0;
            for (int i = c.discard.Count - 1; i >= 0; i--)
            {
                var card = c.discard[i];
                if (card is not HerbCard herb) continue;
                FoundHerb(herb, s, c);
                return;
            }
        }

        public virtual void FoundHerb(HerbCard herb, State s, Combat c)
        {
            c.QueueImmediate(herb.GetActionsOverridden(s, c));
        }
    }

    public class AApplyToEnemyHerbCardOnTopOfDiscard : APlayHerbCardOnTopOfDiscard
    {
        public override void FoundHerb(HerbCard herb, State s, Combat c)
        {
            var a = new AApplySelectedHerbToEnemy();
            a.selectedCard = herb;
            c.QueueImmediate(a);
        }
    }

    public class DrawCardsOfSelectedCardColor : CardAction
    {
        public int count;

        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            var deck = selectedCard.GetMeta().deck;

            bool flag = false;
            int num = 0;
            for (int i = 0; i < count; i++)
            {
                if (c.hand.Count >= 10)
                {
                    c.PulseFullHandWarning();
                    break;
                }
                if (s.deck.Count == 0)
                {
                    foreach (Card item in c.discard)
                    {
                        s.SendCardToDeck(item, doAnimation: true);
                    }
                    c.discard.Clear();
                    s.ShuffleDeck(isMidCombat: true);
                    if (s.deck.Count() == 0)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    Audio.Play(Event.CardHandling);
                    flag = true;
                }

                bool drawn = false;
                for (int j = 0; j < s.deck.Count; j++)
                {
                    if (s.deck[j].GetMeta().deck != deck) continue;
                    c.DrawCardIdx(s, j).waitBeforeMoving = (double)i * 0.09;
                    drawn = true;
                    break;
                }
                if (!drawn)
                {
                    if (c.discard.Count == 0) break; // no cards to draw

                    // else, shuffle discard into deck and try again
                    foreach (Card item in c.discard)
                    {
                        s.SendCardToDeck(item, doAnimation: true);
                    }
                    c.discard.Clear();
                    s.ShuffleDeck(isMidCombat: true);
                    i--;
                }
                num++;
            }
            foreach (Artifact item2 in s.EnumerateAllArtifacts())
            {
                item2.OnDrawCard(s, c, num);
            }
        }
    }

    public class APlayHighestCostCardInHand : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            base.Begin(g, s, c);
            int highestCost = 0;
            c.hand.ForEach(card => highestCost = Math.Max(highestCost, card.GetCurrentCost(s)));
            for(int i = 0; i < c.hand.Count; i++)
            {
                if (c.hand[i].GetCurrentCost(s) == highestCost)
                {
                    c.QueueImmediate(new APlayOtherCard()
                    {
                        timer = 0.5,
                        handPosition = i
                    });
                    return;
                }
            }
        }
    }

    public class APlayHighestCostCardAnywhere : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            base.Begin(g, s, c);
            int highestCost = 0;
            s.deck.ForEach(card => highestCost = Math.Max(highestCost, card.GetCurrentCost(s)));
            for(int i = 0; i < s.deck.Count; i++)
            {
                if (s.deck[i].GetCurrentCost(s) == highestCost)
                {
                    c.QueueImmediate(MainManifest.KokoroApi.Actions.MakePlaySpecificCardFromAnywhere(s.deck[i].uuid));
                    return;
                }
            }
        }
    }

    public class APlaySelectedCardFromAnywhere : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            c.QueueImmediate(MainManifest.KokoroApi.Actions.MakePlaySpecificCardFromAnywhere(selectedCard.uuid));
        }
    }

    public class ASelectCataloguedHerb : CardAction
    {
        public CardAction browseAction;

        public override Route? BeginWithRoute(G g, State s, Combat c)
        {
            CardBrowse cardBrowse = new CataloguedHerbCardBrowse
            {
                mode = CardBrowse.Mode.Browse,
                browseAction = browseAction,
            };
            c.Queue(new ADelay
            {
                time = 0.0,
                timer = 0.0
            });
            if (cardBrowse.GetCardList(g).Count == 0)
            {
                MainManifest.Instance.Logger.LogInformation("No herb cards in catalogue");
                timer = 0.0;
                return null;
            }
            return cardBrowse;
        }

        public override bool CanSkipTimerIfLastEvent()
        {
            return false;
        }

    }

    [HarmonyPatch]
    public class CataloguedHerbCardBrowse : CardBrowse
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CardBrowse), nameof(CardBrowse.GetCardList))]
        public static bool GetCardList(CardBrowse __instance, ref List<Card> __result, G g)
        {
            if (__instance is not CataloguedHerbCardBrowse) return true;

            __result = HerbCard.GetCatalogue(g.state).Values.Select(herb => (Card)herb).ToList();
            typeof(CardBrowse)
                .GetField(nameof(CardBrowse._listCache), AccessTools.all)
                .SetValue(__instance, __result);
            return false;
        }
    }

    public class ADisplayCard : CardAction
    {
        public int uuid;
        public double waitBeforeMoving = 1.2;

        public override void Begin(G g, State s, Combat c)
        {
            Card card = s.FindCard(uuid);

            timer = waitBeforeMoving + 0.2;
            if (s.route is Combat)
            {
                card.pos = new Vec(G.screenSize.x * 0.5 - 30.0, 30.0);
                card.waitBeforeMoving = waitBeforeMoving + 1;
                card.drawAnim = 1.0;
            }
        }
    }

    public class AExhaustFX : CardAction
    {
        public Card card;
        public override void Begin(G g, State s, Combat c)
        {
            card.ExhaustFX();
        }
    }

    public class ACompletelyRemoveCard : CardAction
    {
        public int uuid;
        public bool skipHandCheck;
        
        public const double delayTimer = 0.3;

        public override void Begin(G g, State s, Combat c)
        {
            timer = 0.0;
            Card card = s.FindCard(uuid);
            if (card != null && (skipHandCheck || c.hand.Contains(card)))
            {
                card.ExhaustFX();
                Audio.Play(Event.CardHandling);
                c.hand.Remove(card);
                s.RemoveCardFromWhereverItIs(uuid);
                timer = 0.3;
            }
        }
    }

    public class AAddTempCopyOfSelectedCard : CardAction
    {
        public bool isGainingCard = false;
        public CardDestination destination;

        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            Card newCard = selectedCard.CopyWithNewId();
            newCard.temporaryOverride = true;

            c.QueueImmediate(new AAddCard() { card = newCard, destination = destination });
        }
    }
}
