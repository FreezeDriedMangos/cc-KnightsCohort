using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Treasurer.Artifacts
{
    public class DragonsHoard : Artifact
    {
        public static readonly int INCOME_RATE = 8;
        public override void OnTurnStart(State state, Combat combat)
        {
            int hoardSize = state.deck.Count / INCOME_RATE;
            state.ship.Add((Status)MainManifest.statuses["gold"].Id, hoardSize);
        }
    }
}
