using HarmonyLib;
using KnightsCohort.actions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort
{
    [HarmonyPatch]
    public static class HonorController
    {
        //// TODO: this is just for fixing debug menu's low resolution
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Editor), nameof(Editor.ImGuiLayout))]
        //public static void FixEditorResolution(G g)
        //{

        //}


        public static Color HONOR_COLOR = new Color("c5c5c5");
        public static Color GHOST_HONOR_COLOR = Colors.healthBarGhost; //new Color("f0e892");
        public static Color GHOST_LOST_HONOR_COLOR = new Color("8f34eb");

        private static int ghostHonor = 0;
        private static double ghostHonorTimer = 0;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.Update))]
        public static void HarmonyPostfix_GhostHonorTimerHandler(G g, bool enableParticles = false)
        {
            ghostHonorTimer -= g.dt;
            if (ghostHonorTimer <= 0.0)
            {
                ghostHonorTimer = 0;
                ghostHonor = 0;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.Set))]
        public static void HarmonyPostfix_SetGhostHonor(Ship __instance, Status status, int n)
        {
            if (status != (Status)MainManifest.statuses["honor"].Id) return;
            if (!__instance.isPlayerShip) return;

            int delta = n - __instance.Get((Status)MainManifest.statuses["honor"].Id);
            //if (delta < 0) delta = (int)Math.Floor(delta / 2.0); // I have no clue why this is necessary
            ghostHonor += delta;
            ghostHonorTimer = 1;

            // TODO: make the other ship shake???
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.DrainCardActions))]
        public static void HarmonyPostfix_HonorableWinCheck(Combat __instance, G g)
        {
            bool __result;
            State s = g.state;

            //bool actionJustEnded = __instance.currentCardAction != null && __instance.currentCardAction.timer <= 0.0;
            //if (!actionJustEnded) return;

            // TODO: this breaks when the enemy increases your honor through an on hit effect, such as the martyr banner or vow of courage

            // if (s.ship.Get((Status)MainManifest.statuses["honor"].Id) <= 0) return;
            if (__instance.otherShip.hull <= 0) return; // TODO: BUG: THIS DOESN'T SEEM TO WORK, GAINING ENOUGH HONOR TO MAKE THE ENEMY FLEE AND ALSO KILLING THEM ON THE SAME TURN STILL CAUSES A CRASH
            if (s.ship.hull <= 0) return;

            if (s.ship.Get((Status)MainManifest.statuses["honor"].Id) >= __instance.otherShip.hull + __instance.otherShip.Get(Enum.Parse<Status>("shield")))
            {
                //if (s.map.IsFinalZone()) // this check doesn't work
                //{
                //    // bosses just die
                //    __instance.QueueImmediate(new List<CardAction>()
                //    {
                //        new ADelay() { time = 0.0, timer = 0.3 },
                //        new AHurt() { targetPlayer = false, hurtAmount = __instance.otherShip.hullMax+10, hurtShieldsFirst = false }
                //    });
                //}
                
                if (__instance.otherShip.ai is SogginsEvent) {
                    // TOOD: queue special dialoge "what's honor? can I eat it?"
                    return; // don't resolve the soggins event with honor
                }

                __result = true;

                __instance.Queue(new AHonorableVictory());
            }
            else if (__instance.otherShip.Get((Status)MainManifest.statuses["honor"].Id) >= s.ship.hull + s.ship.Get(Enum.Parse<Status>("shield")))
            {
                __instance.QueueImmediate(new AHurt() { targetPlayer = true, hurtAmount = 9999999 });
                //__instance.noReward = true;
                //__result = true;

                //__instance.Queue(new AMidCombatDialogue
                //{
                //    script = "clay.KnightsCohort.Honorable_Loss", // make this randomly pick a line from a list of multiple for each of the knights
                //    canRunAfterKill = true,
                //});
                //__instance.Queue(new ADelay
                //{
                //    time = 0.0,
                //    timer = 0.1,
                //    canRunAfterKill = true,
                //});
                //__instance.Queue(new AEscape
                //{
                //    targetPlayer = true,
                //    canRunAfterKill = true,
                //});
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.RenderHealthBar))]
        public static void HarmonyPostfix_RenderHonorOnOpponentHealthBar(Ship __instance, G g, bool isPreview, string keyPrefix)
        {
            int ghostHonor = __instance.isPlayerShip ? 0 : HonorController.ghostHonor;

            if (g.state.route is Combat c)
            {
                int honor = __instance.isPlayerShip 
                    ? c.otherShip.Get((Status)MainManifest.statuses["honor"].Id)
                    : g.state.ship.Get((Status)MainManifest.statuses["honor"].Id);
                if (ghostHonor < 0) honor -= ghostHonor;

                int hull = __instance.hull;
                int shield = __instance.Get(Enum.Parse<Status>("shield"));
                int tempShield = __instance.Get(Enum.Parse<Status>("tempShield"));

                int hullHonor = Math.Min(hull, honor);
                int shieldHonor = Math.Min(shield, honor - hullHonor);
                int tempShieldHonor = Math.Min(tempShield, honor - hullHonor - shieldHonor); // I don't think honor will care about temp shield, but let's draw it anyway

                if (ghostHonor < 0) honor += ghostHonor;

                int num = shieldHonor;//Get(Status.shield);
                int num2 = tempShield; //Get(Status.tempShield);
                int maxShield = __instance.GetMaxShield();
                int num3 = __instance.hullMax + maxShield;
                int num4 = num3 + (num2 + __instance.ghostTempShield);
                int num5 = (isPreview ? __instance.parts.Count : (__instance.parts.Count + 2));
                int num6 = 16 * num5;
                int chunkWidth = Mutil.Clamp(num6 / num3, 2, 4) - 1;
                int chunkMargin = 1;
                int num7 = 5;
                int num8 = 3;
                Vec vec = new Vec(num3 * chunkWidth + (num3 - 1) * chunkMargin + 2, num8 + 2);
                Vec vec2 = new Vec(num4 * chunkWidth + (num4 - 1) * chunkMargin + 2, num8 + 2);
                // Vec vec3 = new Vec(__instance.hullMax * chunkWidth + (__instance.hullMax - 1) * chunkMargin + 2, num7 + 2);
                Vec vec4 = default(Vec);
                vec4.x -= Math.Round(vec.x / 2.0);
                Box box = g.Push(new UIKey(UK.healthBar, 1, keyPrefix), new Rect(vec4.x, vec4.y, vec2.x, num7 + 3));
                Vec v = box.rect.xy;

                var t = Math.Min(1, Math.Max(0.7, Math.Abs(3*(g.state.time % 1) - 0.1)));
                for (int j = 0; j < hullHonor; j++)
                {
                    //var t = Math.Max(0, Math.Sin(3*g.state.time + j));
                    var color = j >= honor-ghostHonor 
                        ? GHOST_HONOR_COLOR
                        : Color.Lerp(Colors.healthBarHealth, HONOR_COLOR, t);
                    color = j >= honor ? GHOST_LOST_HONOR_COLOR : color;

                    DrawChunk(j, num7, color, j < hull - 1);
                }
                for (int k = 0; k < shieldHonor; k++)
                {
                    //var t = Math.Max(0, Math.Sin(3*g.state.time + k + __instance.hullMax));
                    var color = k+hullHonor >= honor - ghostHonor
                        ? GHOST_HONOR_COLOR
                        : Color.Lerp(Colors.healthBarShield, HONOR_COLOR, t);
                    color = k+hullHonor >= honor ? GHOST_LOST_HONOR_COLOR : color;

                    DrawChunk(__instance.hullMax + k, num8, color, k < maxShield - 1 && k < num - 1);
                }
                for (int l = 0; l < tempShieldHonor; l++)
                {
                    //var t = Math.Max(0, Math.Sin(3*g.state.time + l + __instance.hullMax + maxShield));
                    var color = l+hullHonor+shieldHonor >= honor - ghostHonor
                        ? GHOST_HONOR_COLOR
                        : Color.Lerp(Colors.healthBarShield, HONOR_COLOR, t);
                    color = l+hullHonor+shieldHonor >= honor ? GHOST_LOST_HONOR_COLOR : color;

                    DrawChunk(__instance.hullMax + maxShield + l, num8, color, l < num2 - 1 && l < num2 - 1);
                }
                g.Pop();
                void DrawChunk(int i, int height, Color color, bool rightMargin)
                {
                    double num9 = v.x + 1.0 + (double)(i * (chunkWidth + chunkMargin));
                    double y = v.y + 1.0;
                    Draw.Rect(num9, y, chunkWidth, height, color);
                    if (rightMargin)
                    {
                        Draw.Rect(num9 + (double)chunkWidth, y, chunkMargin, height, color.fadeAlpha(0.5));
                    }
                }
            }
        }

    }
}
