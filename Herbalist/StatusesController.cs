using HarmonyLib;
using KnightsCohort.actions;
using KnightsCohort.Knight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CardBrowse;

namespace KnightsCohort.Herbalist
{
    [HarmonyPatch]
    public class StatusesController
    {
        //
        // Temp Sherb
        //

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
        private static void DoSherbEffect(AAttack __instance, G g, State s, Combat c)
        {
            Ship ship = (__instance.targetPlayer ? c.otherShip : s.ship);
            if (ship.Get((Status)MainManifest.statuses["tempSherb"].Id) <= 0) return;

            c.QueueImmediate(new AStatus
            {
                targetPlayer = !__instance.targetPlayer,
                status = Status.tempShield,
                statusAmount = 1
            });
        }

        //
        // Herberdrive
        //

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Card), nameof(Card.GetDmg))]
        //public static void GetDmgHerberdrive(ref int __result, State s, int baseDamage, bool targetPlayer = false)
        //{
        //    if (s.ship.Get((Status)MainManifest.statuses["herberdrive"].Id) <= 0) return;
        //    __result++;
        //}

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetActualDamage))]
        public static void GetDmgHerberdrive(ref int __result, State s, int baseDamage, bool targetPlayer = false, Card? card = null)
        {
            Ship ship = (targetPlayer && s.route is Combat c ? c.otherShip : s.ship);
            if (ship.Get((Status)MainManifest.statuses["herberdrive"].Id) <= 0) return;
            __result++;
        }

        //
        // Paranoia
        //

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AEnemyTurnAfter), nameof(AEnemyTurnAfter.Begin))]
        public static void EnemyParanoia(G g, State s, Combat c)
        {
            if (c.otherShip.Get((Status)MainManifest.statuses["paranoia"].Id) <= 0) return;

            c.QueueImmediate(new List<CardAction>()
            {
                new ADelay() { time = 0.0, timer = 0.5 },
                new ANullRandomIntent_Paranoia(),
                new AStatus() { status = (Status)MainManifest.statuses["paranoia"].Id, targetPlayer = false, statusAmount = -1 },
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
        public static void PlayerParanoia(Ship __instance, State s, Combat c)
        {
            if (!__instance.isPlayerShip) return;
            if (__instance.Get((Status)MainManifest.statuses["paranoia"].Id) <= 0) return;
            c.QueueImmediate(new List<CardAction>()
            {
                // TODO: [REQUIRES NICKEL] instead of adding an AbyssalVisions, give the player a random crew-member-missing status
                new AAddCard() { card = new AbyssalVisions(), destination = CardDestination.Hand },
                new AStatus() { status = (Status)MainManifest.statuses["paranoia"].Id, targetPlayer = true, statusAmount = -1 },
            });
        }

        //
        // Dazed
        //

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AMove), nameof(AMove.Begin))]
        public static void DazedPatch(AMove __instance, G g, State s, Combat c)
        {
            Ship ship = (__instance.targetPlayer ? s.ship : c.otherShip);
            int daze = Knight.VowsController.GetAndDecrement(ship, "dazed");
            if (daze > 0)
            {
                __instance.dir *= -1;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnAfterTurn))]
        public static void DazedAndBlindnessDecrement_Enemy(Ship __instance, State s, Combat c)
        {
            // These are decremented at the end of the opponent's turn so that if the player applied, say 1 herberdrive on their turn
            // the enemy WILL fire while the effects of that 1 herberdrive are still active. If it was instead decremented at the
            // start of the enemy turn, it would go to 0 before the enemy ship ever fired, even once.
            if (__instance.isPlayerShip) return;
            Knight.VowsController.GetAndDecrement(__instance, "dazed");
            Knight.VowsController.GetAndDecrement(__instance, "blindness");
            Knight.VowsController.GetAndDecrement(__instance, "herberdrive");
            Knight.VowsController.GetAndDecrement(__instance, "tempSherb");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
        public static void DazedAndBlindnessDecrement(Ship __instance, State s, Combat c)
        {
            if (!__instance.isPlayerShip) return;
            Knight.VowsController.GetAndDecrement(__instance, "dazed");
            Knight.VowsController.GetAndDecrement(__instance, "blindness");
            Knight.VowsController.GetAndDecrement(__instance, "herberdrive");
            Knight.VowsController.GetAndDecrement(__instance, "tempSherb");
        }

        //
        // Blindness
        //

        static bool blindnessEnabled = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Card), nameof(Card.Render))]
        public static void Blindness_EnableDisable(G g, Vec? posOverride = null, State? fakeState = null, bool ignoreAnim = false, bool ignoreHover = false, bool hideFace = false, bool hilight = false, bool showRarity = false, bool autoFocus = false, UIKey? keyOverride = null, OnMouseDown? onMouseDown = null, OnMouseDownRight? onMouseDownRight = null, OnInputPhase? onInputPhase = null, double? overrideWidth = null, UIKey? leftHint = null, UIKey? rightHint = null, UIKey? upHint = null, UIKey? downHint = null, int? renderAutopilot = null, bool? forceIsInteractible = null, bool reportTextBoxesForLocTest = false, bool isInCombatHand = false)
        {
            blindnessEnabled = g.state.ship.Get((Status)MainManifest.statuses["blindness"].Id) > 0;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetFullDisplayName))]
        public static void Blindness_CardNamePatch(Card __instance, ref string __result)
        {
            if (blindnessEnabled) __result = " ";
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Card), nameof(Card.MakeAllActionIcons))]
        public static bool Blindness_CardActionPatch(G g, State s)
        {
            if (blindnessEnabled) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetAllTooltips))]
        public static bool Blindness_HideTooltips(Card __instance, ref IEnumerable<Tooltip> __result, G g, State s, bool showCardTraits = true)
        {
            if (!blindnessEnabled) return true;
            __result = new List<Tooltip>();
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetDataWithOverrides))]
        public static void Blindness_CardArtPatch(Card __instance, ref CardData __result, State state)
        {
            if (blindnessEnabled) __result.art = (Spr)MainManifest.sprites["cards/blindness"].Id;
            if (blindnessEnabled) __result.description = null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AIHelpers), nameof(AIHelpers.MoveToAimAt), new Type[] { typeof(State), typeof(Ship), typeof(Ship), typeof(int), typeof(int), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
        public static void Blindness_EnemyPatch(List<CardAction> __result, State s, Ship movingShip, Ship targetShip, int key, int maxMove = 99, bool movesFast = false, bool? attackWeakPoints = null, bool avoidAsteroids = false, bool avoidMines = true, bool avoidDualDrones = false)
        {
            var blindnessStacks = movingShip.Get((Status)MainManifest.statuses["blindness"].Id);
            if (blindnessStacks <= 0) return;
            if (__result.Count <= 0) return;

            AMove m = __result[0] as AMove;
            if (m.dir == 0) return;

            if (movesFast)
            {
                m.dir += Math.Sign(m.dir) * blindnessStacks;
                return;
            }

            __result.AddRange
            (
                Enumerable.Range(0, blindnessStacks).Select(i => new AMove()
                {
                    dir = Math.Sign(m.dir),
                    targetPlayer = false
                })
            );
        }
    }
}
