﻿using HarmonyLib;
using System;
using System.Collections.Generic;
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
            int? num = aattack.GetFromX(s, c);
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
            if (n > 2) n = 2;
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

        //
        // Vow of Adamancy & Left/Right
        //

        private static int previousX = 0;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AEnemyTurn), nameof(AEnemyTurn.Begin))]
        public static void HarmonyPostfix_VowOfAdamancy_Cleanup(G g, State s, Combat c)
        {
            previousX = 0; // cleanup for second combat
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.DrainCardActions))]
        public static void HarmonyPostfix_VowOfAdamancy(Combat __instance, G g)
        {
            bool actionJustEnded = __instance.currentCardAction != null && __instance.currentCardAction.timer <= 0.0;
            if (!actionJustEnded) return;

            if (previousX != g.state.ship.x)
            {
                previousX = g.state.ship.x;
                GetAndClear(g.state.ship, "vowOfAdamancy"); 
            }

            if (previousX < g.state.ship.x)
            {
                // we've moved right
                GetAndClear(g.state.ship, "vowOfLeft");
            }

            if (previousX > g.state.ship.x)
            {
                // we've moved left
                GetAndClear(g.state.ship, "vowOfRight");
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
                GetAndClear(s.ship, "vowOfLeft");
            }

            if (cardIndex == 0)
            {
                // we've played the leftmost card
                GetAndClear(s.ship, "vowOfRight");
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

            if (__instance.currentCardAction is AAttack)
            {
                attackedThisTurn = true;
            }
        }

        //
        // Vow of Teamwork
        //

        static HashSet<Deck> cardColorsPlayedThisTurn = new();
        static bool hasBrokenTeamworkVow = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AEnemyTurn), nameof(AEnemyTurn.Begin))]
        public static void HarmonyPostfix_VowOfTeamwork_Cleanup(G g, State s, Combat c)
        {
            if (hasBrokenTeamworkVow) // check to see if we missed breaking the vow
            {
                GetAndClear(s.ship, "vowOfTeamwork");
            }
            cardColorsPlayedThisTurn.Clear();
            hasBrokenTeamworkVow = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void HarmonyPostfix_VowOfTeamwork_Enforcement(ref bool __result, State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            if (!__result) return;

            var deck = card.GetMeta().deck;
            if (!NewRunOptions.allChars.Contains(deck)) return; // don't prevent player from playing trash or things like Spent Casings from that one custom ship

            if (cardColorsPlayedThisTurn.Contains(deck) || hasBrokenTeamworkVow) // check to see if we've just broken the vow or if we already have
            {
                var teamworkStacks = GetAndClear(s.ship, "vowOfTeamwork");
                if (teamworkStacks > 0)
                {
                    hasBrokenTeamworkVow = false;
                }
                else
                {
                    hasBrokenTeamworkVow = true;
                }
            }

            cardColorsPlayedThisTurn.Add(deck);
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

            GetAndClear(__instance, "vowOfAction");

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
                GetAndClear(s.ship, "vowOfPoverty");
            }
            else if (originalCardCost == 1)
            {
                GetAndClear(s.ship, "vowOfMiddlingIncome");
            }
            else if (originalCardCost == 2)
            {
                GetAndClear(s.ship, "vowOfAffluence");
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
                GetAndClear(s.ship, "vowOfRest");
            }
            if (c.energy < 2)
            {
                GetAndClear(s.ship, "vowOfMegaRest");
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
