using KnightsCohort.Bannerlady.Midrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Bannerlady.Cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RememberedGlory : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new TatteredWarBanner() },
               new AStatus() { status = Enum.Parse<Status>("droneShift"), statusAmount = 2, targetPlayer = true }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LeadFromTheFront : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AMove() { dir = flipped ? -2 : 2, targetPlayer = true },
               new AStatus() { status = Enum.Parse<Status>("shield"), statusAmount = 1, targetPlayer = true }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BannerladyPity : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new MercyBanner() },
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = true }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BowAndArrow : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new ArrowMissile() },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BannerladyBodyguard : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new TatteredMartyrBanner() },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RestAndReprieve : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new MercyBanner() },
               new AStatus() { status = Enum.Parse<Status>("droneShift"), statusAmount = 2, targetPlayer = true },
               new AStatus() { status = Enum.Parse<Status>("shield"), statusAmount = 2, targetPlayer = true }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ValiantCharge : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            // TODO: make sure this can't be flipped
            return new()
            {
               new AMove() { dir = Math.Sign(c.otherShip.x - s.ship.x), targetPlayer = true },
               new AStatus() { status = Enum.Parse<Status>("droneShift"), statusAmount = 2, targetPlayer = true },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ArcheryTraining : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new ArrowMissile() },
               new AMove() { dir = 1, targetPlayer = true },
               new ASpawn() { thing = new ArrowMissile() },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class PlatemailPiercer : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new BroadheadArrowMissile() },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BannerladyTelegraph : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new MercyBanner(), offset = -1 },
               new ASpawn() { thing = new WarBanner() },
               new ASpawn() { thing = new MercyBanner(), offset = 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 3 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Martyrdom : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new MartyrBanner() }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class IsItWar : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new TatteredWarBanner(), offset = -2 },
               new ASpawn() { thing = new TatteredWarBanner(), offset = -1 },
               new ASpawn() { thing = new TatteredWarBanner(), offset = 0 },
               new ASpawn() { thing = new TatteredWarBanner(), offset = 1 },
               new ASpawn() { thing = new TatteredWarBanner(), offset = 2 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 3 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CoveredRetreat : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AMove() { dir = -2, targetPlayer = true },
               new AAttack() { damage = GetDmg(s, 1) }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class MasterOfArchery : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["flurry"].Id, statusAmount = 1 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 3 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FalseFlag : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new PirateBanner() },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ShieldOfFaith : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["shieldOfFaith"].Id, targetPlayer = true, statusAmount = 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RaiseMorale : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new BannerOfInspiration() },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CoverMe : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = new BannerOfShielding() },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class DeadlyConviction : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> outgoingAttacks = new();
            List<CardAction> incomingAttacks = new();
            foreach (var kvp in c.stuff)
            {
                if (kvp.Value is not Banner) continue;
                outgoingAttacks.Add(new AAttack()
                {
                    damage = 1,
                    fromDroneX = kvp.Key,
                    targetPlayer = false
                });
                incomingAttacks.Add(new AAttack()
                {
                    damage = 1,
                    fromDroneX = kvp.Key,
                    targetPlayer = true
                });
            }

            outgoingAttacks.AddRange(incomingAttacks);
            return outgoingAttacks;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 3 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class DiplomaticImmunity : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("shield"), targetPlayer = true, statusAmount = 2 },
               new AStatus() { status = Enum.Parse<Status>("autododgeRight"), targetPlayer = true, statusAmount = 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class DesperateMeasures : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            bool disabled = s.ship.Get((Status)MainManifest.statuses["honor"].Id) < 1;
            return new()
            {
               new AStatus() { disabled = disabled, status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = true, statusAmount = -1 },
               new ADrawCard() { disabled = disabled, count = 3 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }
}
