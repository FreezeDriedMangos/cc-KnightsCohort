﻿using KnightsCohort.Bannerlady.Midrow;
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
            if (upgrade == Upgrade.B)
            {
                return new()
                {
                   new ASpawn() { thing = new TatteredWarBanner() },
                   new ADroneMove() { dir = 1 },
                   new ASpawn() { thing = new TatteredWarBanner() },
                   new AStatus() { status = Enum.Parse<Status>("droneShift"), statusAmount = 1, targetPlayer = true }
                };
            }

            return new()
            {
                new ASpawn() { thing = new TatteredWarBanner() },
                new AStatus() { status = Enum.Parse<Status>("droneShift"), statusAmount = upgrade == Upgrade.A ? 3 :2, targetPlayer = true }
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
               new ASpawn() { thing = upgrade == Upgrade.A ? new TatteredMercyBanner() : new MercyBanner() },
               new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = upgrade == Upgrade.B ? 2 : 1, targetPlayer = true }
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
            if (upgrade == Upgrade.B)
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
                            amount: 1
                        ),
                        new ASpawn() { thing = new TatteredMartyrBanner() }
                    ),
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
                        new ASpawn() { thing = new TatteredMartyrBanner(), offset = 1 }
                    ),
                };
            }


            return new()
            {
                new ASpawn() { thing = new TatteredMartyrBanner() },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 0 : 1 };
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
               new AStatus() { status = Enum.Parse<Status>("droneShift"), statusAmount = upgrade == Upgrade.B ? 4 : 2, targetPlayer = true },
               new ARetreat() { dir = upgrade == Upgrade.A ? 4 : 2 }
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
                new ACharge() { dir = upgrade == Upgrade.A ? 2 : 1 },
                new AStatus() { status = Enum.Parse<Status>("shield"), statusAmount = upgrade == Upgrade.B ? 2 : 1, targetPlayer = true },
                new AAttack() { damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1), targetPlayer = false },
                new ACharge() { dir = upgrade == Upgrade.A ? 2 : 1 },
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
            List<CardAction> retval = new()
            {
               new ASpawn() { thing = upgrade == Upgrade.A ? new TatteredMercyBanner() : new MercyBanner(), offset = -1 },
               new ASpawn() { thing = new WarBanner() },
               new ASpawn() { thing = upgrade == Upgrade.A ? new TatteredMercyBanner() : new MercyBanner(), offset = 1 },
            };

            if (upgrade == Upgrade.B)
            {
                retval.Insert(0, new ASpawn() { thing = new TatteredWarBanner(), offset = -2 });
                retval.Add(new ASpawn() { thing = new TatteredWarBanner(), offset = 2 });
            }

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Martyrdom : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.B)
            {
                return new()
                {
                   new ASpawn() { thing = new MartyrBanner(), offset = -1 },
                   new ASpawn() { thing = new MartyrBanner() },
                };
            }
            else
            {
                return new()
                {
                   new ASpawn() { thing = new MartyrBanner() },
                };
            }
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 2 : 3, exhaust = true };
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
               new ASpawn() { thing = upgrade == Upgrade.B ? new WarBanner() : new TatteredWarBanner(), offset = 0 },
               new ASpawn() { thing = new TatteredWarBanner(), offset = 1 },
               new ASpawn() { thing = new TatteredWarBanner(), offset = 2 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 1 : 2 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CoveredRetreat : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AAttack() { damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1) },
               new ARetreat() { dir = upgrade == Upgrade.A ? 3 : 2 },
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

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FalseFlag : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ASpawn() { thing = upgrade == Upgrade.A ? new TatteredPirateBanner() : new PirateBanner() },
               new ARetreat() { dir = 2 }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = upgrade == Upgrade.B ? false : true };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ShieldOfFaith : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AStatus() { status = (Status)MainManifest.statuses["shieldOfFaith"].Id, targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 2 : 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 1 : 2 };
        }
    }

    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RaiseMorale : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
               new ASpawn() { thing = new WarBanner(), offset = -1 },
               new ASpawn() { thing = new WarBanner(), offset = 1 },
            };

            if (upgrade == Upgrade.B) retval.Insert(1, new ASpawn() { thing = new BannerOfShielding() });

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 1 : 2, exhaust = true };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CoverMe : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.B)
            {
                return new()
                {
                   new ASpawn() { thing = new BannerOfShielding() },
                   new ACharge() { dir = 1 },
                   new ASpawn() { thing = new BannerOfShielding() },
                };
            }

            return new()
            {
               new ASpawn() { thing = new BannerOfShielding() },
               new ARetreat() { dir = upgrade == Upgrade.A ? 3 : 1 },
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
                    damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1),
                    fromDroneX = kvp.Key,
                    targetPlayer = false
                });
                incomingAttacks.Add(new AAttack()
                {
                    damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1),
                    fromDroneX = kvp.Key,
                    targetPlayer = true
                });
            }

            if (upgrade != Upgrade.A) outgoingAttacks.AddRange(incomingAttacks);
            return outgoingAttacks;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = upgrade switch
                {
                    Upgrade.None => $"From every banner, launch a {GetDmg(state, 1)} damage attack towards the enemy and towards you.",
                    Upgrade.A => $"From every banner, launch a {GetDmg(state, 1)} damage attack towards the enemy.",
                    Upgrade.B => $"From every banner, launch a {GetDmg(state, 2)} damage attack towards the enemy and towards you.",
                }
            };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class DiplomaticImmunity : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.B) return new() { new AStatus() { status = Enum.Parse<Status>("autododgeLeft"), targetPlayer = true, statusAmount = 1 } };

            return new()
            {
               new AStatus() { status = Enum.Parse<Status>("shield"), targetPlayer = true, statusAmount = 2 },
               new AStatus() { status = Enum.Parse<Status>("autododgeRight"), targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 2 : 1 },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 2, exhaust = upgrade == Upgrade.B ? true : false };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class BannerladyDesperateMeasures : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (upgrade == Upgrade.B)
            {
                int cost = 3;
                bool disable = s.ship.Get((Status)MainManifest.statuses["honor"].Id) < cost;
                Guid continueId;
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
                        MainManifest.KokoroApi.Actions.MakeContinue(out continueId)
                    ),
                    MainManifest.KokoroApi.Actions.MakeContinued(continueId, new ADrawCard() { disabled = disable, count = 3 }),
                    MainManifest.KokoroApi.Actions.MakeContinued(continueId, new AStatus() { disabled = disable, status = Enum.Parse<Status>("drawNextTurn"), targetPlayer = true, statusAmount = 3 })
                };
            }

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
            return new() { cost = upgrade == Upgrade.A ? 0 : 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LeadFromTheFront : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
                new ACharge() { dir = 2 },
                new AAttack() { damage = GetDmg(s, 1), targetPlayer = false },
            };

            retval.Insert(upgrade == Upgrade.A ? 2 : 0, new AAttack() { damage = GetDmg(s, 1), targetPlayer = false });

            if (upgrade == Upgrade.B) retval.Insert(0, new ACharge() { dir = 2 });

            return retval;
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
            int costs = upgrade == Upgrade.A ? 2 : 1;
            int gives = upgrade == Upgrade.A ? 1 : 2;

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
                        amount: costs
                    ),
                    new AStatus() { disabled = this.flipped, status = Enum.Parse<Status>("evade"), statusAmount = gives, targetPlayer = true }
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
                        amount: costs
                    ),
                    new AStatus() { disabled = !this.flipped, status = Enum.Parse<Status>("droneShift"), statusAmount = gives, targetPlayer = true }
                );
            lower.disabled = !this.flipped;

            List<CardAction> retval = new()
            {
                upper,
                new ADummyAction(),
                lower,
            };

            if (upgrade == Upgrade.B)
            {
                retval.Add(new AStatus() { disabled = !this.flipped, status = Enum.Parse<Status>("droneShift"), statusAmount = 1, targetPlayer = true });
                retval.Insert(1, new AStatus() { disabled = this.flipped, status = Enum.Parse<Status>("evade"), statusAmount = 1, targetPlayer = true });
            }

            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = upgrade == Upgrade.A ? 0 : 1, infinite = true, floppable = true };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LiftedBurdens : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int honor = s.ship.Get((Status)MainManifest.statuses["honor"].Id);
            int cost = upgrade switch
            {
                Upgrade.None => 2,
                Upgrade.A => 1,
                Upgrade.B => 2,
            };
            int max = upgrade == Upgrade.B ? 6 : 4;

            return new()
            {
                new AVariableHint() { status = (Status)MainManifest.statuses["honor"].Id },
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
                    new AStatus() { status = Enum.Parse<Status>("evade"), statusAmount = Math.Min(max, honor), xHint = 1, targetPlayer = true, disabled = honor < cost }
                ),
                new AText() { text = $"(max {max})" }
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
            string descr = "Align leftmost cannon with nearest midrow";
            int cost = upgrade == Upgrade.A ? 0 : 1;
            bool retain = upgrade == Upgrade.B;
            if (state.route is not Combat) return new() { cost = cost, description = $"{descr}.", retain = retain };


            int? distance = GetDistance(state);
            if (distance == null)
            {
                return new() { cost = cost, description = $"<c=3f3f3f>{descr}.</c>", retain = retain };
            }

            string hint = $"(Right {distance})";
            if (distance < 0) hint = $"(Left {-distance})";

            return new() { cost = cost, description = $"{descr} {hint}.", retain = retain };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CavalryCharge : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> retval = new()
            {
                new ACharge() { dir = upgrade == Upgrade.A ? 2 : 1 },
                new AAttack() { damage = GetDmg(s, 1) },
                new ACharge() { dir = upgrade == Upgrade.A ? 2 : 1 },
                new AAttack() { damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1) },
                new ARetreat() { dir = upgrade == Upgrade.A ? 4 : 2 }
            };
            return retval;
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }
}
