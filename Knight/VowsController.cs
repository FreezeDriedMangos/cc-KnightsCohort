using HarmonyLib;
using KnightsCohort.Herbalist.Artifacts;
using KnightsCohort.Knight.Artifacts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KnightsCohort.Knight
{
    [HarmonyPatch]
    public static class VowsController
    {
        public static readonly int VOW_OF_MERCY_HONOR = 1;
        public static readonly int VOW_OF_COURAGE_HONOR = 1;

        public static bool StatusIsVow(Status status) =>
            status == (Status)MainManifest.statuses["vowOfMercy"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfAdamancy"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfTeamwork"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfAction"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfCourage"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfLeft"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfRight"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfChivalry"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfRest"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfMegaRest"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfPoverty"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfMiddlingIncome"].Id!.Value ||
            status == (Status)MainManifest.statuses["vowOfAffluence"].Id!.Value;


        public static void AddHonor(Ship ship, int amt)
        {
            var honor = ship.Get((Status)MainManifest.statuses["honor"].Id);
            ship.Set((Status)MainManifest.statuses["honor"].Id, honor + amt);
        }

        public static int GetAndClear(Ship ship, string statusName)
        {
            int retval = ship.Get((Status)MainManifest.statuses[statusName].Id);
            ship.Set((Status)MainManifest.statuses[statusName].Id, 0);
            return retval;
        }
        public static int GetAndDecrement(Ship ship, string statusName)
        {
            int retval = ship.Get((Status)MainManifest.statuses[statusName].Id);
            ship.Set((Status)MainManifest.statuses[statusName].Id, Math.Max(0, retval-1));
            return retval;
        }
        public static Part? AAttackPostfix_GetHitShipPart(AAttack aattack, State s, Combat c)
        {
            Ship targetShip = (aattack.targetPlayer ? s.ship : c.otherShip);
            Ship attackingShip = (aattack.targetPlayer ? c.otherShip : s.ship);
            if (targetShip == null || attackingShip == null || targetShip.hull <= 0 || (aattack.fromDroneX.HasValue && !c.stuff.ContainsKey(aattack.fromDroneX.Value)))
            {
                return null;
            }
            int? num = Bannerlady.Banner.AAttackGetFromX(aattack, s, c);
            RaycastResult raycastResult = aattack.fromDroneX.HasValue 
                ? CombatUtils.RaycastGlobal(c, targetShip, fromDrone: true, aattack.fromDroneX.Value) 
                : (num.HasValue ? CombatUtils.RaycastFromShipLocal(s, c, num.Value, aattack.targetPlayer) : null);

            // autododge has already been applied so no need to check for it
            if (raycastResult != null && raycastResult.hitShip)
            {
                return targetShip.GetPartAtWorldX(raycastResult.worldX);
            }

            return null;
        }

        //
        // SHARED
        //

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.Set))]
        public static bool HarmonyPrefix_VowLimits(Ship __instance, Status status, ref int n)
        {
            if (!StatusIsVow(status)) return true;

            var max = g!=null && g.state.EnumerateAllArtifacts().Where(a => a is HolyGrail).Any() ? 3 : 2;
            if (n > max) n = max;
            if (n < 0) n = 0;

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
        public static void HarmonyPostfix_VowOfCourage_Cleanup(Ship __instance, State s, Combat c)
        {
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfAdamancy"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfChivalry"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfRight"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfLeft"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfTeamwork"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfAction"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfPoverty"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfAffluence"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfMiddlingIncome"].Id));
            AddHonor(__instance, __instance.Get((Status)MainManifest.statuses["vowOfRest"].Id));
            AddHonor(__instance, 2* __instance.Get((Status)MainManifest.statuses["vowOfMegaRest"].Id));

            GetAndDecrement(__instance, "vowOfCourage");
        }

        public static void LostVowEffect(G g, Rect shipRect, Spr vowSpr)
        {
            // make vow icon appear
            PFX.combatAlpha.Add(new Particle
            {
                pos = new Vec(shipRect.x, shipRect.y) + new Vec(shipRect.w * 0.5, shipRect.h * 0.2),
                lifetime = 1,
                size = 10,
                color = new Color(1.0, 1.0, 1.0, 0.7),
                dragCoef = 1.0,
                dragVel = new Vec(0.0, -10.0),
                sprite = vowSpr
            });

            // spark effect
            for (int i = 0; i < 10; i++)
            {
                PFX.combatSparks.MakeSparkBounds(shipRect, Mutil.NextRand(), Mutil.NextRand(), 0.0);
            }

            //smoke particles
            //for (int j = 0; j < 20; j++)
            //{
            //    double size = 3.0 + Mutil.NextRand() * 8.0;
            //    Vec pos = new Vec(shipRect.x, shipRect.y) + new Vec(shipRect.w * Mutil.NextRand(), shipRect.h * Mutil.NextRand());
            //    PFX.combatAlpha.Add(new Particle
            //    {
            //        pos = pos,
            //        lifetime = 2.0 * Mutil.NextRand(),
            //        size = size,
            //        color = new Color(1.0, 1.0, 1.0, 0.2),
            //        dragCoef = 1.0,
            //        dragVel = new Vec(0.0, -10.0)
            //    });
            //}
            //for (int k = 0; k < 3; k++)
            //{
            //    double size2 = 3.0 + Mutil.NextRand() * 3.0;
            //    Vec pos2 = new Vec(shipRect.x, shipRect.y) + new Vec(shipRect.w * Mutil.NextRand(), shipRect.h * Mutil.NextRand());
            //    PFX.combatExplosion.Add(new Particle
            //    {
            //        pos = pos2,
            //        lifetime = 2.0 * Mutil.NextRand(),
            //        size = size2,
            //        dragCoef = 1.0,
            //        dragVel = new Vec(0.0, -10.0)
            //    });
            //}
        }

        public static G g;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(G), nameof(G.Render))]
        public static void CaptureG(G __instance, double deltaTime) { if (g == null) g = __instance; }

        //
        // Vow of Adamancy & Left/Right
        //

        private static int previousX = 0;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AEnemyTurn), nameof(AEnemyTurn.Begin))]
        public static void HarmonyPostfix_VowOfAdamancy_Setup(G g, State s, Combat c)
        {
            previousX = s.ship.x;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.Set))]
        public static void HarmonyPostfix_VowOfAdamancy_SetupCleanup(Ship __instance, Status status, int n)
        {
            if (status != (Status)MainManifest.statuses["vowOfAdamancy"].Id) return;
            if (__instance.Get((Status)MainManifest.statuses["vowOfAdamancy"].Id) != 0) return;
            if (!__instance.isPlayerShip) return;

            // if we're gaining vow of adamancy for the first time right now, reset the ship's tracked position
            previousX = __instance.x;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.DrainCardActions))]
        public static void HarmonyPostfix_VowOfAdamancy(Combat __instance, G g)
        {
            //bool actionJustEnded = __instance.currentCardAction != null && __instance.currentCardAction.timer <= 0.0;
            //if (!actionJustEnded) return;

            if (previousX != g.state.ship.x)
            {
                // we've moved in general
                previousX = g.state.ship.x;
                var oldStacks = GetAndClear(g.state.ship, "vowOfAdamancy");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_adamancy"].Id);
            }

            if (previousX < g.state.ship.x)
            {
                // we've moved right
                var oldStacks = GetAndClear(g.state.ship, "vowOfLeft");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_left"].Id);
            }

            if (previousX > g.state.ship.x)
            {
                // we've moved left
                var oldStacks = GetAndClear(g.state.ship, "vowOfRight");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_right"].Id);
            }
        }

        private static int cardIndex = -1;
        private static int handSize = -1;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static bool HarmonyPostfix_VowOfRightLeft_Setup(Combat __instance, State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            cardIndex = __instance.hand.FindIndex((Card c) => c.uuid == card.uuid);
            handSize = __instance.hand.Count();
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void HarmonyPostfix_VowOfRightLeft_Enforcement(ref bool __result, State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            if (!__result) return;

            if (cardIndex == handSize-1)
            {
                // we've played the rightmost card
                var oldStacks = GetAndClear(s.ship, "vowOfLeft");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_left"].Id);
            }

            if (cardIndex == 0)
            {
                // we've played the leftmost card
                var oldStacks = GetAndClear(s.ship, "vowOfRight");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_right"].Id);
            }
        }

        //
        // Vow of Mercy
        //

        static bool attackedThisTurn = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AEnemyTurn), nameof(AEnemyTurn.Begin))]
        public static void HarmonyPostfix_VowOfMercy_Cleanup(G g, State s, Combat c)
        {
            // to handle player gaining Vow of Mercy on the first turn of combat, when the previous combat ended with them attacking
            attackedThisTurn = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAfterPlayerTurn), nameof(AAfterPlayerTurn.Begin))]
        public static void HarmonyPostfix_VowOfMercy_Concequences(G g, State s, Combat c)
        {
            var mercyVow = GetAndDecrement(g.state.ship, "vowOfMercy");
            if (!attackedThisTurn)
            {
                AddHonor(g.state.ship, VOW_OF_MERCY_HONOR * mercyVow);
                g.state.ship.PulseStatus((Status)MainManifest.statuses["vowOfMercy"].Id);
            }
            attackedThisTurn = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.DrainCardActions))]
        public static void HarmonyPostfix_VowOfMercy_AttackTracker(Combat __instance, G g)
        {
            bool actionJustEnded = __instance.currentCardAction != null && __instance.currentCardAction.timer <= 0.0;
            if (!actionJustEnded) return;

            if (__instance.currentCardAction is AAttack aattack && !aattack.targetPlayer && !aattack.fromDroneX.HasValue)
            {
                attackedThisTurn = true;
                int oldStacks = GetAndClear(g.state.ship, "vowOfMercy");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_mercy"].Id);
            }
        }

        //
        // Vow of Teamwork
        //

        static HashSet<Deck> cardColorsPlayedThisTurn = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AEnemyTurn), nameof(AEnemyTurn.Begin))]
        public static void HarmonyPostfix_VowOfTeamwork_Cleanup(G g, State s, Combat c)
        {
            cardColorsPlayedThisTurn.Clear();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void HarmonyPostfix_VowOfTeamwork_Enforcement(ref bool __result, State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            if (!__result) return;

            var deck = card.GetMeta().deck;
            if (!NewRunOptions.allChars.Contains(deck)) return; // don't prevent player from playing trash or things like Spent Casings from that one custom ship

            if (cardColorsPlayedThisTurn.Contains(deck)) // check to see if we've just broken the vow or if we already have
            {
                var teamworkStacks = GetAndClear(s.ship, "vowOfTeamwork");
                if (teamworkStacks > 0)
                {
                    LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_teamwork"].Id);
                }
                cardColorsPlayedThisTurn.Clear();
            }

            if (s.ship.Get((Status)MainManifest.statuses["vowOfTeamwork"].Id) > 0) cardColorsPlayedThisTurn.Add(deck);
        }

        //
        // Oathbreaker
        //

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.Set))]
        public static bool HarmonyPrefix_Oathbreaker(Ship __instance, Status status, int n)
        {
            if (status != (Status)MainManifest.statuses["honor"].Id) return true;

            int diff = n - __instance.Get(status);
            if (diff > 0) return true;

            __instance.Set(Enum.Parse<Status>("tempShield"), (-diff) + __instance.Get(Enum.Parse<Status>("tempShield")));

            return true;
        }

        //
        // Vow of Action
        //

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.Set))]
        public static bool HarmonyPrefix_VowOfAction(Ship __instance, Status status, int n)
        {
            if (status != Enum.Parse<Status>("shield")) return true;

            int diff = n - __instance.Get(status);
            if (diff <= 0) return true;

            int oldStacks = GetAndClear(__instance, "vowOfAction");
            if (oldStacks > 0) LostVowEffect(g, __instance.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_action"].Id);

            return true;
        }

        //
        // Vow of Poverty / Middling Income / Affluence
        //

        private static int originalCardCost = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static bool HarmonyPostfix_VowsOfMoney_Enforcement(State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            if (FeatureFlags.Debug && Input.shift) playNoMatterWhatForFree = true;
            originalCardCost = playNoMatterWhatForFree ? -1 : card.GetCurrentCost(s);

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void HarmonyPostfix_VowsOfMoney_Enforcement(ref bool __result, State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            if (!__result) return;
            if (FeatureFlags.Debug && Input.shift) playNoMatterWhatForFree = true;
            if (playNoMatterWhatForFree) return;

            // we can't just use card.GetCurrentCost, since Combat.TryPlayCard clears the discount the card may have had before this patch is run
            if (originalCardCost == 0)
            {
                int oldStacks = GetAndClear(g.state.ship, "vowOfPoverty");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_poverty"].Id);
            }
            else if (originalCardCost == 1)
            {
                int oldStacks = GetAndClear(g.state.ship, "vowOfMiddlingIncome");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_middling_income"].Id);
            }
            else if (originalCardCost == 2)
            {
                int oldStacks = GetAndClear(g.state.ship, "vowOfAffluence");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_affluence"].Id);
            }
        }

        //
        // Vow of Rest
        //

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AEndTurn), nameof(AEndTurn.Begin))]
        public static void HarmonyPostfix_VowOfRest_Enforcement(G g, State s, Combat c)
        {
            if (c.energy < 1)
            {
                int oldStacks = GetAndClear(g.state.ship, "vowOfRest");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_rest"].Id);
            }
            if (c.energy < 2)
            {
                int oldStacks = GetAndClear(g.state.ship, "vowOfMegaRest");
                if (oldStacks > 0) LostVowEffect(g, g.state.ship.GetShipRect(), (Spr)MainManifest.sprites["icons/vow_of_mega_rest"].Id);
            }
        }

        //
        // Vow of Courage
        //

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
        public static void HarmonyPostfix_VowOfCourage_Reward(AAttack __instance, G g, State s, Combat c)
        {
            if (!__instance.targetPlayer) return;

            var vowStacks = s.ship.Get((Status)MainManifest.statuses["vowOfCourage"].Id);
            if (vowStacks <= 0) return;

            Part? hitPart = AAttackPostfix_GetHitShipPart(__instance, s, c);
            if (hitPart == null) return;
            if (hitPart.type == Enum.Parse<PType>("empty")) return;

            // shot hit player
            AddHonor(s.ship, VOW_OF_COURAGE_HONOR * vowStacks);
        }

        //
        // Vow of Chivalry
        //

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
        public static void HarmonyPostfix_VowOfChivalry(AAttack __instance, G g, State s, Combat c)
        {
            var vowStacks = s.ship.Get((Status)MainManifest.statuses["vowOfChivalry"].Id);
            if (vowStacks <= 0) return;

            Part? hitPart = AAttackPostfix_GetHitShipPart(__instance, s, c);
            if (hitPart == null) return;

            PDamMod targetPartStatus = hitPart.GetDamageModifier();
            bool cheapShot = targetPartStatus == Enum.Parse<PDamMod>("brittle") || targetPartStatus == Enum.Parse<PDamMod>("weak");
            if (!cheapShot) return;

            if (!__instance.targetPlayer)
            {
                s.ship.Set((Status)MainManifest.statuses["vowOfChivalry"].Id, 0);
            }
        }
    }
}
