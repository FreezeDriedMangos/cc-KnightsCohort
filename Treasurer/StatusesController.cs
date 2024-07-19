using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OneOf.Types.TrueFalseOrNull;

namespace KnightsCohort.Treasurer
{
    // prefix/postfix Ship.RenderHealthBar to add gold/honor shield to temp shield
    // in postfix, draw rectangles over the last temp shield bars
    // may need to do the below as well:
    /*
        Box box = g.Push(new UIKey(UK.healthBar, 0, keyPrefix), new Rect(vec4.x, vec4.y, vec2.x, num7 + 3));
		Vec v = box.rect.xy;
		if (box.IsHover())
		{
			string value = Loc.T("combat.healthbar.hull", "Hull: {0}/{1}", hull, hullMax);
			string value2 = Loc.T("combat.healthbar.shield", "Shield: {0}/{1}", Get(Status.shield), GetMaxShield());
			string text = ((Get(Status.tempShield) > 0) ? Loc.T("combat.healthbar.tempshield", "Temp shield: {0}", Get(Status.tempShield)) : null);
			string text2 = $"<c=hurt>{value}</c>\n<c=healthBarShield>{value2}</c>";
			if (text != null)
			{
				text2 = text2 + "\n<c=healthBarTempShield>" + text + "</c>";
			}
			g.tooltips.Add(v + new Vec(0.0, num7 + 8), new TTText(text2));

            string text = ((Get(Status.tempShield) > 0) ? Loc.T("combat.healthbar.tempshield", "Honor shield: {0}", Get(Status.honorShield)) : null);
			g.tooltips.Add(v + new Vec(0.0, num7 + 8), new TTText(text2));
		}

     */

