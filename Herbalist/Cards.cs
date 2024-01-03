using KnightsCohort.actions;
using KnightsCohort.Knight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Herbalist.Cards
{

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class MortarAndPestle : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new ACombineHerbs() { amount = 2, selected = new() { new HerbCard() { SerializedActions = new() { HerbActions.OXIDATION } } } },
               new ATooltipDummy() { tooltips = new(), icons = new() { new Icon((Spr)MainManifest.sprites["icons/herb_bundle_add_oxidize"].Id, 1, Colors.textMain) } }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Smolder : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new AHerbCardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                    browseAction = new AExhaustSelectedCard()
                },
                new ATooltipDummy()
                {
                    tooltips = new() {},
                    icons = new() 
                    {
                        // todo: change to icons/herb_search
                        new Icon((Spr)MainManifest.sprites["icons/herb_bundle"].Id, null, Colors.textMain),
                        new Icon(Enum.Parse<Spr>("icons_exhaust"), null, Colors.textMain),
                    }
                },
                new ADummyAction(),
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LeafPack : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Leaf()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Leaf()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Leaf()) },
               new AAddCard() { card = HerbCard.Generate(s, new HerbCard_Leaf()) },
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 0, description = "Permanently add 4 random leaf herb cards to your deck.", singleUse = true };
        }
    }
}
