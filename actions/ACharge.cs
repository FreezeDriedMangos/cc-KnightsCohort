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

        public override void Begin(G g, State s, Combat c)
        {
            var totalDist = c.otherShip.x - s.ship.x;

            c.QueueImmediate(new AMove() { dir = Math.Min(distance, Math.Abs(totalDist)) * Math.Sign(totalDist), targetPlayer = true });
        }

        public override Icon? GetIcon(State s)
        {
            // if (s.route is Combat c && ((s.routeOverride == null && c.routeOverride == null) || c.eyeballPeek))
            if (s.route is Combat c)
            {
                var totalDist = c.otherShip.x - s.ship.x;
                if (totalDist == 0) return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, distance, Colors.textMain);
                if (totalDist < 0) return new Icon((Spr)MainManifest.sprites[DirectionalSprite].Id, distance, Colors.textMain);
                if (totalDist > 0) return new Icon((Spr)MainManifest.sprites[DirectionalSprite+"_right"].Id, distance, Colors.textMain);
            }

            return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, distance, Colors.textMain);
        }

        public override List<Tooltip> GetTooltips(State s) => new() { new TTGlossary(MainManifest.glossary["charge"].Head, distance) };
    }

    public class ARetreat : ACharge
    {
        protected override string NonDirectionalSprite => "icons/retreat";
        protected override string DirectionalSprite => "icons/retreat_directional";

        public override void Begin(G g, State s, Combat c)
        {
            var direction = -Math.Sign(c.otherShip.x - s.ship.x);
            if (direction == 0) direction = 1;

            c.QueueImmediate(new AMove() { dir = distance * direction, targetPlayer = true });
        }

        public override List<Tooltip> GetTooltips(State s) => new() { new TTGlossary(MainManifest.glossary["retreat"].Head, distance) };

        public override Icon? GetIcon(State s)
        {
            // if (s.route is Combat c && ((s.routeOverride == null && c.routeOverride == null) || c.eyeballPeek))
            if (s.route is Combat c)
            {
                var totalDist = c.otherShip.x - s.ship.x;
                if (totalDist < 0) return new Icon((Spr)MainManifest.sprites[DirectionalSprite].Id, distance, Colors.textMain);
                else               return new Icon((Spr)MainManifest.sprites[DirectionalSprite + "_right"].Id, distance, Colors.textMain);
            }

            return new Icon((Spr)MainManifest.sprites[NonDirectionalSprite].Id, distance, Colors.textMain);
        }
    }
}
