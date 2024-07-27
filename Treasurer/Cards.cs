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


    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true, unreleased = true)]
    public class DEBUG_HonorShield : Card
    {
        public override List<CardAction> GetActions(State s, Combat c) { return new() { new AHonorShield() }; }
        public override CardData GetData(State state) { return new() { cost = 0 }; }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true, unreleased = true)]
    public class DEBUG_GoldShield : Card
    {
        public override List<CardAction> GetActions(State s, Combat c) { return new() { new AGoldShield() }; }
        public override CardData GetData(State state) { return new() { cost = 0 }; }
    }





    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CloakedInHonor : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AStatus() { status = Status.tempShield, targetPlayer = true, statusAmount = 1 }, new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 1 } },
                new() { new AHonorShield() },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }

    // Ruby: Will you be able to sign this or will I need to find your next of kin?
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class PetitionDonations : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var enemyHonorResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["honor"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.EnemyWithOutgoingArrow,
                (Spr)MainManifest.sprites["icons/honor_cost_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/honor_cost"].Id
            );

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 1),
                    new AGoldShield() { amount = 2 }
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UNNAMED1 : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AGoldShield() },
                new() { new AGoldShield() },
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
    public class HonorDuel : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AGoldShield(),
                new AHonorShield(),
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ExtremeConfidence : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2 },
                new ADrawCard()
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, infinite = upgrade == Upgrade.A };
        }
    }


    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Charity : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var enemyHonorResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["gold"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/gold_cost_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/gold_cost_satisfied"].Id
            );

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 2),
                    new AHonorShield()
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Twoumvirate : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AStatus() { status = Status.tempShield, statusAmount = 2, targetPlayer = true } },
                new() { new AStatus() { status = Status.shield, statusAmount = 2, targetPlayer = true } },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 2;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CallForRespite : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var xEqualsEnemyHonor = MainManifest.KokoroApi.Actions.SetTargetPlayer(
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                false
            );

            return new()
            {
                xEqualsEnemyHonor,
                new AStatus() { status = Status.tempShield, targetPlayer = false, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UNNAMED2 : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var enemyHonorResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["honor"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.EnemyWithOutgoingArrow,
                (Spr)MainManifest.sprites["icons/honor_cost_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/honor_cost"].Id
            );

            return new() {
                new() { new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = false } },
                new() {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 2),
                        new AHonorShield()
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

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UNNAMED3 : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {

            return new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["honorShield"].Id, secondStatus = (Status)MainManifest.statuses["goldShield"].Id },
                new AStatus() 
                { 
                    status = (Status)MainManifest.statuses["honor"].Id, 
                    targetPlayer = false, 
                    statusAmount = -s.ship.Get((Status)MainManifest.statuses["honorShield"].Id) - s.ship.Get((Status)MainManifest.statuses["goldShield"].Id), 
                    xHint=-1 
                },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UNNAMED4 : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 1, 3 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AHonorShield() },
                new() { new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = false } },
                new() { new AHonorShield() },
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
    public class AllIn : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {

            return new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["gold"].Id },
                new AStatus()
                {
                    status = (Status)MainManifest.statuses["goldShield"].Id,
                    targetPlayer = true,
                    statusAmount = s.ship.Get((Status)MainManifest.statuses["gold"].Id),
                    xHint=1
                },
                new AStatus() { mode = AStatusMode.Set, status = (Status)MainManifest.statuses["gold"].Id, statusAmount = 0 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UNNAMED5 : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {

            return new()
            {
                new ADummyAction()
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description= "NOT YET IMPLEMENTED! Gost 5 gold, temporarily upgrade a card in hand." };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class MarketSense : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 1 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            // TODO: replace charity with Savvy
            return new() {
                new() { new AStatus() { status = (Status)MainManifest.statuses["charity"].Id, statusAmount = 1, targetPlayer = true } },
                new() 
                { 
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = false },
                    new AStatus() { status = (Status)MainManifest.statuses["charity"].Id, statusAmount = 1, targetPlayer = false }
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
    public class UNNAMED6 : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2, 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            // TODO: replace charity with Savvy
            return new() {
                new() { new AStatus() { status = Status.tempShield, statusAmount = 1, targetPlayer = true } },
                new() { new AStatus() { status = Status.shield, statusAmount = 1, targetPlayer = true } },
                new() { new AStatus() { status = (Status)MainManifest.statuses["honorShield"].Id, statusAmount = 1, targetPlayer = true } },
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


            return new()
            {
                cost = upgrade == Upgrade.A ? 0 : 1,
                exhaust = true,
                description = state.ship.Get((Status)MainManifest.statuses["gold"].Id) >= goldCost
                    ? descriptionText
                    : $"<c=textFaint>{descriptionText}</c>"
            };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UNNAMED7 : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var xEqualsEnemyHonor = MainManifest.KokoroApi.Actions.SetTargetPlayer(
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                false
            );

            return new()
            {
                xEqualsEnemyHonor,
                new AHonorShield() { amount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint = 1 },
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }


    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Tollbooth : Card
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
                    MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: upgrade == Upgrade.B ? 5 : 3),
                    new ADrawCard() { count = 1 }
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, infinite = true };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class GoldenScales : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 3, 3 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AStatus() { status = Status.tempShield, statusAmount = 2, targetPlayer = true } },
                new() { new AHonorShield() },
                new() { new AStatus() { status = Status.shield, statusAmount = 2, targetPlayer = true } },
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
            return new()
            {
                cost = upgrade == Upgrade.A ? 0 : 1,
                description = upgrade switch
                {
                    Upgrade.None => $"Cost 3 gold, cancel 3 random enemy intents.",
                    Upgrade.A => $"Cost 3 gold, cancel 1 random enemy intent.",
                    Upgrade.B => $"Cost 5 gold, cancel all enemy intents.",
                }
            };
        }
    }
}
