using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Treasurer
{
    [HarmonyPatch]
    public class StatusesController
    {
        public static int INTEREST_RATE = 3;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnAfterTurn))]
        public static void GoldInterest(Ship __instance, State s, Combat c)
        {
            var goldStatus = (Status)MainManifest.statuses["gold"].Id;
            int goldAmount = __instance.Get(goldStatus);
            __instance.Set(goldStatus, goldAmount + (goldAmount / INTEREST_RATE));

            var charityAmount = __instance.Get((Status)MainManifest.statuses["charity"].Id);
            goldAmount = __instance.Get(goldStatus); // refetch gold just in case
            __instance.Set(goldStatus, Math.Max(0, goldAmount - charityAmount));
            __instance.Add((Status)MainManifest.statuses["honor"].Id, Math.Max(0, goldAmount - charityAmount));
        }
    }
}
