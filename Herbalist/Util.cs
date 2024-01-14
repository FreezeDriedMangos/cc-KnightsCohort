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

        public static HerbCard GenerateRandomHerbCard(State s)
        {
            Rarity rarity = CardReward_GetRandomRarity(s.rngCardOfferings, Enum.Parse<BattleType>("Normal"));

            List<HerbCard> offerableCards = rarity switch
            {
                Rarity.common => new() { new HerbCard_Leaf(), new HerbCard_Bark() },
                Rarity.uncommon => new() { new HerbCard_Seed(), new HerbCard_Root() },
                Rarity.rare => new() { new HerbCard_Shroom() },
            };

            return HerbCard.Generate(s, offerableCards.KnightRandom(s.rngCardOfferings));
        }
    }
}
