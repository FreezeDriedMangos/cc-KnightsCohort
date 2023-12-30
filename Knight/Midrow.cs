using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Knight.Midrow
{
    public static class MissileTypeExtender
    {
        private static Dictionary<string, MissileType> customTypes = new();
        private static int MaxMissileTypeValue = 0;
        public static MissileType Get(string customTypeName)
        {
            if (customTypes.ContainsKey(customTypeName)) return customTypes[customTypeName];
            if (MaxMissileTypeValue == 0) MaxMissileTypeValue = Enum.GetValues(typeof(MissileType)).Cast<int>().Max();

            customTypes[customTypeName] = (MissileType)(++MaxMissileTypeValue);
            return customTypes[customTypeName];
        }
    }

    public class Dagger : Missile
    {
        // add entry to Missile.missileData
        // add entry to DB.drones
        // set this.skin
        static Dagger()
        {
            DB.drones["missile_dagger"] = (Spr)MainManifest.sprites["midrow/dagger"].Id;
        }
        public Dagger()
        {
            base.skin = "missile_dagger";
            base.missileType = MissileTypeExtender.Get("dagger");
        }

        // to circumvent missileData
        public override Spr? GetIcon()
        {
            return (Spr)MainManifest.sprites["icons/missile_dagger"].Id;
        }

        public override string GetDialogueTag()
        {
            return "missile_dagger";
        }

        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = new List<Tooltip>()
            {
                new TTGlossary(MainManifest.glossary["missiledagger"].Head, 1)
                {
                    flipIconY = base.targetPlayer
                }
            };

            if (base.bubbleShield)
            {
                tooltips.Add(new TTGlossary("midrow.bubbleShield"));
            }
            return tooltips;
        }

        public override List<CardAction>? GetActions(State s, Combat c)
        {
            return new List<CardAction>()
            {
                new AMissileHit
                {
                    worldX = x,
                    outgoingDamage = 1,
                    targetPlayer = targetPlayer
                }
            };
        }
    }
}
