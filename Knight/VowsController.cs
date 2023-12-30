using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Knight
{
    [HarmonyPatch]
    public static class VowsController
    {
        public static readonly int VOW_OF_MERCY_HONOR = 1;
        public static readonly int VOW_OF_ADAMANCY_HONOR = 1;


        //
        // Vow of Adamancy
        //

        private static int previousX = 0;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.DrainCardActions))]
        public static void HarmonyPostfix_VowOfAdamancy(Combat __instance, G g)
        {
            bool actionJustEnded = __instance.currentCardAction != null && __instance.currentCardAction.timer <= 0.0;
            if (!actionJustEnded) return;

            if (previousX != g.state.ship.x)
            {
                previousX = g.state.ship.x;
                if (g.state.ship.Get((Status)MainManifest.statuses["vowOfAdamancy"].Id) > 0)
                {
                    var adamancyVow = g.state.ship.Get((Status)MainManifest.statuses["vowOfAdamancy"].Id);
                    g.state.ship.Set((Status)MainManifest.statuses["vowOfAdamancy"].Id, 0);
                    var honor = g.state.ship.Get((Status)MainManifest.statuses["honor"].Id);
                    g.state.ship.Set((Status)MainManifest.statuses["honor"].Id, honor - VOW_OF_ADAMANCY_HONOR * adamancyVow);
                }
            }
        }

        //
        // Vow of Mercy
        //

        static bool attackedThisTurn = false;


        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAfterPlayerTurn), nameof(AAfterPlayerTurn.Begin))]
        public static void HarmonyPostfix_VowOfMercy_Concequences(G g, State s, Combat c)
        {
            var mercyVow = g.state.ship.Get((Status)MainManifest.statuses["vowOfMercy"].Id);
            if (!attackedThisTurn)
            {
                var honor = g.state.ship.Get((Status)MainManifest.statuses["honor"].Id);
                g.state.ship.Set((Status)MainManifest.statuses["honor"].Id, honor + VOW_OF_MERCY_HONOR * mercyVow);
                g.state.ship.PulseStatus((Status)MainManifest.statuses["vowOfMercy"].Id);
            }

            g.state.ship.Set((Status)MainManifest.statuses["vowOfMercy"].Id, mercyVow - 1);
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
    }
}
