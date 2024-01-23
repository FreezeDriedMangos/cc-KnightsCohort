using KnightsCohort.Knight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Treasurer.Cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Dragonfire : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new();
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }
}
