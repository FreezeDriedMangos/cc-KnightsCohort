using KnightsCohort.actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace KnightsCohort.Treasurer.OldCards
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

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class InitialInvestment : InvestmentCard
    {
        public override List<int> upgradeCosts => upgrade == Upgrade.B
            ? new() { 1, 1 }
            : new() { 1 };

        public override bool SkipSpacerActions => upgrade == Upgrade.B ? true : false;

        protected override List<List<CardAction>> GetTierActions(State s, Combat c) 
        { 
            if (upgrade == Upgrade.B)
            {
                return new() {
                    new() {
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 }
                    },
                    new() {
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 },
                        new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = true, statusAmount = 1  }
                    },
                    new() {
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 }
                    },
                };
            }

            return new() { 
                new() { 
                    new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 },
                    new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 2 : 1  } 
                },
                new() {
                    new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 },
                    new AStatus() { status = Enum.Parse<Status>(upgrade == Upgrade.A ? "shield" : "tempShield"), targetPlayer = true, statusAmount = 1 }
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

            if (upgrade == Upgrade.A)
            {
                return new() {
                    new() { new ADrawCard() { count = 1 } },
                    new() {
                        MainManifest.KokoroApi.ActionCosts.Make
                        (
                            MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 1),
                            new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = true }
                        ),
                    }
                };
            }

            return new() {
                new() { new ADrawCard() { count = 1 } },
                new() { 
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 1),
                        new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = true }
                    ), 
                },
                new() {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                        new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = upgrade == Upgrade.B ? 3 : 2, targetPlayer = true }
                    ),
                },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            if (upgrade == Upgrade.A) cardData.infinite = true;
            return cardData;
        }
    }


    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class AskForAid : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { upgrade switch {
            Upgrade.None => 2,
            Upgrade.A => 1,
            Upgrade.B => 3
        }};

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
                    new ADrawCard() { count = upgrade == Upgrade.B ? 2 : 1 },
                    new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, statusAmount = upgrade == Upgrade.B ? 2 : 1, targetPlayer = true }
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
        public override List<int> upgradeCosts => upgrade switch {
            Upgrade.None => new() { 1, 2 },
            Upgrade.A => new() { 1, 1 },
            Upgrade.B => new() { 4, 1 },
        };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new ADrawCard() { count = upgrade == Upgrade.A ? 2 : 1 } },
                new() { new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = upgrade == Upgrade.B ? 2 : 1, targetPlayer = true } },
                new() { new ADrawCard() { count = upgrade == Upgrade.B ? 2 : 1 } },
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
        public override List<int> upgradeCosts => upgrade == Upgrade.A 
            ? new() { 1, 1 }
            : new() { 2, 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AAttack() { damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1) } },
                new() { new ADrawCard() { count = 2 } },
                new() { new AStatus() { status = Enum.Parse<Status>("shield"), targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 3 : 2 } }
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
            var xEqualsEnemyHeat = MainManifest.KokoroApi.Actions.SetTargetPlayer(
                new AVariableHint() { status = Enum.Parse<Status>("heat") }, 
                false
            );

            if (upgrade == Upgrade.B)
            {
                var multiplier = 2;
                return new()
                {
                   xEqualsEnemyHeat,
                   new AStatus() { status = Enum.Parse<Status>("heat"), statusAmount = multiplier*c.otherShip.Get(Enum.Parse<Status>("heat")), targetPlayer = true, xHint = multiplier },
                   new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = multiplier*c.otherShip.Get(Enum.Parse<Status>("heat")), targetPlayer = true, xHint = multiplier },
                };
            }

            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    new AStatus() { status = Enum.Parse<Status>("heat"), statusAmount = 1, targetPlayer = false },
                   xEqualsEnemyHeat,
                   new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = c.otherShip.Get(Enum.Parse<Status>("heat")), targetPlayer = true, xHint = 1 }
                };
            }

            return new()
            {
               xEqualsEnemyHeat,
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = c.otherShip.Get(Enum.Parse<Status>("heat")), targetPlayer = true, xHint = 1 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 2 : 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SpringCleaning : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var discard = upgrade switch
            {
                Upgrade.None => 2,
                Upgrade.A => 1,
                Upgrade.B => 3
            };

            return new()
            {
               new ADiscard() { count = discard },
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
               new AAttack() { damage = GetDmg(s, upgrade == Upgrade.B ? 1 : 2), status = Enum.Parse<Status>("heat"), statusAmount = upgrade == Upgrade.A ? 5 : 3 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 1 : 2 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class GoldenScales : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["gold"].Id },
                new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = true, statusAmount = s.ship.Get((Status)MainManifest.statuses["gold"].Id), xHint = 1 }
            };

            if (upgrade == Upgrade.A) retval.Insert(0, new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1});
            if (upgrade == Upgrade.B) retval.Insert(0, new AStatus() { status = Enum.Parse<Status>("shield"), targetPlayer = true, statusAmount = s.ship.Get((Status)MainManifest.statuses["gold"].Id), xHint = 1 });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 3 : 2 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class TradingOnReputation : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = s.ship.Get((Status)MainManifest.statuses["honor"].Id), xHint = 1 },
            };

            if (upgrade == Upgrade.None) retval.Add(new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = 0, mode = AStatusMode.Set });
            if (upgrade == Upgrade.B) retval.Add(new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Set });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = upgrade == Upgrade.A ? true : false };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ForgedInFire : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var xEqualsEnemyHeat = MainManifest.KokoroApi.Actions.SetTargetPlayer(
                new AVariableHint() { status = Enum.Parse<Status>("heat") },
                false
            );

            List<CardAction> retval = new()
            {
               xEqualsEnemyHeat,
               new ADrawCard() { count = c.otherShip.Get(Enum.Parse<Status>("heat")), xHint = 2 },
               new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 0, mode = AStatusMode.Set }
            };

            if (upgrade == Upgrade.A) retval.Add(new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 1, });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, retain = upgrade == Upgrade.B ? true : false };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class AncientWeapons : InvestmentCard
    {
        public override List<int> upgradeCosts => upgrade switch {
            Upgrade.None => new() { 2, 4 },
            Upgrade.A => new() { 1, 2 },
            Upgrade.B => new() { 3, 4 },
        };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = 1, targetPlayer = true } },
                new() { new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = 1, targetPlayer = true } },
                new() { new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = upgrade == Upgrade.B ? 2 : 1, targetPlayer = true } },
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
        public override List<int> upgradeCosts => upgrade == Upgrade.A 
            ? new() { 3, 3 }
            : new() { 5, 5 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var lastAction = upgrade == Upgrade.A
                ? new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = 2, targetPlayer = true }
                : new AStatus() { status = Enum.Parse<Status>("powerdrive"), statusAmount = 1, targetPlayer = true };

            return new() {
                new() { new AAttack() { damage = GetDmg(s, upgrade == Upgrade.B ? 1 : 0) } },
                new() { new AAttack() { damage = GetDmg(s, upgrade == Upgrade.B ? 1 : 0) } },
                new() { lastAction },
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
                new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 3 : 0, mode = AStatusMode.Set }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2, exhaust = upgrade == Upgrade.A ? false : true };
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

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class OpportunisticSale : InvestmentCard
    {
        public override List<int> upgradeCosts => upgrade == Upgrade.B
            ? new() { 2, 2 }
            : new() { 1, 1 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var enemyHeatResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                Enum.Parse<Status>("heat"),
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.EnemyWithOutgoingArrow,
                (Spr)MainManifest.sprites["icons/heat_cost_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/heat_cost_satisfied"].Id
            );
            int multiplier = upgrade == Upgrade.B ? 2 : 1;

            return new() {
                new() 
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHeatResource, amount: multiplier*1),
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = multiplier*1 }
                    )
                },
                new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHeatResource, amount: multiplier*1),
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = multiplier*2 }
                    )
                },
                new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHeatResource, amount: multiplier*1),
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = multiplier*3 }
                    )
                },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            if (upgrade == Upgrade.A) cardData.retain = true;
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

            int cost = upgrade switch
            {
                Upgrade.None => 5,
                Upgrade.A => 3,
                Upgrade.B => 10,
            };

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: cost),
                    upgrade == Upgrade.B 
                        ? new APlayHighestCostCardAnywhere()
                        : new APlayHighestCostCardInHand()
                )
            };
        }
        public override CardData GetData(State state)
        {
            var goldCost = upgrade switch { Upgrade.None => 5, Upgrade.A => 3, Upgrade.B => 10, _ => 999999 };
            var descriptionText = upgrade == Upgrade.B
                    ? $"Cost {goldCost} gold. Play highest energy cost card owned, wherever it is."
                    : $"Cost {goldCost} gold. Play the highest energy cost card in hand.";


            return new() { 
                cost = upgrade == Upgrade.A ? 0 : 1, 
                exhaust = true, 
                description = state.ship.Get((Status)MainManifest.statuses["gold"].Id) >= goldCost
                    ? descriptionText
                    : $"<c=textFaint>{descriptionText}</c>" 
            };
        }
    }


    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ReocurringDonation : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new() { new AStatus() { status = (Status)MainManifest.statuses["charity"].Id, targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 2 : 1 } };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = upgrade == Upgrade.A ? false : true };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class HotCommodity : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.B)
            {
                return new()
                {
                   new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 4 },
                   new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = true, statusAmount = 2 },
                   new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 2 : 1 }
                };
            }

            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 2 },
               new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 2 : 1 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 1 : 0 };
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

            CardAction boughtAction = upgrade switch
            {
                Upgrade.None => new AQueueImmediateOtherActions()
                    {
                        actions = new()
                        {
                            new ANullRandomIntent_Paranoia(),
                            new ANullRandomIntent_Paranoia(),
                            new ANullRandomIntent_Paranoia()
                        }
                    },
                Upgrade.A => new ANullRandomIntent_Paranoia(),
                Upgrade.B => new AStunShip(),
            };

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: upgrade == Upgrade.B ? 5 : 3),
                    boughtAction
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 0 : 1, description = upgrade switch
            {
                Upgrade.None => $"Cost 3 gold, cancel 3 random enemy intents.",
                Upgrade.A => $"Cost 3 gold, cancel 1 random enemy intent.",
                Upgrade.B => $"Cost 5 gold, cancel all enemy intents.",
            }};
        }
    }


    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Firewall : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.A)
            {
                return new()
                {
                   new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 3 },
                   new AStatus() { status = Enum.Parse<Status>("maxShield"), targetPlayer = false, statusAmount = 2 },
                   new AStatus() { status = Enum.Parse<Status>("shield"), targetPlayer = false, statusAmount = 2 },
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                   new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 2 },
                   new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = true, statusAmount = 1 },
                   new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = false, statusAmount = 2 },
                };
            } 

            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 2 },
               new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = false, statusAmount = 2 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 1 : 0 };
        }
    }


    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FlameCoating : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.A)
            {
                return new()
                {
                   new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = true, statusAmount = 1 },
                   new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 1 },
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                   new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = true, statusAmount = 2 },
                   new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 2 },
                };
            }

            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("tempShield"), targetPlayer = true, statusAmount = 2 },
               new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, infinite = upgrade == Upgrade.A };
        }
    }

    // TODO: Bad card - replace with something else (maybe a regular card with 2 outgoing heat, 2 outgoing heal, and 2 gold?)
    // Replace with a "pay gold, temp upgrade a card in hand (like upgrade how Cleo would)
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Cauterize : InvestmentCard
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
                    new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = upgrade == Upgrade.B ? 2 : 1 },
                },
                new()
                {
                    new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = false, statusAmount = 1 },
                    new AHeal() { targetPlayer = false, healAmount = 2 },

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
                        new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 2 }
                    )
                },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = upgrade == Upgrade.A ? 0 : 1;
            return cardData;
        }
    }

}
