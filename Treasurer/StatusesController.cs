using HarmonyLib;
using KnightsCohort.actions;
using Microsoft.Extensions.Logging;
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


    //
    //
    // NOTE: behind the scenes, gold/honor shield is just temp shield that's been colored over in yellow/green sharpie
    //
    //
    [HarmonyPatch]
    public class CustomShieldsController
    {
        static int originalTempShield;

        //private static int tempShieldToAddInPost = 0;

        // static bool freezeCustomShields = false;

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Ship), nameof(Ship.Set))]
        //public static void SyncTempShieldWithCustomShields(Ship __instance, Status status, int n = 1)
        //{
        //    //
        //    // Handling for making sure we don't end up with more custom shields than temp shields
        //    //
        //    if (status == Status.tempShield)
        //    {
        //        if (freezeCustomShields) return;

        //        // if n < goldShield+honorShield, remove gold shield
        //        // if still less, remove honor shield
        //        int goldShield = __instance.Get((Status)MainManifest.statuses["goldShield"].Id);
        //        int honorShield = __instance.Get((Status)MainManifest.statuses["honorShield"].Id);

        //        if (n >= goldShield + honorShield) return;

        //        MainManifest.Instance.Logger.LogInformation($"Setting temp shield, {n} golds{goldShield} honors{honorShield}");

        //        // GGHHH
        //        // TTT
        //        __instance._Set((Status)MainManifest.statuses["goldShield"].Id, Math.Min(n, goldShield));

        //        n -= Math.Min(n, goldShield);

        //        __instance._Set((Status)MainManifest.statuses["honorShield"].Id, Math.Min(n, honorShield));

        //        MainManifest.Instance.Logger.LogInformation($"        {n} golds{__instance.Get((Status)MainManifest.statuses["goldShield"].Id)} honors{__instance.Get((Status)MainManifest.statuses["honorShield"].Id)}");
        //    }
        //    ////
        //    //// Handling for changing custom shield stacks
        //    ////
        //    //else if (status == (Status)MainManifest.statuses["honorShield"].Id)
        //    //{
        //    //    int diff = n - __instance.Get((Status)MainManifest.statuses["honorShield"].Id);
        //    //    MainManifest.Instance.Logger.LogInformation($"Setting honor shield, {n} diffing temp shield {diff}");
        //    //    //__instance.Add(Status.tempShield, diff);
        //    //    tempShieldToAddInPost = diff;
        //    //}
        //    //else if (status == (Status)MainManifest.statuses["goldShield"].Id)
        //    //{
        //    //    int diff = n - __instance.Get((Status)MainManifest.statuses["goldShield"].Id);
        //    //    MainManifest.Instance.Logger.LogInformation($"Setting gold shield, {n} diffing gold shield {diff}");
        //    //    //__instance.Add(Status.tempShield, diff);
        //    //    tempShieldToAddInPost = diff;
        //    //}
        //}


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Ship), nameof(Ship.Set))]
        //public static void SyncTempShieldWithCustomShields_Post(Ship __instance, Status status, int n = 1)
        //{
        //    if (tempShieldToAddInPost == 0) return;
        //    MainManifest.Instance.Logger.LogInformation($"      temp shield, {__instance.Get(Status.tempShield) + tempShieldToAddInPost} diff of {tempShieldToAddInPost}");
        //    __instance._Set(Status.tempShield, __instance.Get(Status.tempShield) + tempShieldToAddInPost);
        //    tempShieldToAddInPost = 0;
        //}

        // ///////////////////////////////////////////
        //
        // HIT HANDLING
        //
        // ///////////////////////////////////////////

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage))]
        public static void CustomShieldDamagePrePatch(Ship __instance, State s, Combat c, int incomingDamage, int? maybeWorldGridX, bool piercing = false)
        {
            originalTempShield = __instance.Get(Status.tempShield);
            //// I'm directly accessing the statusEffects dictionary rather than using Set in order to avoid triggering anyone's "on gain temp shield" effects, since this isn't really gaining temp shield, it's just hijacking tempShield to avoid needing to transpile Ship.NormalDamage 
            //__instance.statusEffects[Status.tempShield] =
            //    originalTempShield
            //    + __instance.Get((Status)MainManifest.statuses["goldShield"].Id)
            //    + __instance.Get((Status)MainManifest.statuses["honorShield"].Id);
        }


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage))]
        //public static void FreezeCustomShieldsForDamageCalc(Ship __instance, State s, Combat c, int incomingDamage, int? maybeWorldGridX, bool piercing = false)
        //{
        //    freezeCustomShields = true;
        //}

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
        public static void ExpireCustomShields(Ship __instance, State s, Combat c)
        {
            Status goldShield = (Status)MainManifest.statuses["goldShield"].Id;
            Status honorShield = (Status)MainManifest.statuses["honorShield"].Id;

            int originalGoldShield = __instance.Get(goldShield);
            int originalHonorShield = __instance.Get(honorShield);

            __instance.Add((Status)MainManifest.statuses["gold"].Id, originalGoldShield);
            __instance.Add((Status)MainManifest.statuses["honor"].Id, originalHonorShield);
            
            __instance.Set(goldShield, 0);
            __instance.Set(honorShield, 0);

            MainManifest.Instance.Logger.LogInformation($"GAINING {originalHonorShield} HONOR and {originalGoldShield} GOLD  -  original gshield {originalGoldShield} original hshield {originalHonorShield}  -  original tshield {originalTempShield} new temp shield {__instance.Get(Status.tempShield)}");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage))]
        public static void CustomShieldDamagePostPatch(Ship __instance, State s, Combat c, int incomingDamage, int? maybeWorldGridX, bool piercing = false)
        {
            // freezeCustomShields = false;

            Status goldShield = (Status)MainManifest.statuses["goldShield"].Id;
            Status honorShield = (Status)MainManifest.statuses["honorShield"].Id;

            int originalHonorShield = __instance.Get(honorShield);
            int originalGoldShield = __instance.Get(goldShield);
            int missingShield = originalTempShield - __instance.Get(Status.tempShield);

            int missingHonorShield = Math.Min(originalHonorShield, missingShield);
            __instance.Add(honorShield, -missingHonorShield);
            __instance.Add((Status)MainManifest.statuses["honor"].Id, 2*missingHonorShield);
            missingShield = Math.Max(0, missingShield - missingHonorShield);

            int missingGoldShield = Math.Min(originalGoldShield, missingShield);
            __instance.Add(goldShield, -missingGoldShield);
            __instance.Add((Status)MainManifest.statuses["gold"].Id, 2*missingGoldShield);
            missingShield = Math.Max(0, missingShield - originalGoldShield);

            MainManifest.Instance.Logger.LogInformation($"GAINING 2*{missingHonorShield} HONOR and 2*{missingGoldShield} GOLD  -  original gshield {originalGoldShield} original hshield {originalHonorShield}  -  original tshield {originalTempShield} new temp shield {__instance.Get(Status.tempShield)} -  missing shield {missingShield}");
            // MainManifest.Instance.Logger.LogInformation($"GAINING {missingHonorShield} HONOR and {missingGoldShield} GOLD  -  original gshield {originalGoldShield} original hshield {originalHonorShield}  -  original tshield {originalTempShield} new temp shield {__instance.Get(Status.tempShield)} -  missing shield {missingShield}");

            // // Again, accessing __instance.statusEffects directly to avoid "on gain/lose temp shield" effects
            //if (missingShield > originalTempShield)
            //{
            //    throw new Exception($"Math isn't mathing. {{currentTempShield: {__instance.Get(Status.tempShield)}, originalTempShield: {originalTempShield}, originalHonorShield: {originalHonorShield}, originalGoldShield: {originalGoldShield}, missingShield: {originalTempShield - __instance.Get(Status.tempShield)}}}");
            //}
            //else if (missingShield == originalTempShield)
            //{
            //    __instance.statusEffects.Remove(Status.tempShield);
            //}
            //else
            //{
            //    __instance.statusEffects[Status.tempShield] = originalTempShield - missingShield;
            //}
        }

        // ///////////////////////////////////////////
        //
        // DRAWING
        //
        // ///////////////////////////////////////////


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Ship), nameof(Ship.RenderHealthBar))]
        //public static void CustomShieldDrawingPrePatch(Ship __instance, G g, bool isPreview, string keyPrefix)
        //{
        //    originalTempShield = __instance.Get(Status.tempShield);
        //    // I'm directly accessing the statusEffects dictionary rather than using Set in order to avoid triggering anyone's "on gain temp shield" effects, since this isn't really gaining temp shield, it's just hijacking tempShield to avoid needing to transpile Ship.NormalDamage 
        //    __instance.statusEffects[Status.tempShield] =
        //        originalTempShield
        //        + __instance.Get((Status)MainManifest.statuses["goldShield"].Id)
        //        + __instance.Get((Status)MainManifest.statuses["honorShield"].Id)
        //        - __instance.ghostTempShield;
        //}

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
                Color color4 = l > minRealTempIndex + goldShieldAmt
                    ? new Color("aaffaa")
                    : new Color("bbbb55"); 
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

            //// I'm directly accessing the statusEffects dictionary rather than using Set in order to avoid triggering anyone's "on gain temp shield" effects, since this isn't really gaining temp shield, it's just hijacking tempShield to avoid needing to transpile Ship.NormalDamage 
            //if (originalTempShield == 0) __instance.statusEffects.Remove(Status.tempShield);
            //else                         __instance.statusEffects[Status.tempShield] = originalTempShield;
        }


        //// TODO: use Kokoro instead of doing this
        //private static int gshield = 0;
        //private static int hshield = 0;
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Ship), nameof(Ship.RenderStatuses))]
        //public static void DontRenderCustomShieldsBelowHealthbar(Ship __instance, G g, string keyPrefix)
        //{
        //    gshield = __instance.Get((Status)MainManifest.statuses["goldShield"].Id);
        //    hshield = __instance.Get((Status)MainManifest.statuses["honorShield"].Id);
        //    __instance.statusEffects.Remove((Status)MainManifest.statuses["goldShield"].Id);
        //    __instance.statusEffects.Remove((Status)MainManifest.statuses["honorShield"].Id);
        //}
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Ship), nameof(Ship.RenderStatuses))]
        //public static void DontRenderCustomShieldsBelowHealthbar_Cleanup(Ship __instance, G g, string keyPrefix)
        //{
        //    if (gshield != 0) __instance.statusEffects[(Status)MainManifest.statuses["goldShield"].Id] = gshield;
        //    if (hshield != 0) __instance.statusEffects[(Status)MainManifest.statuses["honorShield"].Id] = hshield;
        //}
    }
}
