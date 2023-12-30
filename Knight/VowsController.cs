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
        public static readonly int VOW_OF_TEAMWORK_HONOR = 3;


        //
        // Vow of Adamancy
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
                var teamworkStacks = s.ship.Get((Status)MainManifest.statuses["vowOfTeamwork"].Id);
                var honor = s.ship.Get((Status)MainManifest.statuses["honor"].Id);
                s.ship.Set((Status)MainManifest.statuses["honor"].Id, honor - VOW_OF_TEAMWORK_HONOR * teamworkStacks);
                s.ship.Set((Status)MainManifest.statuses["vowOfTeamwork"].Id, 0);
            }
            cardColorsPlayedThisTurn.Clear();
            hasBrokenTeamworkVow = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void HarmonyPostfix_VowOfTeamwork_Enforcement(ref bool __result, State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            if (!__result) return;
            var teamworkStacks = s.ship.Get((Status)MainManifest.statuses["vowOfTeamwork"].Id);

            var deck = card.GetMeta().deck;
            if (!NewRunOptions.allChars.Contains(deck)) return; // don't prevent player from playing trash or things like Spent Casings from that one custom ship

            if (cardColorsPlayedThisTurn.Contains(deck) || hasBrokenTeamworkVow) // check to see if we've just broken the vow or if we already have
            {
                if (teamworkStacks > 0)
                {
                    var honor = s.ship.Get((Status)MainManifest.statuses["honor"].Id);
                    s.ship.Set((Status)MainManifest.statuses["honor"].Id, honor - VOW_OF_TEAMWORK_HONOR * teamworkStacks);
                    s.ship.Set((Status)MainManifest.statuses["vowOfTeamwork"].Id, 0);
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
        // Vow of Left/Right
        //
    }
}
