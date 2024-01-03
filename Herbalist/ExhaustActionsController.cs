using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Herbalist
{
    [HarmonyPatch]
    public class ExhaustActionsController
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.SendCardToExhaust))]
        public static void SendCardToExhaust(Combat __instance, State s, Card card)
        {
            if (card is HerbCard herb)
            {
                herb.OnExhausted(s, __instance);
                return;
            }

            //var actions = card.GetActionsOverridden(s, __instance);
            //foreach (var action in actions)
            //{
            //    if (action is AOnExhaust)
            //    {
            //        // queue wrapped actions
            //    }
            //}
        }
    }
}
