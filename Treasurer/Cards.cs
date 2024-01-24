using KnightsCohort.actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnightsCohort.Treasurer.Cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Dragonfire : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2, 4 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AAttack() { damage = GetDmg(s, 0), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
                new() { new AAttack() { damage = GetDmg(s, 2), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
                new() { new AAttack() { damage = GetDmg(s, 3), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class InitialInvestment : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 1, 3 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c) 
        { return new() { 
            new() { new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 } }, 
            new() { 
                new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 2 } ,
                new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = true, statusAmount = 2 } 
            }, 
            new() { new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 4 } }
        };}

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }
}
