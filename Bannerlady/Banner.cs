using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace KnightsCohort.Bannerlady
{
    [HarmonyPatch]
    public class Banner : StuffBase
    {
        public static readonly int MIDROW_SPRITE_WIDTH = 17;
        public static readonly int MIDROW_SPRITE_HEIGHT = 33;
        public static readonly int BANNER_ANIMATION_NUM_FRAMES = 6;
        public static readonly double BANNER_ANIMATION_SPEED = 7;
        public override bool Invincible() { return true; }

        //public override List<CardAction>? GetActionsOnDestroyed(State s, Combat c, bool wasPlayer, int worldX)
        //{
        //    return GetActionsOnShotWhileInvincible(s, c, wasPlayer, 0);
        //}

        //public override List<CardAction>? GetActionsOnShotWhileInvincible(State s, Combat c, bool wasPlayer, int damage)
        //{
        //    if (s.ship.Get((Status)MainManifest.statuses["shieldOfFaith"].Id) > 0 && !wasPlayer) return new() { };

        //    if (c.currentCardAction is AAttack hit)
        //    {
        //        AAttack aattack = Mutil.DeepCopy(hit);

        //        aattack.fromDroneX = this.x;
        //        aattack.fast = true;

        //        ModifyAction(aattack, s, c, wasPlayer);

        //        //return new() { aattack };
        //        return new() { };
        //    }

        //    MainManifest.Instance.Logger.LogError("Banner was shot with non attack " + c.currentCardAction?.GetType().FullName);
        //    return new() { };
        //}


        public static int? AAttackGetFromX(AAttack a, State s, Combat c)
        {
            if (a.fromX.HasValue)
            {
                return a.fromX;
            }
            int num = (a.targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
            if (num != -1)
            {
                return num;
            }
            return null;
        }

        static StuffBase removedBanner = null;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
        public static void RestoreNontatteredBanner(AAttack __instance, G g, State s, Combat c)
        {
            if (removedBanner == null) return;
            c.stuff[removedBanner.x] = removedBanner;
            removedBanner = null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
        public static void BannerModifyAAttack(AAttack __instance, G g, State s, Combat c)
        {
            if (__instance.fromDroneX.HasValue) return;

            Ship fromShip = (__instance.targetPlayer ? c.otherShip : s.ship);
            var x = fromShip.x + Banner.AAttackGetFromX(__instance, s, c) ?? 0;

            if (c.stuff.ContainsKey(x) && c.stuff[x] is Banner banner && s.ship.Get((Status)MainManifest.statuses["shieldOfFaith"].Id) <= 0)
            {
                banner.ModifyAction(__instance, s, c, !__instance.targetPlayer);

                if (!banner.Invincible())
                {
                    banner.DoDestroyedEffect(s, c);
                    c.stuff.Remove(x);

                    if (!__instance.targetPlayer)
                    {
                        foreach (Artifact artifact in s.EnumerateAllArtifacts())
                        {
                            artifact.OnPlayerDestroyDrone(s, c);
                        }
                    }
                }
                else
                {
                    // temporarily remove banner to make AAttack behave
                    removedBanner = banner;
                    c.stuff.Remove(x);
                }
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CombatUtils), nameof(CombatUtils.RaycastGlobal))]
        public static void AttackPassthroughBannerPatch(RaycastResult __result, Combat c, Ship target, bool fromDrone, int worldX)
        {
            if (c.stuff.ContainsKey(worldX) && c.stuff[worldX] is Banner)
            {
                __result.hitShip = target.HasNonEmptyPartAtWorldX(worldX);
                __result.hitDrone = false;
            }
        }

        public virtual Spr GetSprite()
        {
            return (Spr)MainManifest.sprites["midrow/banner_of_mercy"].Id;
        }

        public virtual void ModifyAction(AAttack aattack, State s, Combat c, bool wasPlayer) {}

        public override void Render(G g, Vec v)
        {
            Color exhaustColor = new Color(1, 1, 1);
            
            Vec vec3 = default(Vec);
            vec3 += new Vec(0.0, 21.0);

            Vec vec4 = v + vec3 + new Vec(7.0, 8.0);

            int frame = (int)Math.Truncate((-this.x + g.state.time*BANNER_ANIMATION_SPEED) % BANNER_ANIMATION_NUM_FRAMES);
            DrawWithHilight(g, GetSprite(), v, pixelRect:new Rect(frame*MIDROW_SPRITE_WIDTH, 0, MIDROW_SPRITE_WIDTH, MIDROW_SPRITE_HEIGHT));

            // this is for the missile's exhaust (this render function is based on missile's render function)
            //Glow.Draw(vec4 + new Vec(0.5, -2.5), 25.0, exhaustColor * new Color(1.0, 0.5, 0.5).gain(0.2 + 0.1 * Math.Sin(g.state.time * 30.0 + (double)x) * 0.5));
        }

        // stolen right from StuffBase, but I added the pixelRect argument to support spritesheet animations
        public void DrawWithHilight(G g, Spr id, Vec v, bool flipX = false, bool flipY = false, Rect? pixelRect = null)
        {
            if (ShouldDrawHilight(g))
            {
                Texture2D? outlined = SpriteLoader.GetOutlined(id);
                double num = v.x - 2.0;
                double y = v.y - 2.0;
                BlendState screen = BlendMode.Screen;
                Color? color = Colors.droneOutline;
                Draw.Sprite(outlined, num, y, flipX, flipY, 0.0, null, null, null, pixelRect, color, screen);
            }
            Draw.Sprite(id, v.x - 1.0, v.y - 1.0, flipX, flipY, pixelRect: pixelRect);
        }
    }
}
