using FMOD.Studio;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Bannerlady
{
    [HarmonyPatch]
    public static class StatusesController
    {
        public static void DecrementStatus(Ship ship, Status status)
        {
            var stacks = ship.Get(status);
            ship.Set(status, Math.Max(0, stacks-1));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
        public static void HarmonyPostfix_VowOfCourage_Cleanup(Ship __instance, State s, Combat c)
        {
            DecrementStatus(__instance, (Status)MainManifest.statuses["shieldOfFaith"].Id);
        }
    }
}
