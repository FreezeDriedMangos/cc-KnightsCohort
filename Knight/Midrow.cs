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
            Missile.missileData.Add(
                MissileTypeExtender.Get("dagger"),
                new Missile.MissileMetadata()
                {
                    key = "missile_dagger",
                    exhaustColor = new Color("cc33ff"),
                    icon = (Spr)MainManifest.sprites["icons/missile_dagger"].Id,
                    baseDamage = 1
                }
            );

            DB.drones["missile_dagger"] = (Spr)MainManifest.sprites["midrow/dagger"].Id;
        }
        public Dagger()
        {
            base.skin = "missile_dagger";
            base.missileType = MissileTypeExtender.Get("dagger");
        }


        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = new List<Tooltip>()
            {
                new TTGlossary(MainManifest.glossary["missile_dagger"].Head, Missile.missileData[this.missileType].baseDamage)
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
    }
}
