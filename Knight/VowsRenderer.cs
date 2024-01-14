using KnightsCohort.Knight.Artifacts;
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
            int max = state.EnumerateAllArtifacts().Where(a => a is HolyGrail).Any() ? 3 : 2;

            var colors = new Color[max];
            for (int i = 1; i <= max; i++)
            {
                colors[i-1] = amount >= i ? Colors.cheevoGold : new Color("57411f");
            }

            return (colors, null);
        }
    }
}
