using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Herbalist
{
    [HarmonyPatch]
    public class StatusesController
    {
        //
        // Dazed
        //

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AMove), nameof(AMove.Begin))]
        public static void DazedPatch(AMove __instance, G g, State s, Combat c)
        {
            Ship ship = (__instance.targetPlayer ? s.ship : c.otherShip);
            if (ship.Get((Status)MainManifest.statuses["dazed"].Id) > 0)
            {
                __instance.dir *= -1;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
        public static void DazedAndBlindnessDecrement(Ship __instance, State s, Combat c)
        {
            Knight.VowsController.GetAndDecrement(__instance, "dazed");
            Knight.VowsController.GetAndDecrement(__instance, "blindness");
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
        [HarmonyPatch(typeof(Card), nameof(Card.RenderAction))]
        public static bool Blindness_CardActionPatch(G g, State state, CardAction action, bool dontDraw = false, int shardAvailable = 0, int stunChargeAvailable = 0, int bubbleJuiceAvailable = 0)
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
            // TODO: make a generic card art "password hidden; eye with a / over it" for this, rather than OwnerMissing
            if (blindnessEnabled) __result.art = (Spr)MainManifest.sprites["cards/blindness"].Id; //Enum.Parse<Spr>("cards_OwnerMissing");
            if (blindnessEnabled) __result.description = null;
        }

        // TODO: enemy blindness effect
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AIHelpers), nameof(AIHelpers.MoveToAimAt))]
        public static void Blindness_EnemyPatch(List<CardAction> __result, State s, Ship movingShip, Ship targetShip, int alignPartLocalX, int maxMove = 99, bool movesFast = false, bool? attackWeakPoints = null, bool avoidAsteroids = false, bool avoidMines = true)
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
                Enumerable.Range(0, blindnessStacks).Select(i => new AMove() {
                    dir = Math.Sign(m.dir),
                    targetPlayer = false
                })
            );
        }
    }
}
