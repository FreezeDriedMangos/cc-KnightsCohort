using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Treasurer.Artifacts
{

    // TODO: if I go with "honor/gold shield only covers one part and doesn't expire" make an artifact that makes them cover the whole ship but tick down by 1 at the end of the turn

    // TODO: should trigger every other turn
    public class DragonsHoard : Artifact
    {
        public static readonly int INCOME_RATE = 8;
        public override void OnTurnStart(State state, Combat combat)
        {
            state.ship.Add((Status)MainManifest.statuses["gold"].Id, this.GetDisplayNumber(state) ?? 0);
            this.Pulse();
        }

        public override int? GetDisplayNumber(State state)
        {
            int deckSize = state.deck.Where(c => !c.GetDataWithOverrides(state).temporary).Count();

            if (state.route is Combat combat)
            {
                deckSize +=
                      combat.hand.Where(c => !c.GetDataWithOverrides(state).temporary).Count()
                    + combat.discard.Where(c => !c.GetDataWithOverrides(state).temporary).Count()
                    + combat.exhausted.Where(c => !c.GetDataWithOverrides(state).temporary).Count();
            }

            int hoardSize = deckSize / INCOME_RATE;
            return hoardSize;
        }
    }
}
