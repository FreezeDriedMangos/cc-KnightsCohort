using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using KnightsCohort.Knight.Midrow;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Knight.Cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FightingChance : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
               new AStatus() { disabled = s.route is Combat && VowsController.attackedThisTurn, status = (Status)MainManifest.statuses["vowOfMercy"].Id, statusAmount = (upgrade == Upgrade.B ? 2 : 1), targetPlayer = true },
               new AStatus() { status = Enum.Parse<Status>("tempShield"), statusAmount = 2, targetPlayer = true },
            };

            //if (upgrade == Upgrade.A) retval.Add(new AStatus() { status = Enum.Parse<Status>("evade"), statusAmount = 2, targetPlayer = true });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 0 : 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class OffhandWeapon : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.B)
            {
                return new()
                {
                   new ASpawn()
                   {
                       thing = new Dagger
                       {
                           targetPlayer = s.ship.Get(Enum.Parse<Status>("backwardsMissiles")) > 0
                       }
                   },
                   new AMove() { targetPlayer = true, dir = 1 },
                   new ASpawn()
                   {
                       thing = new Dagger
                       {
                           targetPlayer = s.ship.Get(Enum.Parse<Status>("backwardsMissiles")) > 0
                       }
                   },
                };
            }

            return new()
            {
               new ASpawn()
               {
                   thing = new Dagger
                   {
                       targetPlayer = s.ship.Get(Enum.Parse<Status>("backwardsMissiles")) > 0
                   }
               },
               new AStatus() { status = Enum.Parse<Status>("evade"), statusAmount = upgrade == Upgrade.A ? 2 : 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Claymore : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
               new ASpawn()
               {
                   thing = new Sword
                   {
                       targetPlayer = s.ship.Get(Enum.Parse<Status>("backwardsMissiles")) > 0,
                       bubbleShield = upgrade == Upgrade.A
                   }
               }
            };

            if (upgrade == Upgrade.B)
            {
                retval.Insert(0, new ASpawn()
                {
                    thing = new Dagger
                    {
                        targetPlayer = s.ship.Get(Enum.Parse<Status>("backwardsMissiles")) > 0
                    }
                });
                retval.Insert(1, new ADroneMove() { dir = 1 });
            }

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Footwork : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
               new AStatus() { status = Enum.Parse<Status>("evade"), statusAmount = 2, targetPlayer = true },
               new AStatus() { status = Enum.Parse<Status>("tempShield"), statusAmount = 2, targetPlayer = true },
            };

            if (upgrade == Upgrade.B) retval.Add(new AStatus() { status = Enum.Parse<Status>("droneShift"), statusAmount = 2, targetPlayer = true });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 1 : 2 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UnmovingFaith : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int vowStacks = upgrade == Upgrade.B ? 2 : 1;
            List<CardAction> retval = new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfAdamancy"].Id, statusAmount = vowStacks, targetPlayer = true },
            };
            
            if (upgrade != Upgrade.A) retval.Add(new AStatus() { targetPlayer = true, statusAmount = 2, status = Enum.Parse<Status>("tempShield") });

            if (upgrade == Upgrade.A) retval.Add(new AStatus() { targetPlayer = true, statusAmount = 2, status = Enum.Parse<Status>("shield") });
            if (upgrade == Upgrade.B) retval.Add(new AStatus() { targetPlayer = true, statusAmount = 0, status = Enum.Parse<Status>("evade"), mode = AStatusMode.Set });


            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FixYourForm : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int overdrive = upgrade switch
            {
                Upgrade.None => 2,
                Upgrade.A => 1,
                Upgrade.B => 3,
            };
            int honor = upgrade switch
            {
                Upgrade.None => 2,
                Upgrade.A => 2,
                Upgrade.B => 3,
            };

            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = overdrive, targetPlayer = false },
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = honor, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Excalibur : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
               new ASpawn()
               {
                   thing = new ExcaliburMissile
                   {
                       targetPlayer = s.ship.Get(Enum.Parse<Status>("backwardsMissiles")) > 0,
                       bubbleShield = true
                   }
               }
            };

            if (upgrade == Upgrade.B) retval.Insert(0, new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 2, targetPlayer = true });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 2 : 3 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CheapShot : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost
                    (
                        MainManifest.KokoroApi.ActionCosts.StatusResource
                        (
                            (Status)MainManifest.statuses["honor"].Id,
                            Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                            (Spr)MainManifest.sprites["icons/honor_cost_unsatisfied"].Id,
                            (Spr)MainManifest.sprites["icons/honor_cost"].Id
                        ),
                        amount: upgrade switch {
                            Upgrade.None => 3,
                            Upgrade.A => 2,
                            Upgrade.B => 1,
                        }
                    ),
                    new AAttack() { damage = GetDmg(s, 2), weaken = true }
                ),
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2, exhaust = upgrade == Upgrade.B };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Teamwork : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfTeamwork"].Id, statusAmount = upgrade == Upgrade.B ? 2 : 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = upgrade != Upgrade.A };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class HonorableStrike : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
               new AAttack() { damage = GetDmg(s, s.ship.Get((Status)MainManifest.statuses["honor"].Id)), xHint = 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 3 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FinancialAdvice : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var vowName = base.upgrade switch
            {
                Upgrade.None => "vowOfPoverty",
                Upgrade.A => "vowOfAffluence",
                Upgrade.B => "vowOfMiddlingIncome",
                _ => throw new Exception("Card was upgraded to an upgrade that doesn't exist!"),
            };

            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses[vowName].Id, statusAmount = 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class KnightsRest : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfRest"].Id, statusAmount = 1, targetPlayer = true },
            };
            if (upgrade == Upgrade.B) retval.Add(new AStatus() { targetPlayer = true, statusAmount = 1, status = (Status)MainManifest.statuses["vowOfMegaRest"].Id });
            if (upgrade == Upgrade.A) retval.Add(new AStatus() { targetPlayer = true, statusAmount = 1, status = Enum.Parse<Status>("energyNextTurn") });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = (upgrade == Upgrade.B ? true : false) };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UnrelentingOath : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.B)
            {
                return new()
                {
                   new AStatus() { status = (Status)MainManifest.statuses["vowOfAction"].Id, statusAmount = 2, targetPlayer = true },
                   new AStatus() { status = Enum.Parse<Status>("shield"), statusAmount = 0, mode = AStatusMode.Set, targetPlayer = true },
                };
            }
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfAction"].Id, statusAmount = 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, retain = upgrade == Upgrade.A };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Truce : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfCourage"].Id, statusAmount = 2, targetPlayer = true },
            };

            if (upgrade == Upgrade.B) retval.Add(new AStatus() { disabled = s.route is Combat && VowsController.attackedThisTurn, status = (Status)MainManifest.statuses["vowOfMercy"].Id, statusAmount = 1, targetPlayer = true });
            if (upgrade != Upgrade.A) retval.Add(new AEndTurn());

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2, exhaust = upgrade == Upgrade.A ? true : false };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ShieldBash : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int shield = s.ship.Get(Enum.Parse<Status>("shield"));
            int direction = flipped ? 1 : -1;

            List<CardAction> retval = new()
            {
                new AVariableHint() { status = Enum.Parse<Status>("shield") },
                new AAttack() { damage = GetDmg(s, shield), xHint = 1 },
                new AMove() { dir = direction*shield, xHint = 1, targetPlayer = false },
            };

            if (upgrade != Upgrade.A) retval.Add(new AStatus() { status = Enum.Parse<Status>("shield"), mode = Enum.Parse<AStatusMode>("Set"), statusAmount = 0, targetPlayer = true });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2, flippable = upgrade == Upgrade.B };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FreeHit : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.A)
            {
                return new()
                {
                    new AStatus() { status = (Status)MainManifest.statuses["vowOfCourage"].Id, statusAmount = 1, targetPlayer = true }
                };
            }

            bool disabled = s.ship.Get((Status)MainManifest.statuses["honor"].Id) <= 0;
            int cost = upgrade == Upgrade.B ? 2 : 1;
            int vow = upgrade == Upgrade.B ? 2 : 1;
            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost
                    (
                        MainManifest.KokoroApi.ActionCosts.StatusResource
                        (
                            (Status)MainManifest.statuses["honor"].Id,
                            Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                            (Spr)MainManifest.sprites["icons/honor_cost_unsatisfied"].Id,
                            (Spr)MainManifest.sprites["icons/honor_cost"].Id
                        ),
                        amount: cost
                    ),
                    new AStatus() { disabled = disabled, status = (Status)MainManifest.statuses["vowOfCourage"].Id, statusAmount = vow, targetPlayer = true }
                ),
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Handicap : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
                new AStatus() { status = (Status)MainManifest.statuses[upgrade == Upgrade.A ? "vowOfRight" : "vowOfLeft"].Id, statusAmount = 1, targetPlayer = true },
            };

            if (upgrade == Upgrade.B) retval.Add(new AStatus() { status = (Status)MainManifest.statuses["vowOfRight"].Id, statusAmount = 1, targetPlayer = true });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Oathbreaker : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["oathbreaker"].Id, statusAmount = 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FriendlyDuel : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["vowOfChivalry"].Id, statusAmount = upgrade == Upgrade.B ? 2 : 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = upgrade != Upgrade.A };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Challenge : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AMove() { targetPlayer = false, dir = s.ship.x - c.otherShip.x },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 0 : 1, retain = upgrade == Upgrade.B, description = "Move opponent ship to align with your ship." };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ShieldOfHonor : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                new AStatus() { status = Enum.Parse<Status>("shield"), xHint = 1, targetPlayer = true, statusAmount = s.ship.Get((Status)MainManifest.statuses["honor"].Id) }
            };

            if (upgrade == Upgrade.A) retval.Add(new AStatus() { status = Enum.Parse<Status>("tempShield"), statusAmount = 2, targetPlayer = true });
            if (upgrade == Upgrade.B) retval.Insert(0, new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = true });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }
}
