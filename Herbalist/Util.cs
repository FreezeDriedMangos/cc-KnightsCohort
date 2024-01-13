using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Herbalist
{
    public class Util
    {

        public static Rarity CardReward_GetRandomRarity(Rand rng, BattleType battleType)
        {
            return battleType switch
            {
                BattleType.Boss => Rarity.rare,
                BattleType.Elite => Mutil.Roll(rng.Next(), (0.35, Rarity.common), (0.45, Rarity.uncommon), (0.2, Rarity.rare)),
                _ => Mutil.Roll(rng.Next(), (0.75, Rarity.common), (0.2, Rarity.uncommon), (0.05, Rarity.rare)),
            };
        }

        // TODO: this doesn't work
        public static HerbCard GenerateRandomHerbCard(State s)
        {
            //if (s.map.markers[s.map.currentLocation].contents is MapBattle mapBattle) mapBattle.battleType
            Rarity rarity = CardReward_GetRandomRarity(s.rngCardOfferings, Enum.Parse<BattleType>("Normal"));

            List<Card> offerableCards = DB.releasedCards.Where(delegate (Card c)
            {
                CardMeta meta = c.GetMeta();
                if (meta.rarity != rarity)
                {
                    return false;
                }
                if (meta.deck != (Deck)MainManifest.decks["herbs"].Id)
                {
                    return false;
                }
                if (meta.dontOffer)
                {
                    return false;
                }
                if (meta.unreleased)
                {
                    return false;
                }
                return true;
            }).ToList();

            if (offerableCards.Count <= 0) return new();

            var offeredCard = offerableCards.KnightRandom(s.rngCardOfferings);
            Card card = (Card)Activator.CreateInstance(offeredCard.GetType());

            return HerbCard.Generate(s, card as HerbCard);
        }
    }
}
