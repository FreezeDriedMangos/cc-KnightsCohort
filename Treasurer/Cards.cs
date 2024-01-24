using KnightsCohort.actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnightsCohort.Treasurer.Cards
{
    //internal sealed class ActionCostGoldResource : IKokoroApi.IActionCostApi.IResource
    //{
    //    [JsonProperty]
    //    public readonly int packagedCost;

    //    [JsonIgnore]
    //    public Status Status => (Status)MainManifest.statuses["gold"].Id;

    //    [JsonIgnore]
    //    public IKokoroApi.IActionCostApi.StatusResourceTarget Target 
    //        => IKokoroApi.IActionCostApi.StatusResourceTarget.Player;

    //    [JsonIgnore]
    //    public string ResourceKey => $"Status.Player.{Status.Key()}";

    //    [JsonIgnore]
    //    public int? IconWidth { get; }

    //    Spr? IKokoroApi.IActionCostApi.IResource.CostUnsatisfiedIcon => throw new NotImplementedException();
    //    Spr? IKokoroApi.IActionCostApi.IResource.CostSatisfiedIcon => throw new NotImplementedException();


    //    [JsonConstructor]
    //    public ActionCostGoldResource(int packagedCost)
    //    {
    //        this.packagedCost = packagedCost;
    //    }

    //    public int GetCurrentResourceAmount(State state, Combat combat)
    //    {
    //        var ship = Target == IKokoroApi.IActionCostApi.StatusResourceTarget.Player ? state.ship : combat.otherShip;
    //        return ship.Get(Status) / packagedCost;
    //    }

    //    public void PayResource(State state, Combat combat, int amount)
    //    {
    //        var ship = Target == IKokoroApi.IActionCostApi.StatusResourceTarget.Player ? state.ship : combat.otherShip;
    //        ship.Add(Status, -amount * packagedCost);
    //    }

    //    public void RenderPrefix(G g, ref Vec position, bool isDisabled, bool dontRender) {}

    //    public void Render(G g, ref Vec position, bool isSatisfied, bool isDisabled, bool dontRender)
    //    {
    //        if (dontRender)
    //        {
    //            position.x += IconWidth ?? 8;
    //            return;
    //        }

    //        int iconspace = 2;
    //        int intericonspace = 2;
    //        int runningCost = packagedCost;
    //        double posX = position.x;
    //        Spr icon;

    //        int cost10 = runningCost / 10;
    //        runningCost -= cost10 * 10;
    //        int cost5 = runningCost / 5;
    //        runningCost -= cost5 * 5;
    //        int cost1 = runningCost;

    //        icon = isSatisfied
    //            ? (Spr)MainManifest.sprites["icons/gold_1_satisfied"].Id
    //            : (Spr)MainManifest.sprites["icons/gold_1_unsatisfied"].Id;

    //        for (int i = 0; i < cost1; i++)
    //        {
    //            Draw.Sprite(icon, posX, position.y, color: isDisabled ? Colors.disabledIconTint : Colors.white);
    //            posX -= iconspace;
    //        }
    //        if (cost1 > 0) posX -= intericonspace;

    //        icon = isSatisfied
    //            ? (Spr)MainManifest.sprites["icons/gold_10_satisfied"].Id
    //            : (Spr)MainManifest.sprites["icons/gold_10_unsatisfied"].Id;

    //        for (int i = 0; i < cost5; i++)
    //        {
    //            Draw.Sprite(icon, posX, position.y, color: isDisabled ? Colors.disabledIconTint : Colors.white);
    //            posX -= iconspace;
    //        }
    //        if (cost5 > 0) posX -= intericonspace;

    //        for (int i = 0; i < cost10; i++)
    //        {
    //            Draw.Sprite(icon, posX, position.y, color: isDisabled ? Colors.disabledIconTint : Colors.white);
    //            posX -= iconspace;
    //        }
    //        if (cost10 > 0) posX -= intericonspace;

    //        icon = isSatisfied
    //            ? (Spr)MainManifest.sprites["icons/gold_5_satisfied"].Id
    //            : (Spr)MainManifest.sprites["icons/gold_5_unsatisfied"].Id;


    //        position.x += IconWidth ?? 8; // costs stretch left, so this should work
    //    }

    //    public List<Tooltip> GetTooltips(State state, Combat? combat, int amount)
    //    {
    //        string nameFormat = I18n.StatusPlayerCostActionName;
    //        string descriptionFormat = I18n.StatusPlayerCostActionDescription;

    //        //var icon = CostSatisfiedIcon ?? CostUnsatisfiedIcon ?? DB.statuses[Status].icon;
    //        var icon = DB.statuses[Status].icon;
    //        string name = string.Format(nameFormat, Status.GetLocName().ToUpper());
    //        string description = string.Format(descriptionFormat, amount*packagedCost, Status.GetLocName().ToUpper());

    //        return new()
    //        {
    //            new CustomTTGlossary(
    //                CustomTTGlossary.GlossaryType.action,
    //                () => icon,
    //                () => name,
    //                () => description,
    //                key: $"{name}\n{description}"
    //            )
    //        };
    //    }
    //}


    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Dragonfire : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 2, 4 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c)
        {
            return new() {
                new() { new AAttack() { damage = GetDmg(s, 0), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
                new() { new AAttack() { damage = GetDmg(s, 2), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
                new() { new AAttack() { damage = GetDmg(s, 3), status = Enum.Parse<Status>("heat"), statusAmount = 1 } },
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
    public class InitialInvestment : InvestmentCard
    {
        public override List<int> upgradeCosts => new() { 1, 3 };

        protected override List<List<CardAction>> GetTierActions(State s, Combat c) 
        { return new() { 
            new() { new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 1 } }, 
            new() { 
                new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 2 } ,
                new AStatus() { status = Enum.Parse<Status>("heat"), targetPlayer = true, statusAmount = 2 } 
            }, 
            new() { new AStatus() { status = (Status)MainManifest.statuses["gold"].Id, targetPlayer = true, statusAmount = 4 } }
        };}

        public override CardData GetData(State state)
        {
            CardData cardData = base.GetData(state);
            cardData.cost = 1;
            return cardData;
        }
    }
}
