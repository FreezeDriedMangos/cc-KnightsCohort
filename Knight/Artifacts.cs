using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Knight.Artifacts
{
    public class HolyGrail : Artifact { }
    public class PeaceDove : Artifact 
    {
        public override void OnTurnStart(State state, Combat combat)
        {
            state.ship.Add((Status)MainManifest.statuses["vowOfMercy"].Id);
        }
    }
}
