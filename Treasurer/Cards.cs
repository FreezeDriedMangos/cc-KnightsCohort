using KnightsCohort.actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace KnightsCohort.Treasurer.Cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Dragonfire : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2, 4 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            CardAction firstAction = upgrade == Upgrade.A
                ? new AAttack() { damage = GetDmg(s, 0), status = Enum.Parse<Status>("heat"), statusAmount = 1 }
                : new AStatus() { targetPlayer = false, status = Enum.Parse<Status>("heat"), statusAmount = 1 };

            return new() {
                new() { firstAction },
                new() { new AAttack() { damage = GetDmg(s, 1), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
                new() { new AAttack() { damage = GetDmg(s, 2), status = Enum.Parse<Status>("heat"), statusAmount = upgrade == Upgrade.B ? 3 : 1 } },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SuitableWeapons : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new ACardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                    browseAction = new DrawCardsOfSelectedCardColor() { count = upgrade == Upgrade.B ? 5 : 3 }
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = upgrade == Upgrade.A ? false : true, description = "Select a card in hand, draw 3 cards of that color." };
        }
    }
}
