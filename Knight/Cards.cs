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
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfMercy"].Id, statusAmount = 1, targetPlayer = true },
               new AStatus() { status = Enum.Parse<Status>("tempShield"), statusAmount = 2, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class OffhandWeapon : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
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
               new AStatus() { status = Enum.Parse<Status>("evade"), statusAmount = 1, targetPlayer = true },
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
            return new()
            {
               new ASpawn()
               {
                   thing = new Sword
                   {
                       targetPlayer = s.ship.Get(Enum.Parse<Status>("backwardsMissiles")) > 0
                   }
               }
            };
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
            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("evade"), statusAmount = 2, targetPlayer = true },
               new AStatus() { status = Enum.Parse<Status>("tempShield"), statusAmount = 2, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UnmovingFaith : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfAdamancy"].Id, statusAmount = 1, targetPlayer = true },
            };
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
            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("overdrive"), statusAmount = 2, targetPlayer = false },
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 2, targetPlayer = true },
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
            return new()
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
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 3 };
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
                        amount: 3
                    ),
                    new AAttack() { damage = GetDmg(s, 2), weaken = true }
                ),
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Teamwork : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfTeamwork"].Id, statusAmount = 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
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
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfRest"].Id, statusAmount = 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class UnrelentingOath : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfAction"].Id, statusAmount = 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Truce : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["vowOfCourage"].Id, statusAmount = 2, targetPlayer = true },
               new AEndTurn()
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ShieldBash : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int shield = s.ship.Get(Enum.Parse<Status>("shield"));
            int direction = flipped ? -1 : 1;

            return new()
            {
                new AVariableHint() { status = Enum.Parse<Status>("shield") },
                new AAttack() { damage = GetDmg(s, shield), xHint = 1, moveEnemy = direction*shield },
                new AStatus() { status = Enum.Parse<Status>("shield"), mode = Enum.Parse<AStatusMode>("Set"), statusAmount = 0, targetPlayer = true }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FreeHit : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            bool disabled = s.ship.Get((Status)MainManifest.statuses["honor"].Id) <= 0;
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
                        amount: 1
                    ),
                    new AStatus() { disabled = disabled, status = (Status)MainManifest.statuses["vowOfCourage"].Id, statusAmount = 1, targetPlayer = true }
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
            return new()
            {
                new AStatus() { status = (Status)MainManifest.statuses["vowOfLeft"].Id, statusAmount = 1, targetPlayer = true },
            };
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
                new AStatus() { status = (Status)MainManifest.statuses["vowOfChivalry"].Id, statusAmount = 1, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
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
            return new() { cost = 1, description = "Move opponent ship to align with your ship." };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ShieldOfHonor : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
                new AStatus() { status = Enum.Parse<Status>("shield") }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = "Move opponent ship to align with your ship." };
        }
    }
}
