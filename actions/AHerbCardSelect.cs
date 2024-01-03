using HarmonyLib;
using KnightsCohort.Herbalist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    [HarmonyPatch]
    public class AHerbCardSelect : ACardSelect
    {
        public List<Card> omit = new();

        public override Route? BeginWithRoute(G g, State s, Combat c)
        {
            herbsOnly = true;
            omitThese = omit;
            var res = base.BeginWithRoute(g, s, c);
            omitThese = new();
            herbsOnly = false;

            return res;
        }

        public override Icon? GetIcon(State s)
        {
            //return new Icon((Spr)MainManifest.sprites["icons/search_herb"].Id, null, Colors.textMain);
            return null;
        }

        public static bool herbsOnly = false;
        public static List<Card> omitThese = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardBrowse), nameof(CardBrowse.GetCardList))]
        public static void GetCardList(List<Card> __result, G g)
        {
            if (!herbsOnly) return;

            List<Card> herbs = new();
            foreach (var card in __result)
            {
                if (card is HerbCard && !omitThese.Contains(card)) herbs.Add(card);
            }

            __result.Clear();
            __result.AddRange(herbs);
        }
    }
}
