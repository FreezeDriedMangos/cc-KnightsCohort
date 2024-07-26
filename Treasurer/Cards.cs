using KnightsCohort.actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace KnightsCohort.Treasurer.Cards
{


    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true, unreleased = true)]
    public class DEBUG_HonorShield : Card
    {
        public override List<CardAction> GetActions(State s, Combat c) { return new() { new AHonorShield() }; }
        public override CardData GetData(State state) { return new() { cost = 0 }; }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B }, dontOffer = true, unreleased = true)]
    public class DEBUG_GoldShield : Card
    {
        public override List<CardAction> GetActions(State s, Combat c) { return new() { new AGoldShield() }; }
        public override CardData GetData(State state) { return new() { cost = 0 }; }
    }





    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CloakedInHonor : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AStatus() { status = Status.tempShield, targetPlayer = true, statusAmount = 1 }, new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, targetPlayer = false, statusAmount = 1 } },
                new() { new AHonorShield() },
            };
        }

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }

    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class PetitionDonations : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var enemyHonorResource = MainManifest.KokoroApi.ActionCosts.StatusResource
            (
                (Status)MainManifest.statuses["honor"].Id,
                Shockah.Kokoro.IKokoroApi.IActionCostApi.StatusResourceTarget.EnemyWithOutgoingArrow,
                (Spr)MainManifest.sprites["icons/honor_cost_unsatisfied"].Id,
                (Spr)MainManifest.sprites["icons/honor_cost"].Id
            );

            return new()
            {
                MainManifest.KokoroApi.ActionCosts.Make
                (
                    MainManifest.KokoroApi.ActionCosts.Cost(enemyHonorResource , amount: 1),
                    new AGoldShield() { amount = 2 }
                )
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1 };
        }
    }

    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SuitableWeapons : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
                new ACardSelect()
                {
                    browseSource = Enum.Parse<CardBrowse.Source>("Hand"),
                    browseAction = new DrawCardsOfSelectedCardColor() { count = upgrade == Upgrade.B ? 5 : 3 }
                }
            };
        }
        public override CardData GetData(State state)
        {
            return new() { cost = 1, exhaust = upgrade == Upgrade.A ? false : true, description = "Select a card in hand, draw 3 cards of that color." };
        }
    }
}
