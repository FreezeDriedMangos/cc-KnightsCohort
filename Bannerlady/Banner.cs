using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace KnightsCohort.Bannerlady
{
    public class Banner : StuffBase
    {
        public static readonly int MIDROW_SPRITE_WIDTH = 17;
        public static readonly int MIDROW_SPRITE_HEIGHT = 33;
        public static readonly int BANNER_ANIMATION_NUM_FRAMES = 6;
        public static readonly double BANNER_ANIMATION_SPEED = 7;
        public override bool Invincible() { return true; }

        public override List<CardAction>? GetActionsOnDestroyed(State s, Combat c, bool wasPlayer, int worldX)
        {
            return GetActionsOnShotWhileInvincible(s, c, wasPlayer, 0);
        }

        public override List<CardAction>? GetActionsOnShotWhileInvincible(State s, Combat c, bool wasPlayer, int damage)
        {
            if (s.ship.Get((Status)MainManifest.statuses["shieldOfFaith"].Id) > 0 && !wasPlayer) return new() { };

            if (c.currentCardAction is AAttack hit)
            {
                AAttack aattack = Mutil.DeepCopy(hit);

                aattack.fromDroneX = this.x;
                aattack.fast = true;

                ModifyAction(aattack, s, c, wasPlayer);

                return new() { aattack };
            }

            MainManifest.Instance.Logger.LogError("Banner was shot with non attack " + c.currentCardAction?.GetType().FullName);
            return new() { };
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
