using Microsoft.Extensions.Logging.Abstractions;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Knight
{
    internal class VowsRenderer : IStatusRenderHook
    {

        internal VowsRenderer() : base()
        {
            MainManifest.KokoroApi.RegisterStatusRenderHook(this, double.MinValue);
        }

        //public IEnumerable<(Status Status, double Priority)> GetExtraStatusesToShow(State state, Combat combat, Ship ship)
        //{
        //    if (MainManifest.Instance.Api.GetSmug(state, ship) is not null)
        //        yield return (Status: (Status)MainManifest.Instance.SmugStatus.Id!.Value, Priority: 10);
        //}

        public bool? ShouldOverrideStatusRenderingAsBars(State state, Combat combat, Ship ship, Status status, int amount)
            => VowsController.StatusIsVow(status) ? true : null;

        public (IReadOnlyList<Color> Colors, int? BarTickWidth) OverrideStatusRendering(State state, Combat combat, Ship ship, Status status, int amount)
        {
            return (new Color[] 
            { 
                amount >= 1 ? Colors.cheevoGold : Colors.disabledIconTint,
                amount >= 2 ? Colors.cheevoGold : new Color("57411f")
            }, null);

            //if (status == (Status)MainManifest.Instance.DoubleTimeStatus.Id!.Value)
            //    return (new Color[] { new(0, 0, 0, 0) } );

            //var barCount = MainManifest.Instance.Api.GetMaxSmug(ship) - MainManifest.Instance.Api.GetMinSmug(ship) + 1;
            //var colors = new Color[barCount];

            //if (ship.Get((Status)MainManifest.Instance.DoubleTimeStatus.Id!.Value) > 0)
            //{
            //    Array.Fill(colors, Colors.cheevoGold);
            //    return (colors, null);
            //}

            //var goodColor = Colors.cheevoGold;
            //if (MainManifest.Instance.Api.IsOversmug(state, ship))
            //{
            //    double f = Math.Sin(MainManifest.Instance.KokoroApi.TotalGameTime.TotalSeconds * Math.PI * 2) * 0.5 + 0.5;
            //    goodColor = Color.Lerp(Colors.downside, Colors.white, f);
            //}

            //for (int barIndex = 0; barIndex < colors.Length; barIndex++)
            //{
            //    var smugIndex = barIndex + MainManifest.Instance.Api.GetMinSmug(ship);
            //    if (smugIndex == 0)
            //    {
            //        colors[barIndex] = Colors.white;
            //        continue;
            //    }

            //    var smug = MainManifest.Instance.Api.GetSmug(state, ship) ?? 0;
            //    if (smug < 0 && smugIndex >= smug && smugIndex < 0)
            //        colors[barIndex] = Colors.downside;
            //    else if (smug > 0 && smugIndex <= smug && smugIndex > 0)
            //        colors[barIndex] = goodColor;
            //    else
            //        colors[barIndex] = MainManifest.Instance.KokoroApi.DefaultInactiveStatusBarColor;
            //}
            //return (colors, null);
        }

        //public List<Tooltip> OverrideStatusTooltips(Status status, int amount, bool isForShipStatus, List<Tooltip> tooltips)
        //{
        //    if (status == (Status)MainManifest.Instance.SmugStatus.Id!.Value)
        //    {
        //        if (isForShipStatus)
        //        {
        //            var glossary = tooltips.FirstOrDefault() as TTGlossary;
        //            if (glossary is not null)
        //                tooltips[0] = new CustomTTGlossary(
        //                    CustomTTGlossary.GlossaryType.status,
        //                    () => I18n.SmugStatusName,
        //                    () => I18n.SmugStatusLongDescription
        //                );

        //            if (StateExt.MainManifest.Instance is { } state)
        //            {
        //                double botchChance = Math.Clamp(MainManifest.Instance.Api.GetSmugBotchChance(state, state.ship, null), 0, 1);
        //                double doubleChance = Math.Clamp(MainManifest.Instance.Api.GetSmugDoubleChance(state, state.ship, null), 0, 1 - botchChance);
        //                tooltips.Add(new TTText(string.Format(I18n.SmugStatusOddsDescription, doubleChance * 100, botchChance * 100)));
        //            }
        //        }
        //    }
        //    else if (status == (Status)MainManifest.Instance.BidingTimeStatus.Id!.Value)
        //    {
        //        tooltips.Add(new TTGlossary($"status.{MainManifest.Instance.DoubleTimeStatus.Id!.Value}", amount));
        //    }
        //    else if (status == (Status)MainManifest.Instance.DoublersLuckStatus.Id!.Value)
        //    {
        //        tooltips.Clear();
        //        tooltips.Add(new TTGlossary($"status.{MainManifest.Instance.DoublersLuckStatus.Id!.Value}", amount + 1));
        //    }
        //    return tooltips;
        //}
    }
}
