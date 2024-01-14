using HarmonyLib;
using KnightsCohort.Knight.Midrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Knight.Cards
{
    [HarmonyPatch]
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RiposteCard : Card
    {
        public static bool RiposteReady = false;
        public static bool RiposteTwice = false;
        public static int RiposteDamage = 2;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AReadyRiposte() { card = this, ready = true, twice = upgrade == Upgrade.B },
                new AAttack() { damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1) },
                new AReadyRiposte() { card = this, ready = false },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, description = $"Attack for {GetDmg(state, upgrade == Upgrade.A ? 2 : 1)}. Attack again for {GetDmg(state, RiposteDamage)}{(upgrade == Upgrade.B ? $" and again for {GetDmg(state, RiposteDamage)}" : "")} if the hit part intends to attack." };
        }

        public class AReadyRiposte : CardAction
        {
            public Card card;
            public bool ready = true;
            public bool twice = false;
            public override void Begin(G g, State s, Combat c)
            {
                RiposteCard.RiposteReady = ready;
                RiposteCard.RiposteTwice = twice;
                RiposteCard.RiposteDamage = card?.GetDmg(s, 2) ?? 2;
            }
            public override Icon? GetIcon(State s) { return null; }
        }

        public static bool AttackWillBeVolley(AAttack __instance, G g) => !__instance.targetPlayer && !__instance.fromDroneX.HasValue && g.state.ship.GetPartTypeCount(PType.cannon) > 1 && !__instance.multiCannonVolley;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
        public static void HarmonyPostfix_RiposteController(AAttack __instance, G g, State s, Combat c)
        {
            if (!RiposteReady) return;
            if (__instance.targetPlayer) return;
            if (AttackWillBeVolley(__instance, g)) return;

            Part? hitPart = VowsController.AAttackPostfix_GetHitShipPart(__instance, s, c);
            if (hitPart == null) return;
            if (hitPart.type == Enum.Parse<PType>("empty")) return;

            if (hitPart.intent is IntentAttack)
            {
                c.Queue(new AAttack()
                {
                    damage = RiposteDamage,
                    fromX = __instance.fromX,
                    multiCannonVolley = true // don't trigger another volley, so that only attacking parts are hit twice
                });
            }
        }
    }
}
