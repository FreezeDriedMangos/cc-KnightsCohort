using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class ACharge : CardAction
    {
        public int distance = 0;
        protected virtual string NonDirectionalSprite => "icons/charge";
        protected virtual string DirectionalSprite => "icons/charge_directional";

        public static double GetCentralX(Ship ship)
        {
            return ship.x + ship.parts.Count / 2.0;
        }

        public virtual int GetDir(State s, Combat c)
        {
            // TODO: calculate this from the middle part of each ship, not the leftmost (what ship.x represents)
            var totalDist = (int)Math.Ceiling(GetCentralX(c.otherShip) - GetCentralX(s.ship));
            return Math.Min(distance, Math.Abs(totalDist)) * Math.Sign(totalDist);
        }

        public override void Begin(G g, State s, Combat c)
        {
            c.QueueImmediate(new AMove() { dir = GetDir(s, c), targetPlayer = true });
        }

        public override Icon? GetIcon(State s)
        {
            // if (s.route is Combat c && ((s.routeOverride == null && c.routeOverride == null) || c.eyeballPeek))
            if (s.route is Combat c)
            {
                var dir = GetDir(s, c);
                if (dir == 0) return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, dir, Colors.textMain);
                if (dir < 0) return new Icon((Spr)MainManifest.sprites[DirectionalSprite].Id, -dir, Colors.textMain);
                if (dir > 0) return new Icon((Spr)MainManifest.sprites[DirectionalSprite+"_right"].Id, dir, Colors.textMain);
            }

            return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, distance, Colors.textMain);
        }

        public override List<Tooltip> GetTooltips(State s) => new() { new TTGlossary(MainManifest.glossary["charge"].Head, distance) };
    }

    [HarmonyPatch]
    public class ARetreat : ACharge
    {
        public static int TiebreakerDirection = 1;
        public int tiebreakerDirection = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
        public static void TrackPlaying(Combat __instance, State s, Card card, bool playNoMatterWhatForFree = false, bool exhaustNoMatterWhat = false)
        {
            TiebreakerDirection = -Math.Sign(GetCentralX(__instance.otherShip) - GetCentralX(s.ship));
            if (TiebreakerDirection == 0) TiebreakerDirection = 1;
        }

        public ARetreat() : base()
        {
            if (tiebreakerDirection == 0) tiebreakerDirection = TiebreakerDirection;
        }

        protected override string NonDirectionalSprite => "icons/retreat";
        protected override string DirectionalSprite => "icons/retreat_directional";

        public override void Begin(G g, State s, Combat c)
        {
            var direction = -Math.Sign(GetCentralX(c.otherShip) - GetCentralX(s.ship));
            if (direction == 0) direction = tiebreakerDirection;

            c.QueueImmediate(new AMove() { dir = distance * direction, targetPlayer = true });
        }

        public override List<Tooltip> GetTooltips(State s) => new() { new TTGlossary(MainManifest.glossary["retreat"].Head, distance) };

        public override Icon? GetIcon(State s)
        {
            // if (s.route is Combat c && ((s.routeOverride == null && c.routeOverride == null) || c.eyeballPeek))
            if (s.route is Combat c)
            {
                var direction = -Math.Sign(GetCentralX(c.otherShip) - GetCentralX(s.ship));
                if (direction < 0) return new Icon((Spr)MainManifest.sprites[DirectionalSprite].Id, distance, Colors.textMain);
                else               return new Icon((Spr)MainManifest.sprites[DirectionalSprite + "_right"].Id, distance, Colors.textMain);
            }

            return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, distance, Colors.textMain);
        }
    }
}
