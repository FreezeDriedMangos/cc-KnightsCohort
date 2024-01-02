using HarmonyLib;
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
        public static Color HONOR_COLOR = new Color("c5c5c5");

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.DrainCardActions))]
        public static void HarmonyPostfix_HonorableWinCheck(Combat __instance, G g)
        {
            bool actionJustEnded = __instance.currentCardAction != null && __instance.currentCardAction.timer <= 0.0;
            if (!actionJustEnded) return;

            if (g.state.ship.Get((Status)MainManifest.statuses["honor"].Id) >= __instance.otherShip.hull + __instance.otherShip.Get(Enum.Parse<Status>("shield")))
            {
                // TODO: don't do this for certain fights, like Soggins

                __instance.Queue(new AMidCombatDialogue
                {
                    script = "clay.KnightsCohort.Honorable_Win" // make this randomly pick a line from a list of multiple for each of the 3 knights
                });
                __instance.Queue(new ADelay
                {
                    time = 0.0,
                    timer = 0.1
                });
                __instance.Queue(new AEscape
                {
                    targetPlayer = false
                });
            }
            else if (__instance.otherShip.Get((Status)MainManifest.statuses["honor"].Id) >= g.state.ship.hull + g.state.ship.Get(Enum.Parse<Status>("shield")))
            {
                __instance.noReward = true;

                __instance.Queue(new AMidCombatDialogue
                {
                    script = "clay.KnightsCohort.Honorable_Loss" // make this randomly pick a line from a list of multiple for each of the 3 knights
                });
                __instance.Queue(new ADelay
                {
                    time = 0.0,
                    timer = 0.1
                });
                __instance.Queue(new AEscape
                {
                    targetPlayer = true
                });
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.RenderHealthBar))]
        public static void HarmonyPostfix_RenderHonorOnOpponentHealthBar(Ship __instance, G g, bool isPreview, string keyPrefix)
        {
            if (g.state.route is Combat c)
            {
                int honor = __instance.isPlayerShip 
                    ? c.otherShip.Get((Status)MainManifest.statuses["honor"].Id)
                    : g.state.ship.Get((Status)MainManifest.statuses["honor"].Id);

                int hull = __instance.hull;
                int shield = __instance.Get(Enum.Parse<Status>("shield"));
                int tempShield = __instance.Get(Enum.Parse<Status>("tempShield"));

                int hullHonor = Math.Min(hull, honor);
                int shieldHonor = Math.Min(shield, honor - hullHonor);
                int tempShieldHonor = Math.Min(tempShield, honor - hullHonor - shieldHonor); // I don't think honor will care about temp shield, but let's draw it anyway

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
                
                for (int j = 0; j < hullHonor; j++)
                {
                    DrawChunk(j, num7, HONOR_COLOR, j < hull - 1);
                }
                for (int k = 0; k < shieldHonor; k++)
                {
                    DrawChunk(__instance.hullMax + k, num8, HONOR_COLOR, k < maxShield - 1 && k < num - 1);
                }
                for (int l = 0; l < tempShieldHonor; l++)
                {
                    DrawChunk(__instance.hullMax + maxShield + l, num8, HONOR_COLOR, l < num2 - 1 && l < num2 - 1);
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
