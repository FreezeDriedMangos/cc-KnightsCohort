using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class ADie : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            s.ship.hull = 0;
        }
    }
}
