using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class AHonorShield : ACustomTempShield
    {
        public override string StatusName() => "honorShield";
        public override string IconSprite() => "icons/honor_shield";
    }
    public class AGoldShield : ACustomTempShield
    {
        public override string StatusName() => "goldShield";
        public override string IconSprite() => "icons/gold_shield";
    }

    public abstract class ACustomTempShield : CardAction
    {
        public int amount = 1;

        public abstract string StatusName();
        public abstract string IconSprite();

        public override void Begin(G g, State s, Combat c)
        {
            c.QueueImmediate(new AStatus() { status = Status.tempShield, statusAmount = amount, targetPlayer = true });
            c.QueueImmediate(new AStatus() { status = (Status)MainManifest.statuses[StatusName()].Id, statusAmount = amount, targetPlayer = true });
        }

        public override Icon? GetIcon(State s)
        {
            return new Icon((Spr)MainManifest.sprites[IconSprite()].Id, amount, Colors.textMain);
        }

        public override List<Tooltip> GetTooltips(State s) => (new AStatus() { status = (Status)MainManifest.statuses[StatusName()].Id, statusAmount = amount, targetPlayer = true }).GetTooltips(s);
    }
}
