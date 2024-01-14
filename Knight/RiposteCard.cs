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
            var riposteDamage = upgrade == Upgrade.A ? 1 : 2;
            return new()
            {
                new AReadyRiposte() { ready = true, twice = upgrade == Upgrade.A, dmg = GetDmg(s, riposteDamage) },
                new AAttack() { damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1) },
                new AReadyRiposte() { ready = false },
            };
        }
        public override CardData GetData(State state)
        {
            var riposteDamage = upgrade == Upgrade.A ? 1 : 2;
            return new() { cost = 1, 
                description = upgrade == Upgrade.A
                ? $"Attack for {GetDmg(state, 1)}, then twice for {GetDmg(state, riposteDamage)} if the hit part intends to attack."
                : $"Attack for {GetDmg(state, upgrade == Upgrade.B ? 2 : 1)}, then again for {GetDmg(state, riposteDamage)} if the hit part intends to attack."
            };
        }

        public class AReadyRiposte : CardAction
        {
            public bool ready = true;
            public bool twice = false;
            public int dmg = 1;
            public override void Begin(G g, State s, Combat c)
            {
                RiposteCard.RiposteReady = ready;
                RiposteCard.RiposteTwice = twice;
                RiposteCard.RiposteDamage = dmg;
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
