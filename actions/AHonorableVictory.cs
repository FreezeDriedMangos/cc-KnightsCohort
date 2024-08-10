using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class AHonorableVictory : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            if (c.otherShip.hull <= 0) return;
            if (s.ship.hull <= 0) return;

            c.QueueImmediate([
                new AMidCombatDialogue
                {
                    script = "clay.KnightsCohort.Honorable_Win", // make this randomly pick a line from a list of multiple for each of the 3 knights
                    canRunAfterKill = true,
                },
                new ADelay
                {
                    time = 0.0,
                    timer = 0.7,
                    //canRunAfterKill = true,
                },
                new AEscape
                {
                    targetPlayer = false,
                    canRunAfterKill = true,
                }
            ]);
        }
    }
}