    [HarmonyPatch]
    public class StatusesController
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnAfterTurn))]
        public static void Charity(Ship __instance, State s, Combat c)
        {
            var goldStatus = (Status)MainManifest.statuses["gold"].Id;
            var charityAmount = __instance.Get((Status)MainManifest.statuses["charity"].Id);
            int goldAmount = __instance.Get(goldStatus);
            __instance.Set(goldStatus, Math.Max(0, goldAmount - charityAmount));
            __instance.Add((Status)MainManifest.statuses["honor"].Id, Math.Min(goldAmount, charityAmount));
        }
    }

    [HarmonyPatch]
    public class CustomShieldsController
    {
        static int originalTempShield;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
        public static void ClearTempCustomShields(Ship __instance, State s, Combat c)
        {
            if (__instance.Get((Status)MainManifest.statuses["goldShield"].Id) > 0)
            {
                __instance.Set((Status)MainManifest.statuses["goldShield"].Id, 0);
            }
            if (__instance.Get((Status)MainManifest.statuses["honorShield"].Id) > 0)
            {
                __instance.Set((Status)MainManifest.statuses["honorShield"].Id, 0);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.RenderHealthBar))]
        public static void CustomShieldDrawingPrePatch(Ship __instance, G g, bool isPreview, string keyPrefix)
        {
            originalTempShield = __instance.Get(Status.tempShield);
            // I'm directly accessing the statusEffects dictionary rather than using Set in order to avoid triggering anyone's "on gain temp shield" effects, since this isn't really gaining temp shield, it's just hijacking tempShield to avoid needing to transpile Ship.NormalDamage 
            __instance.statusEffects[Status.tempShield] =
                originalTempShield
                + __instance.Get((Status)MainManifest.statuses["goldShield"].Id)
                + __instance.Get((Status)MainManifest.statuses["honorShield"].Id)
                - __instance.ghostTempShield;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.RenderHealthBar))]
        public static void CustomShieldDrawingPostPatch(Ship __instance, G g, bool isPreview, string keyPrefix)
        {
            int shieldAmt = __instance.Get(Status.shield);
            int tempShieldAmt = __instance.Get(Status.tempShield);
            int goldShieldAmt = __instance.Get((Status)MainManifest.statuses["goldShield"].Id);
            int honorShieldAmt = __instance.Get((Status)MainManifest.statuses["honorShield"].Id);
            int maxShield = __instance.GetMaxShield();
            int emptyBarChunkCount = __instance.hullMax + maxShield;
            int barPlusTempShieldChunkCount = emptyBarChunkCount + (tempShieldAmt + __instance.ghostTempShield);
            int shipPartsWidth = (isPreview ? __instance.parts.Count : (__instance.parts.Count + 2));
            int shipPixelWidth = 16 * shipPartsWidth;
            int chunkWidth = Mutil.Clamp(shipPixelWidth / emptyBarChunkCount, 2, 4) - 1;
            int chunkMargin = 1;
            int num7 = 5;
            int num8 = 3;
            Vec vec = new Vec(emptyBarChunkCount * chunkWidth + (emptyBarChunkCount - 1) * chunkMargin + 2, num8 + 2);
            Vec vec2 = new Vec(barPlusTempShieldChunkCount * chunkWidth + (barPlusTempShieldChunkCount - 1) * chunkMargin + 2, num8 + 2);
            Vec vec3 = new Vec(__instance.hullMax * chunkWidth + (__instance.hullMax - 1) * chunkMargin + 2, num7 + 2);
            Vec vec4 = default(Vec);
            vec4.x -= Math.Round(vec.x / 2.0);
            Box box = g.Push(new UIKey(UK.healthBar + 8473749, 0, keyPrefix), new Rect(vec4.x, vec4.y, vec2.x, num7 + 3));
            Vec v = box.rect.xy;
            if (box.IsHover())
            {
                string value = Loc.T("combat.healthbar.hull", "Hull: {0}/{1}", __instance.hull, __instance.hullMax);
                string value2 = Loc.T("combat.healthbar.shield", "Shield: {0}/{1}", __instance.Get(Status.shield), __instance.GetMaxShield());
                string text = ((__instance.Get(Status.tempShield) > 0) ? Loc.T("combat.healthbar.tempshield", "Temp shield: {0}", __instance.Get(Status.tempShield)) : null);
                string text2 = $"<c=hurt>{value}</c>\n<c=healthBarShield>{value2}</c>";
                if (text != null)
                {
                    text2 = text2 + "\n<c=healthBarTempShield>" + text + "</c>";
                }
                g.tooltips.Add(v + new Vec(0.0, num7 + 8), new TTText(text2));
            }

            int maxTempIndex = tempShieldAmt + __instance.ghostTempShield - 1; // final index of all temporary shields (vanilla and custom)
            int minRealTempIndex = tempShieldAmt + __instance.ghostTempShield - 1 - honorShieldAmt - goldShieldAmt; // final index of vanilla temp shields
            for (int l = minRealTempIndex + 1; l <= maxTempIndex; l++)
            {
                bool isNotGhostShield = l < tempShieldAmt;
                Color color4 = l > minRealTempIndex + goldShieldAmt ? new Color("bbbb55") : new Color("aaffaa"); //Colors.healthBarTempShield;
                color4 = isNotGhostShield ? color4 : color4.fadeAlpha(0.25);

                if (l >= tempShieldAmt && l < tempShieldAmt + __instance.ghostTempShield)
                {
                    color4 = Colors.healthBarGhost;
                }
                DrawChunk(__instance.hullMax + maxShield + l, num8, color4, l < tempShieldAmt - 1 && l < tempShieldAmt - 1);
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

            // I'm directly accessing the statusEffects dictionary rather than using Set in order to avoid triggering anyone's "on gain temp shield" effects, since this isn't really gaining temp shield, it's just hijacking tempShield to avoid needing to transpile Ship.NormalDamage 
            if (originalTempShield == 0) __instance.statusEffects.Remove(Status.tempShield);
            else                         __instance.statusEffects[Status.tempShield] = originalTempShield;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage))]
        public static void CustomShieldDamagePrePatch(Ship __instance, State s, Combat c, int incomingDamage, int? maybeWorldGridX, bool piercing = false)
        {
            originalTempShield = __instance.Get(Status.tempShield);
            // I'm directly accessing the statusEffects dictionary rather than using Set in order to avoid triggering anyone's "on gain temp shield" effects, since this isn't really gaining temp shield, it's just hijacking tempShield to avoid needing to transpile Ship.NormalDamage 
            __instance.statusEffects[Status.tempShield] =
                originalTempShield
                + __instance.Get((Status)MainManifest.statuses["goldShield"].Id)
                + __instance.Get((Status)MainManifest.statuses["honorShield"].Id);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage))]
        public static void CustomShieldDamagePostPatch(Ship __instance, State s, Combat c, int incomingDamage, int? maybeWorldGridX, bool piercing = false)
        {
            Status goldShield = (Status)MainManifest.statuses["goldShield"].Id;
            Status honorShield = (Status)MainManifest.statuses["honorShield"].Id;

            int missingShield = originalTempShield - __instance.Get(Status.tempShield);

            int originalHonorShield = __instance.Get(honorShield);
            int missingHonorShield = Math.Min(originalHonorShield, missingShield);
            __instance.Add(honorShield, -missingHonorShield);
            __instance.Add((Status)MainManifest.statuses["honor"].Id, missingHonorShield);
            missingShield = Math.Max(0, missingShield - missingHonorShield);

            int originalGoldShield = __instance.Get(goldShield);
            int missingGoldShield = Math.Min(originalGoldShield, missingShield);
            __instance.Add(goldShield, -missingGoldShield);
            __instance.Add((Status)MainManifest.statuses["gold"].Id, missingGoldShield);
            missingShield = Math.Max(0, missingShield - originalGoldShield);

            if (missingShield > originalTempShield)
            {
                throw new Exception($"Math isn't mathing. {{currentTempShield: {__instance.Get(Status.tempShield)}, originalTempShield: {originalTempShield}, originalHonorShield: {originalHonorShield}, originalGoldShield: {originalGoldShield}, missingShield: {originalTempShield - __instance.Get(Status.tempShield)}}}");
            }
            else if (missingShield == originalTempShield)
            {
                __instance.statusEffects.Remove(Status.tempShield);
            }
            else
            {
                __instance.statusEffects[Status.tempShield] = originalTempShield - missingShield;
            }
        }
    }
}
