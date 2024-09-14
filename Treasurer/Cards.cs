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


    [CardMeta(rarity = Rarity.uncommon, dontOffer = true, unreleased = true)]
    public class DEBUG_HonorShield : Card
    {
        public override List<CardAction> GetActions(State s, Combat c) { return new() { new AHonorShield() }; }
        public override CardData GetData(State state) { return new() { cost = 0 }; }
    }

    [CardMeta(rarity = Rarity.uncommon, dontOffer = true, unreleased = true)]
    public class DEBUG_GoldShield : Card
    {
        public override List<CardAction> GetActions(State s, Combat c) { return new() { new AGoldShield() }; }
        public override CardData GetData(State state) { return new() { cost = 0 }; }
    }





    [CardMeta(rarity = Rarity.common, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class CloakedInHonor : InvestmentCard
    {
        public override List<int> upgradeCosts => upgrade == Upgrade.B ? new() { 3 } : new() { 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var xEqualsEnemyHonor = MainManifest.KokoroApi.Actions.SetTargetPlayer(
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                false
            );

            if (upgrade == Upgrade.A)
            {
                return new() {
                    new() { new AStatus() { status = Status.tempShield, targetPlayer = true, statusAmount = 1 }, new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 1 } },
                    new() { xEqualsEnemyHonor, new AStatus() { status = Status.maxShield, targetPlayer = true, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint = 1 } },
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new() {
                    new() { new AStatus() { status = Status.tempShield, targetPlayer = true, statusAmount = 2 }, new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2 } },
                    new() { new AHonorShield() },
                };
            }

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
    [CardMeta(rarity = Rarity.common, upgradesTo = [ Upgrade.A, Upgrade.B ])]
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

            var goldResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["gold"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
            );

            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource , amount: 1),
                        new ACharge() { dir = 2 }
                    ),
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 1),
                        new AGoldShield() { amount = 2 }
                    ),
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                    new ACharge() { dir = 1 },
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 1),
                        new AGoldShield() { amount = 2 }
                    ),
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 1),
                        new AGoldShield() { amount = 2 }
                    ),
                };
            }

            return new()
            {
                new ACharge() { dir = 1 },
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

    [CardMeta(rarity = Rarity.common, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class GoldStandard : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { upgrade == Upgrade.B ? 1: 2 };

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
                    new() { new AGoldShield() },
                    new() { MainManifest.KokoroApi.ActionCosts.Make
                        (
                            MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 1),
                            new AGoldShield() { amount = 3 }
                        )
                    }
                };
            }

            return new() {
                new() { new AGoldShield() },
                new() { new AGoldShield() { amount = 2 } },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class HonorDuel : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    new ACharge() { dir = 2 },
                    new AGoldShield(),
                    new AHonorShield(),
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2 }
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                    new AHonorShield(),
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2 }
                };
            }

            return new()
            {
                new AGoldShield(),
                new AHonorShield(),
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 1 : 2 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class ExtremeConfidence : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    new ACharge() { dir = 1 },
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2 },
                    new ADrawCard() { count = 1 }
                };
            }

            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2 },
                new ADrawCard() { count = 2 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, infinite = upgrade == Upgrade.B };
        }
    }


    [CardMeta(rarity = Rarity.common, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class Charity : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var enemyHonorResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["gold"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
            );

            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 2),
                        new AHonorShield()
                    ),
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 2),
                        new AHonorShield()
                    ),
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 2),
                        new AHonorShield()
                    ),
                    new AVariableHint() { status = (Status)MainManifest.statuses["gold"].Id },
                    new AStatus() { status = Status.maxShield, xHint = 1, statusAmount = s.ship.Get((Status)MainManifest.statuses["gold"].Id), targetPlayer = true },
                    new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, statusAmount = 0, mode = AStatusMode.Set, targetPlayer = true },
                };
            }

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

    [CardMeta(rarity = Rarity.common, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class Twoumvirate : InvestmentCard
    {
        public override List<int> upgradeCosts => upgrade == Upgrade.B ? new() { 2, 2 } : new() { 2 };

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
                    new() { new AStatus() { status = Status.tempShield, statusAmount = 2, targetPlayer = true } },
                    new() { new AStatus() { status = Status.shield, statusAmount = 1, targetPlayer = true } },
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new() {
                    new() { new AStatus() { status = Status.tempShield, statusAmount = 2, targetPlayer = true } },
                    new() { new AStatus() { status = Status.shield, statusAmount = 1, targetPlayer = true } },
                    new() { MainManifest.KokoroApi.ActionCosts.Make
                        (
                            MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                            new AStatus() { status = Status.shield, statusAmount = 2, targetPlayer = true }
                        ) 
                    }
                };
            }

            return new() {
                new() { new AStatus() { status = Status.tempShield, statusAmount = 2, targetPlayer = true } },
                new() { new AStatus() { status = Status.shield, statusAmount = 2, targetPlayer = true } },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = upgrade == Upgrade.A ? 1 : 2;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class CallForRespite : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var xEqualsEnemyHonor = MainManifest.KokoroApi.Actions.SetTargetPlayer(
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                false
            );

            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    xEqualsEnemyHonor,
                    new AStatus() { status = Status.tempShield, targetPlayer = true, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
                    new AStatus() { status = Status.maxShield,  targetPlayer = true, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                    xEqualsEnemyHonor,
                    new AStatus() { status = Status.tempShield, targetPlayer = true, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
                    new AStatus() { status = Status.shield,     targetPlayer = true, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
                };
            }

            return new()
            {
                xEqualsEnemyHonor,
                new AStatus() { status = Status.tempShield, targetPlayer = true, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class Bravado : InvestmentCard
    {
        public override List<int> upgradeCosts => upgrade switch {
            Upgrade.None => new() { 2 },
            Upgrade.A => new() { 2, 2 },
            Upgrade.B => new() { 2 },
        };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var enemyHonorResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["honor"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.EnemyWithOutgoingArrow,
                (Spr)MainManifest.sprites["icons/honor_cost_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/honor_cost"].Id
            );

            var goldResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["gold"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
            );

            var upgradeTiers = new List<List<CardAction>>() {
                new() { 
                    new ACharge() { dir = 2 },
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = false } 
                },
                new() {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 2),
                        new AHonorShield()
                    ),
                },
            };

            if (upgrade == Upgrade.B)
            {
                upgradeTiers[1][0] = MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                    new AHonorShield()
                );
            }

            if (upgrade == Upgrade.A)
            {
                upgradeTiers.Add(new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 1),
                        new AHonorShield()
                    ),
                });
            } 
            else
            {
                //upgradeTiers[1].Add(new ADummyAction());
            }

            return upgradeTiers;
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class Revocation : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    new AGoldShield(),
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

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                    new AStatus() { status = Status.shield, targetPlayer = true, statusAmount = 1 },
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

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class ForGlory : InvestmentCard
    {
        public override List<int> upgradeCosts => upgrade == Upgrade.A ? new() { 3 } : new() { 1, 3 };

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
                    new() { 
                        MainManifest.KokoroApi.ActionCosts.Make
                        (
                            MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 1),
                            new AHonorShield()
                        ) 
                    },
                    new() { 
                        MainManifest.KokoroApi.ActionCosts.Make
                        (
                            MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                            new AHonorShield()
                        ) 
                    },
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new() {
                    new() { new AHonorShield() },
                    new() { new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = false } },
                    new() { new AStatus() { status = Status.maxShield, statusAmount = 1, targetPlayer = true }, new AHonorShield() },
                };
            }

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

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class AllIn : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var actions = new List<CardAction>();

            if (upgrade == Upgrade.A)
            {
                actions.Add(new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 });
            }

            actions.AddRange(new List<CardAction>()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["gold"].Id },
                new AStatus()
                {
                    status = (Status)MainManifest.statuses["goldShield"].Id,
                    targetPlayer = true,
                    statusAmount = s.ship.Get((Status)MainManifest.statuses["gold"].Id),
                    xHint=1
                },
                new AStatus() { mode = AStatusMode.Set, status = (Status)MainManifest.statuses["gold"].Id, statusAmount = 0, targetPlayer = true }
            });

            return actions;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, recycle = upgrade == Upgrade.B };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, dontOffer = true)] // TODO: remove DONT OFFER
    public class DeluxeEdition : Card
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
            return new() { cost = 1, description= "NOT YET IMPLEMENTED! Cost 5 gold, temporarily upgrade a card in hand." };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class HoardWards : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2 };

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
                    new() { new AStatus() { status = Status.maxShield, statusAmount = 1, targetPlayer = true } },
                    new () {
                        MainManifest.KokoroApi.ActionCosts.Make
                        (
                            MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                            new AStatus() { status = Status.shield, statusAmount = 2, targetPlayer = true }
                        )
                    }
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new() {
                    new() {
                        MainManifest.KokoroApi.ActionCosts.Make
                        (
                            MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                            new AStatus() { status = Status.maxShield, statusAmount = 2, targetPlayer = true }
                        )
                    },
                    new () { new AStatus() { status = Status.shield, statusAmount = 1, targetPlayer = true } }
                };
            }

            return new() {
                new() { new AStatus() { status = Status.maxShield, statusAmount = 1, targetPlayer = true } },
                new() { new AStatus() { status = Status.shield, statusAmount = 1, targetPlayer = true } },
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
            var goldResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["gold"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
            );

            if (upgrade != Upgrade.None)
            {

                return new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: upgrade == Upgrade.B ? 5 : 3),
                        new ACardSelect()
                        {
                            browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                            browseAction = new DrawCardsOfSelectedCardColor() { count = upgrade == Upgrade.B ? 5 : 3 }
                        }
                    )
                };
            }

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
            string description = upgrade switch
            {
                Upgrade.None => "Select a card in hand, draw 3 cards of that color.",
                Upgrade.A => "Cost 3 gold. Select a card in hand, draw 3 cards of that color.",
                Upgrade.B => "Cost 5 gold. Select a card in hand, draw 3 cards of that color.",
            };

            int goldCost = upgrade switch
            {
                Upgrade.None => 0,
                Upgrade.A => 3,
                Upgrade.B => 5,
            };

            description = state.route is Combat && state.ship.Get((Status)MainManifest.statuses["gold"].Id) >= goldCost
                ? description
                : $"<c=textFaint>{description}</c>";

            return new() 
            { 
                cost = 1, 
                exhaust = upgrade == Upgrade.A ? false : true, 
                description = description
            };
        }
    }

    [CardMeta(rarity = Rarity.uncommon)]
    public class LayeredShield : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2, 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
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

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class ShieldCharge : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2, 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            if (upgrade == Upgrade.A)
            {
                return new() {
                    new() { new ACharge() { dir = 2 } },
                    new() { new AStatus() { status = Status.shield, statusAmount = 3, targetPlayer = true } },
                    new() { new ACharge() { dir = 2 } },
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new() {
                    new() { new ACharge() { dir = 3 } },
                    new() { new AStatus() { status = Status.shield, statusAmount = 2, targetPlayer = true } },
                    new() { new ACharge() { dir = 3 } },
                };
            }

            return new() {
                new() { new ACharge() { dir = 2 } },
                new() { new AStatus() { status = Status.shield, statusAmount = 2, targetPlayer = true } },
                new() { new ACharge() { dir = 2 } },
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
                Upgrade.A => 7,
                Upgrade.B => 7,
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
                cost = 1,
                exhaust = (upgrade == Upgrade.A ? false : true),
                description = state.route is Combat && state.ship.Get((Status)MainManifest.statuses["gold"].Id) >= goldCost
                    ? descriptionText
                    : $"<c=textFaint>{descriptionText}</c>"
            };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class MutualRespect : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var xEqualsEnemyHonor = MainManifest.KokoroApi.Actions.SetTargetPlayer(
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                false
            );

            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 2},
                    xEqualsEnemyHonor,
                    new AHonorShield() { amount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint = 1 },
                    new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
                };
            }

            if (upgrade == Upgrade.B)
            {
                var goldResource = MainManifest.KokoroApi.ActionCosts.StatusResource
                (
                    (Status)MainManifest.statuses["gold"].Id,
                    Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                    (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id,
                    (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
                );

                var continueAction = MainManifest.KokoroApi.Actions.MakeContinue(out var continueGuid);

                return new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 3),
                        continueAction
                    ),
                    MainManifest.KokoroApi.Actions.MakeContinued(continueGuid, xEqualsEnemyHonor),
                    MainManifest.KokoroApi.Actions.MakeContinued(continueGuid, new AHonorShield() { amount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint = 1 }),
                    MainManifest.KokoroApi.Actions.MakeContinued(continueGuid, new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 }),
                };
            }

            return new()
            {
                xEqualsEnemyHonor,
                new AHonorShield() { amount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint = 1 },
                new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = c.otherShip.Get((Status)MainManifest.statuses["honor"].Id), xHint=1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.B ? 1 : 2 };
        }
    }


    [CardMeta(rarity = Rarity.rare, upgradesTo = [ Upgrade.A, Upgrade.B ])]
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

            if (upgrade == Upgrade.A)
            {

                return new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 3),
                        new ADrawCard() { count = 1 }
                    ),
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 2),
                        new ADrawCard() { count = 1 }
                    )
                };
            }

            if (upgrade == Upgrade.B)
            {
                return new()
                {
                    MainManifest.KokoroApi.ActionCosts.Make
                    (
                        MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 8),
                        new ACardSelect
                        {
                            browseAction = new ChooseCardToPutInHand(),
                            browseSource = CardBrowse.Source.DrawOrDiscardPile,
                            filterUUID = uuid
                        }
                    )
                };
            }

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(goldResource, amount: 3),
                    new ADrawCard() { count = 1 }
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, infinite = true };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = [Upgrade.A, Upgrade.B])]
    public class GoldenScales : InvestmentCard
    {
        public override List<int> upgradeCosts => upgrade switch
        {
            Upgrade.None => new() { 3, 3 },
            Upgrade.A => new() { 3, 6 },
            Upgrade.B => new() { 1, 2 },
        };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            var actionTiers = new List<List<CardAction>>() {
                new() { new AGoldShield() { amount = upgrade == Upgrade.B ? 1 : 2 } },
                new() { new AHonorShield() { amount = upgrade == Upgrade.A ? 2 : 1 } },
                new() { new AStatus() { status = Status.shield, statusAmount = upgrade == Upgrade.B ? 1 : 2, targetPlayer = true } },
            };

            if (upgrade == Upgrade.A || upgrade == Upgrade.B)
            {
                var swap = actionTiers[2];
                actionTiers[2] = actionTiers[1];
                actionTiers[1] = swap;
            }

            return actionTiers;
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
