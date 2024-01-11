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

        public override void Begin(G g, State s, Combat c)
        {
            var totalDist = c.otherShip.x - s.ship.x;
            
            c.QueueImmediate(new AMove() { dir = Math.Min(distance, Math.Abs(totalDist)) * Math.Sign(totalDist), targetPlayer = true });
        }

        public override Icon? GetIcon(State s)
        {
            // TODO: add a charge left and charge right and choose the appropriate icon depending on which direction the ship will actually move
            return new Icon((Spr)MainManifest.sprites["icons/charge"].Id, distance, Colors.textMain);
        }
    }
}
