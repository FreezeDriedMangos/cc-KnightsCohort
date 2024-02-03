using KnightsCohort.Herbalist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.actions
{
    public class ACombineHerbs : CardAction
    {
        public int amount = 2;
        public bool selecting;
        public List<Card> selected = new();

        public override Icon? GetIcon(State s)
        {
            //return new Icon((Spr)MainManifest.sprites["icons/herb_bundle"].Id, amount, Colors.textMain);
            return null;
        }

        public override void Begin(G g, State s, Combat c)
        {
            if (selecting && selectedCard != null)
            {
                selected.Add(selectedCard);
                g.state.RemoveCardFromWhereverItIs(selectedCard.uuid);
            }

            if (amount > 0)
            {
                c.QueueImmediate(
                    new AHerbCardSelect
                    {
                        browseAction = new ACombineHerbs() { selected = selected, amount = amount-1, selecting = true },
                        browseSource = Enum.Parse<CardBrowse.Source>("Deck"),
                        omit = selected,
                        //rawOnly = true
                        excludePoultices = true // less restrictive, I think it's better like this
                    }
                );
            }

            if (amount == 0)
            {
                HerbCard_Poultice poultice = new HerbCard_Poultice();
                poultice.SerializedActions = new();
                poultice.name = "Poultice";
                poultice.revealed = true;
                poultice.isPoultice = true;
                foreach(Card card in selected)
                {
                    if (card is not HerbCard herb) { throw new Exception("Non herb card put into ACombineHerbs!"); }
                    poultice.SerializedActions.AddRange(herb.SerializedActions);
                    poultice.isTea = poultice.isTea || herb.isTea;
                    poultice.isCultivated = poultice.isCultivated || herb.isCultivated;
                }

                c.QueueImmediate(new AAddCard() { card = poultice, destination = Enum.Parse<CardDestination>("Hand")});
            }
        }
    }
}
