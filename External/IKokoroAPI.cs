using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CobaltCoreModding.Definitions.ExternalItems;

namespace KnightsCohort.External
{
    public partial interface IKokoroApi
    {
        ExternalStatus OxidationStatus { get; }
        Tooltip GetOxidationStatusTooltip(State state, Ship ship);
        int GetOxidationStatusMaxValue(State state, Ship ship);

        void RegisterOxidationStatusHook(IOxidationStatusHook hook, double priority);
        void UnregisterOxidationStatusHook(IOxidationStatusHook hook);
    }

    public interface IOxidationStatusHook
    {
        int ModifyOxidationRequirement(State state, Ship ship, int value);
    }
}
