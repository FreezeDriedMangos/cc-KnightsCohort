using KnightsCohort.Bannerlady.Midrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsCohort.actions;

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
            return new() { cost = 1 };
        }
    }

    //[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    //public class ForgeAhead : Card
    //{
    //    public override List<CardAction> GetActions(State s, Combat c)
    //    {
    //        return new()
    //        {
    //           new AMove() { dir = flipped ? -2 : 2, targetPlayer = true },
    //           new AStatus() { status = Enum.Parse<Status>("shield"), statusAmount = 1, targetPlayer = true }
    //        };
    //    }
    //    public override CardData GetData(State state)
    //    {
    //        return new() { cost = 1 };
    //    }
    //}

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
                new AAttack() { targetPlayer = false, damage = GetDmg(s, 1) },
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
            Guid stopId;
            return new()
            {
                new ASpawn() { thing = new MartyrBanner() },
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
                        amount: 2
                    ),
                    MainManifest.KokoroApi.Actions.MakeStop(out stopId)
                ),
                MainManifest.KokoroApi.Actions.MakeStopped(stopId, new AHurt(){ hurtAmount = 1, targetPlayer = true })
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
               //new AStatus() { status = Enum.Parse<Status>("shield"), statusAmount = 2, targetPlayer = true }
               new ARetreat() { distance = 2 }
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
            return new()
            {
                new ACharge() { distance = 1 },
                new AStatus() { status = Enum.Parse<Status>("shield"), statusAmount = 1, targetPlayer = true },
                new AAttack() { damage = GetDmg(s, 1), targetPlayer = false },
                new ACharge() { distance = 1 },
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
               new ASpawn() { thing = new MartyrBanner(), offset = -1 },
               new ASpawn() { thing = new MartyrBanner() },
               new ASpawn() { thing = new MartyrBanner(), offset = 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 3, exhaust = true };
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
               new AAttack() { damage = GetDmg(s, 1) },
               new ARetreat() { distance = 2 },
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
               new AStatus() { status = (Status)MainManifest.statuses["flurry"].Id, statusAmount = 1, targetPlayer = true }
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
               new ARetreat() { distance = 2 }
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
               new ASpawn() { thing = new WarBanner() },
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
               new ARetreat() { distance = 1 },
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
            return new() { cost = 3, description = "From every banner, launch a 1 damage attack towards the enemy and towards you." };
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
    public class BannerladyDesperateMeasures : Card
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
                        amount: 2
                    ),
                    new ADrawCard() { count = 3 }
                ),
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LeadFromTheFront : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AAttack() { damage = GetDmg(s, 1), targetPlayer = false },
                new ACharge() { distance = 2 },
                new AAttack() { damage = GetDmg(s, 1), targetPlayer = false },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BannerladyRally : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            CardAction upper =
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost
                    (
                        MainManifest.KokoroApi.ActionCosts.StatusResource
                        (
                            Enum.Parse<Status>("droneShift"),
                            Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                            (Spr)MainManifest.sprites["icons/droneshift_cost_unsatisfied"].Id!.Value, 
                            (Spr)MainManifest.sprites["icons/droneshift_cost"].Id!.Value
                        ),
                        amount: 1
                    ),
                    new AStatus() { disabled = this.flipped, status = Enum.Parse<Status>("evade"), statusAmount = 2, targetPlayer = true }
                );
            upper.disabled = this.flipped;

            CardAction lower =
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost
                    (
                        MainManifest.KokoroApi.ActionCosts.StatusResource
                        (
                            Enum.Parse<Status>("evade"),
                            Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.Player,
                            (Spr)MainManifest.sprites["icons/evade_cost_unsatisfied"].Id!.Value,
                            (Spr)MainManifest.sprites["icons/evade_cost"].Id!.Value
                        ),
                        amount: 1
                    ),
                    new AStatus() { disabled = !this.flipped, status = Enum.Parse<Status>("droneShift"), statusAmount = 2, targetPlayer = true }
                );
            lower.disabled = !this.flipped;

            return new()
            {
                upper,
                new ADummyAction(),
                new ADummyAction(),
                lower,
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, infinite = true };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LiftedBurdens : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
               new AStatus() { status = Enum.Parse<Status>("evade"), statusAmount = s.ship.Get((Status)MainManifest.statuses["honor"].Id), xHint = 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }


    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SharpEyes : Card
    {
        public int? GetDistance(State s)
        {
            if (s.route is not Combat c) return null;
            if (c.stuff.Count == 0) return null;

            int localCannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
            if (localCannonX == -1) return null;
            int cannonX = s.ship.x + localCannonX;

            return c.stuff.Keys.Select(x => x - cannonX).OrderBy(x => Math.Abs(x)).First();
        }
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int distance = GetDistance(s) ?? 0;
            return new()
            {
               new AMove() { dir = distance, targetPlayer = true }
            };
        }
        public override CardData GetData(State state)
        {
            string descr = "Align leftmost active cannon with nearest midrow object";
            int cost = 1;
            if (state.route is not Combat) return new() { cost = cost, description = $"{descr}." };


            int? distance = GetDistance(state);
            if (distance == null)
            {
                return new() { cost = cost, description = $"<c=3f3f3f>{descr}.</c>" };
            }

            string hint = $"(Right {distance})";
            if (distance < 0) hint = $"(Left {-distance})";

            return new() { cost = cost, description = $"{descr} {hint}." };
        }
    }
}
