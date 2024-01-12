using HarmonyLib;
using KnightsCohort.Herbalist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class HerbCardBrowse : CardBrowse { public List<Card> omit = new(); }

    [HarmonyPatch]
    public class AHerbCardSelect : ACardSelect
    {
        public List<Card> omit = new();

        public override Route? BeginWithRoute(G g, State s, Combat c)
        {
            HerbCardBrowse cardBrowse = new HerbCardBrowse
            {
                mode = CardBrowse.Mode.Browse,
                browseSource = browseSource,
                browseAction = browseAction,
                filterUnremovableAtShops = filterUnremovableAtShops,
                filterExhaust = filterExhaust,
                filterRetain = filterRetain,
                filterBuoyant = filterBuoyant,
                filterTemporary = filterTemporary,
                includeTemporaryCards = includeTemporaryCards,
                filterOutTheseRarities = filterOutTheseRarities,
                filterMinCost = filterMinCost,
                filterMaxCost = filterMaxCost,
                filterUpgrade = filterUpgrade,
                filterAvailableUpgrade = filterAvailableUpgrade,
                filterUUID = filterUUID,
                ignoreCardType = ignoreCardType,
                allowCancel = allowCancel,
                allowCloseOverride = allowCloseOverride,
                omit = omit
            };
            c.Queue(new ADelay
            {
                time = 0.0,
                timer = 0.0
            });
            if (cardBrowse.GetCardList(g).Count == 0)
            {
                timer = 0.0;
                return null;
            }
            return cardBrowse;
        }

        public override Icon? GetIcon(State s)
        {
            //return new Icon((Spr)MainManifest.sprites["icons/search_herb"].Id, null, Colors.textMain);
            return null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardBrowse), nameof(CardBrowse.GetCardList))]
        public static void GetCardList(CardBrowse __instance, ref List<Card> __result, G g)
        {
            if (__instance is not HerbCardBrowse hcb) return;

            List<Card> herbs = new();
            foreach (var card in __result)
            {
                if (card is HerbCard && !hcb.omit.Contains(card)) herbs.Add(card);
            }

            __result.Clear();
            __result.AddRange(herbs);
        }
    }
}
