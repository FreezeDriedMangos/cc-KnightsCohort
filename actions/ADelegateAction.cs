using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class ADelegateAction : CardAction
    {
        public delegate void OnBegin(CardAction instance, G g, State s, Combat c);
        public OnBegin onBegin;

        public override void Begin(G g, State s, Combat c)
        {
            if (onBegin == null) return;
            onBegin(this, g, s, c);
        }
    }
}
