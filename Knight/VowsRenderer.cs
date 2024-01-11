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

        public bool? ShouldOverrideStatusRenderingAsBars(State state, Combat combat, Ship ship, Status status, int amount)
            => VowsController.StatusIsVow(status) ? true : null;

        public (IReadOnlyList<Color> Colors, int? BarTickWidth) OverrideStatusRendering(State state, Combat combat, Ship ship, Status status, int amount)
        {
            return (new Color[] 
            { 
                amount >= 1 ? Colors.cheevoGold : Colors.disabledIconTint,
                amount >= 2 ? Colors.cheevoGold : new Color("57411f")
            }, null);
        }
    }
}
