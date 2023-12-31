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
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = VowsController.VOW_OF_ADAMANCY_HONOR, targetPlayer = true },
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
               new AStatus() { status = Enum.Parse<Status>("powerdrive"), statusAmount = 1, targetPlayer = false },
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = true },
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
               },
               new AAttack() { damage = GetDmg(s, 2) }
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
            int cost = 3;
            bool canPay = s.ship.Get((Status)MainManifest.statuses["honor"].Id) >= cost;

            return new()
            {
               new AStatus() { disabled = !canPay, status = (Status)MainManifest.statuses["honor"].Id, statusAmount = -cost, targetPlayer = true },
               new AAttack() { disabled = !canPay, damage = GetDmg(s, 2), weaken = true }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 3 };
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
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = VowsController.VOW_OF_TEAMWORK_HONOR, targetPlayer = true },
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

            var honor = base.upgrade switch
            {
                Upgrade.None => VowsController.VOW_OF_POVERTY_HONOR,
                Upgrade.A => VowsController.VOW_OF_AFFLUENCE_HONOR,
                Upgrade.B => VowsController.VOW_OF_MIDDLING_INCOME_HONOR,
                _ => throw new Exception("Card was upgraded to an upgrade that doesn't exist!"),
            };

            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses[vowName].Id, statusAmount = 1, targetPlayer = true },
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = honor, targetPlayer = true },
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
               new AStatus() 
               { 
                   status = (Status)MainManifest.statuses["honor"].Id, 
                   statusAmount = base.upgrade == Upgrade.A ? VowsController.VOW_OF_REST_HONOR+1 : VowsController.VOW_OF_REST_HONOR, 
                   targetPlayer = true 
               },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
        }
    }
}
