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
            return new() {
                new() { new AStatus() { targetPlayer = false, status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
                new() { new AAttack() { damage = GetDmg(s, 1), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
                new() { new AAttack() { damage = GetDmg(s, 2), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
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
        public override List<int> upgradeCosts => new() { 1 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c) 
        { return new() { 
            new() { 
                new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 2 },
                new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = true, statusAmount = 1 } 
            },
            new() {
                new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 2 },
                new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = true, statusAmount = 1 }
            },
        };}

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 0;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Donation : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 1, 1 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var goldResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["gold"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
            );

            return new() {
                new() { new ADrawCard() { count = 2 } },
                new() { 
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 1),
                        new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 2, targetPlayer = true }
                    ), 
                },
                new() {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                        new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 4, targetPlayer = true }
                    ),
                },
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
    public class AskForAid : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var honorResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["honor"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/honor_cost_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/honor_cost"].Id
            );

            return new() {
                new()
                {
                    new ADrawCard() { count = 1 },
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(honorResource, amount: 1),
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, statusAmount = 3, targetPlayer = true }
                    ),
                },
                new() {
                    new ADrawCard() { count = 1 },
                    new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, statusAmount = 1, targetPlayer = true }
                },
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
    public class Inspiration : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 1, 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new ADrawCard() { count = 1 } },
                new() { new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = 1, targetPlayer = true } },
                new() { new ADrawCard() { count = 1 } },
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
    public class WeaponsVault : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2, 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AAttack() { damage = GetDmg(s, 1) } },
                new() { new ADrawCard() { count = 2 } },
                new() { new AStatus() { status = Enum.Parse<Status>("shield"), targetPlayer = true, statusAmount = 1 } }
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
    public class BurningGlory : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ATooltipDummy() { icons = new() {
                    new Icon(Enum.Parse<Spr>("icons_x"), null, Colors.textMain),
                    new Icon((Spr)MainManifest.sprites["icons/equal_sign"].Id, null, Colors.textMain),
                    new Icon(Enum.Parse<Spr>("icons_outgoing"), null, Colors.textMain),
                    new Icon(Enum.Parse<Spr>("icons_heat"), null, Colors.textMain),
                } },
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = c.otherShip.Get(Enum.Parse<Status>("heat")), targetPlayer = true, xHint = 1 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UNNAMED : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ADiscard() { count = 2 },
               new ADrawCard() { count = 2 },
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = true }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FireBreath : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AAttack() { damage = GetDmg(s, 2), status = Enum.Parse<Status>("heat"), statusAmount = 3 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class GoldenScales : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["gold"].Id },
                new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = true, statusAmount = s.ship.Get((Status)MainManifest.statuses["gold"].Id), xHint = 1 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class TradingOnReputation : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = s.ship.Get((Status)MainManifest.statuses["honor"].Id), xHint = 1 },
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = 0, mode = AStatusMode.Set }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ForgedInFire : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ATooltipDummy() { icons = new() {
                    new Icon(Enum.Parse<Spr>("icons_x"), null, Colors.textMain),
                    new Icon((Spr)MainManifest.sprites["icons/equal_sign"].Id, null, Colors.textMain),
                    new Icon(Enum.Parse<Spr>("icons_outgoing"), null, Colors.textMain),
                    new Icon(Enum.Parse<Spr>("icons_heat"), null, Colors.textMain),
                } },
               new ADrawCard() { count = c.otherShip.Get(Enum.Parse<Status>("heat")), xHint = 2 },
               new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 0, mode = AStatusMode.Set }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class AncientWeapons : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2, 4 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = 1, targetPlayer = true } },
                new() { new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = 1, targetPlayer = true } },
                new() { new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = 1, targetPlayer = true } },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 2;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class DeepSleep : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 5, 5 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AAttack() { damage = GetDmg(s, 0) } },
                new() { new AAttack() { damage = GetDmg(s, 0) } },
                new() { new AStatus() { status = Enum.Parse<Status>("powerdrive"), statusAmount = 1, targetPlayer = true } },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }


    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class GiveGenerously : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["gold"].Id },
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = s.ship.Get((Status)MainManifest.statuses["gold"].Id), xHint = 1 },
                new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = s.ship.Get((Status)MainManifest.statuses["gold"].Id), mode = AStatusMode.Set }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, exhaust = true };
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
                    browseAction = new DrawCardsOfSelectedCardColor() { count = 3 }
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true, description = "Select a card in hand, draw 3 cards of that color." };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class OpportunisticSale : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 1, 1 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var enemyHeatResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                Enum.Parse<Status>("heat"),
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.EnemyWithOutgoingArrow,
                (Spr)MainManifest.sprites["icons/heat_cost_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/heat_cost_satisfied"].Id
            );

            return new() {
                new() 
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHeatResource, amount: 1),
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 }
                    )
                },
                new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHeatResource, amount: 1),
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 2 }
                    )
                },
                new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHeatResource, amount: 1),
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 3 }
                    )
                },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BigBudget : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var goldResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["gold"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
            );

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 5),
                    new APlayHighestCostCardInHand()
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true, description = "Cost 5 gold. Play the highest energy cost card in hand." };
        }
    }


    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ReocurringDonation : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new() { new AStatus() { status = (Status)MainManifest.statuses["charity"].Id, targetPlayer = true, statusAmount = 1 } };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class HotCommodity : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 2 },
               new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Bribe : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var goldResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["gold"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
            );

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                    new ANullRandomIntent_Paranoia()
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = $"Cost {2} gold, cancel a random enemy intent." };
        }
    }
}
