using HarmonyLib;
using KnightsCohort.Bannerlady.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class ACharge : CardAction
    {
        public int dir = 0;
        protected virtual string NonDirectionalSprite => "icons/charge";
        protected virtual string DirectionalSprite => "icons/charge_directional";

        public static double GetCentralX(Ship ship)
        {
            return ship.x + ship.parts.Count / 2.0;
        }

        public static int GetDir(int distance, State s, Combat c)
        {
            if (s.route is not Combat) return distance;

            var totalDist = (int)Math.Ceiling(GetCentralX(c.otherShip) - GetCentralX(s.ship));
            return Math.Min(distance, Math.Abs(totalDist)) * Math.Sign(totalDist);
        }

        public override void Begin(G g, State s, Combat c)
        {
            c.QueueImmediate(new AMove() { dir = dir, targetPlayer = true });
        }

        public override Icon? GetIcon(State s)
        {
            // if (s.route is Combat c && ((s.routeOverride == null && c.routeOverride == null) || c.eyeballPeek))
            if (s.route is Combat c)
            {
                if (dir == 0) return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, Math.Abs(dir), Colors.textMain);
                if (dir < 0) return new Icon((Spr)MainManifest.sprites[DirectionalSprite].Id, Math.Abs(dir), Colors.textMain);
                if (dir > 0) return new Icon((Spr)MainManifest.sprites[DirectionalSprite+"_right"].Id, Math.Abs(dir), Colors.textMain);
            }

            return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, Math.Abs(dir), Colors.textMain);
        }

        public override List<Tooltip> GetTooltips(State s) => new() { new TTGlossary(MainManifest.glossary["charge"].Head, Math.Abs(dir)) };
    }

    public class ARetreat : ACharge
    {
        protected override string NonDirectionalSprite => "icons/retreat";
        protected override string DirectionalSprite => "icons/retreat_directional";


        public static new int GetDir(int distance, State s, Combat c)
        {
            if (s.route is not Combat) return distance;

            var totalDist = -(int)Math.Ceiling(GetCentralX(c.otherShip) - GetCentralX(s.ship));
            if (totalDist == 0) totalDist = 1;
            return distance * Math.Sign(totalDist);
        }

        public override void Begin(G g, State s, Combat c)
        {
            c.QueueImmediate(new AMove() { dir = dir, targetPlayer = true });
        }

        public override List<Tooltip> GetTooltips(State s) => new() { new TTGlossary(MainManifest.glossary["retreat"].Head, Math.Abs(dir)) };

        public override Icon? GetIcon(State s)
        {
            // if (s.route is Combat c && ((s.routeOverride == null && c.routeOverride == null) || c.eyeballPeek))
            if (s.route is Combat c)
            {
                if (dir == 0) return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, Math.Abs(dir), Colors.textMain);
                if (dir < 0)  return new Icon((Spr)MainManifest.sprites[DirectionalSprite].Id, Math.Abs(dir), Colors.textMain);
                if (dir > 0)  return new Icon((Spr)MainManifest.sprites[DirectionalSprite + "_right"].Id, Math.Abs(dir), Colors.textMain);
            }

            return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, Math.Abs(dir), Colors.textMain);
        }
    }
}
