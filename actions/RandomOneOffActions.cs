using FMOD;
using FSPRO;
using KnightsCohort.Herbalist;
using KnightsCohort.Herbalist.Cards;
using Shockah.Dracula;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            c.SendCardToHand(s, selectedCard);
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

}
