using FSPRO;
using HarmonyLib;
using KnightsCohort.actions;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Treasurer
{
    [HarmonyPatch]
    public class InvestmentCard : Card
    {
        public static Status GoldStatus => (Status)MainManifest.statuses["gold"].Id;

        public virtual List<int> upgradeCosts => new() { 3, 4 };
        public int tier = 0;

        protected virtual List<Spr> tierArt => upgradeCosts.Count == 1
            ? new()
                {
                    (Spr)MainManifest.sprites["cards/treasurer/card_default_treasurer_investment_2_tier_1"].Id,
                    (Spr)MainManifest.sprites["cards/treasurer/card_default_treasurer_investment_2_tier_2"].Id,
                }
            : new()
                {
                    (Spr)MainManifest.sprites["cards/treasurer/card_default_treasurer_investment_tier_1"].Id,
                    (Spr)MainManifest.sprites["cards/treasurer/card_default_treasurer_investment_tier_2"].Id,
                    (Spr)MainManifest.sprites["cards/treasurer/card_default_treasurer_investment_tier_3"].Id,
                };
        protected virtual List<List<CardAction>> GetTierActions(State s, Combat c) { return new() { new(), new(), new() }; }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.FlipCardInHand))]
        public static void InvestOnRightClickPatch(Combat __instance, G g, Card card)
        {
            // TODO: acts weird when Ruby's missing
            if (card is InvestmentCard icard)
            {
                icard.TryInvest(g.state, __instance);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.PlayerWon))]
        public static void ClearInvestmentsOnWin(G g)
        {
            g.state.deck.ForEach(c =>
            {
                if (c is InvestmentCard icard) icard.tier = 0;
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.PlayerLost))]
        public static void ClearInvestmentsOnLoss(G g)
        {
            g.state.deck.ForEach(c =>
            {
                if (c is InvestmentCard icard) icard.tier = 0;
            });
        }


        public void TryInvest(State s, Combat c)
        {
            var gold = s.ship.Get(GoldStatus);
            if (upgradeCosts.Count <= tier || upgradeCosts[tier] > gold)
            {
                this.shakeNoAnim = 1.0;
                Audio.Play(Event.ZeroEnergy);
                return;
            }

            s.ship.Set(GoldStatus, gold - upgradeCosts[tier]);
            tier++;
        }

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> allActions = new() { new ADummyAction() };

            var tieredActions = GetTierActions(s, c);
            for (int t = 0;  t < tieredActions.Count; t++)
            {
                tieredActions[t].ForEach(a => a.disabled = t > tier);
                allActions.AddRange(tieredActions[t]);
                allActions.Add(new ADummyAction());
            }

            allActions.Add(new ATooltipDummy {
                tooltips = upgradeCosts.Count == 2
                    ? new() { new TTGlossary(MainManifest.glossary["3tierinvestmentcard"].Head, upgradeCosts[0], upgradeCosts[1]) }
                    : new() { new TTGlossary(MainManifest.glossary["2tierinvestmentcard"].Head, upgradeCosts[0]) }
            });
            allActions.Insert(0, new ADummyAction());

            return allActions;
        }

        public override CardData GetData(State state)
        {
            CardData d = base.GetData(state);
            d.art = tierArt[tier];
            d.artTint = "ffffff";
            return d;
        }

        public override void ExtraRender(G g, Vec v)
        {
            // TODO: should return early when ruby's missing

            base.ExtraRender(g, v);
            int gold = g.state.ship.Get(GoldStatus);

            int startingY = this.upgradeCosts.Count switch
            {
                1 => 42,
                2 => 32,
                _ => 0
            };
            int tierSeparation = 20;

            for (int t = 0; t < this.upgradeCosts.Count; t++)
            {
                //var cache = g.mg.GraphicsDevice.GetRenderTargets();
                //var renderTarget = new RenderTarget2D(g.mg.GraphicsDevice, 20, 8);
                //g.mg.GraphicsDevice.SetRenderTarget(renderTarget);

                for (int i = 0; i < this.upgradeCosts[t]; i++)
                {
                    // behold my accursed nested ternary
                    // muahahahaha
                    // but seriously this is bad code and I'm going to regret it eventually
                    string iconPath = t < tier
                        ? "icons/gold_1_paid"
                        : t > tier
                            ? "icons/gold_1_locked"
                            : gold <= i
                                ? "icons/gold_1_unmet"
                                : "icons/gold_1_met";

                    Draw.Sprite((Spr)MainManifest.sprites[iconPath].Id, i*3 + v.x + 3, v.y + startingY + t*tierSeparation);

                    if (iconPath != "icons/gold_1_paid")
                    {
                        var outlinePath = "icons/gold_1_outline_middle";
                        if (i == 0) outlinePath = "icons/gold_1_outline_left";
                        else if (i == this.upgradeCosts[t] - 1) outlinePath = "icons/gold_1_outline_right";

                        if (this.upgradeCosts[t] == 1) outlinePath = "icons/gold_1_outline_full";

                        Color outlineColor = iconPath switch
                        {
                            "icons/gold_1_unmet" => new Color("41aac9"),
                            "icons/gold_1_met" => new Color("41aac9"),
                            "icons/gold_1_locked" => new Color("042d39"),
                            _ => new Color("fff")
                        };

                        Draw.Sprite((Spr)MainManifest.sprites[outlinePath].Id, i * 3 + v.x + 3, v.y + startingY + t * tierSeparation, color: outlineColor);

                    }


                    //Draw.Sprite((Spr)MainManifest.sprites[iconPath].Id, i*3, 0);
                }

                //g.mg.GraphicsDevice.SetRenderTargets(cache);
                //var outlined = SpriteUtil.MakeOutline(renderTarget);
                //renderTarget.Dispose();
                //Draw.Sprite(outlined, 3, v.y + startingY + t * tierSeparation);
                //outlined.Dispose();
            }
        }


        public static Vec GetShakeOffset(Card __instance, G g)
        {
            Vec vec = new Vec(Math.Round(Math.Sin(__instance.shakeNoAnim * Math.PI * 2.0 * 4.0) * (__instance.shakeNoAnim * 1.5)));
            if (__instance.GetMeta().deck == Deck.corrupted)
            {
                double num = Mutil.Rand(Math.Round(g.state.time * 18.0) + __instance.seed);
                if (num < 0.1)
                {
                    return vec + (new Vec(Mutil.Rand(num) - 0.5, Mutil.Rand(num + 0.1) - 0.5) * 3.0).round();
                }
            }
            return vec;
        }

    }
}
