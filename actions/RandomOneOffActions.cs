using FMOD;
using KnightsCohort.Herbalist;
using KnightsCohort.Herbalist.Cards;
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
    public class ABrewTea : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            if (selectedCard == null) return;
            if (selectedCard is not HerbCard herb) return;

            // choose an action type to remove
            HashSet<HerbActions> actions = new HashSet<HerbActions>(herb.SerializedActions);
            int removalIndex = (int)(Math.Abs(s.rngActions.NextInt()) % actions.Count);
            var removeAction = actions.ToList()[removalIndex];

            // build action counts for the tea card
            var actionCounts = HerbCard.CountSerializedActions(herb.SerializedActions);
            actionCounts.Remove(removeAction);
            actionCounts.Keys.ToList().ForEach(a => actionCounts[a]++);

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
    public class AEvaluateQuestSubmission : CardAction
    {
        public List<HerbActions> requirements;
        public int uuid;
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
            bool requirementsPerfectlyMet = submittedActions.Count == goalActions.Count;

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
}
