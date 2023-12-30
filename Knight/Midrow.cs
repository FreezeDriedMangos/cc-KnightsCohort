using KnightsCohort.actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Knight.Midrow
{
    public class Dagger : Missile
    {
        public static readonly string MIDROW_OBJECT_NAME = "missileDagger";
        public static readonly int BASE_DAMAGE = 1;

        // add entry to Missile.missileData
        // add entry to DB.drones
        // set this.skin
        static Dagger()
        {
            DB.drones[MIDROW_OBJECT_NAME] = (Spr)MainManifest.sprites["midrow/dagger"].Id;
        }
        public Dagger()
        {
            base.skin = MIDROW_OBJECT_NAME;
        }

        // to circumvent missileData
        public override Spr? GetIcon()
        {
            return (Spr)MainManifest.sprites["icons/missile_dagger"].Id;
        }

        public override string GetDialogueTag()
        {
            return MIDROW_OBJECT_NAME;
        }

        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = new List<Tooltip>()
            {
                new TTGlossary(MainManifest.glossary[MIDROW_OBJECT_NAME].Head, BASE_DAMAGE)
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
                    outgoingDamage = BASE_DAMAGE,
                    targetPlayer = targetPlayer
                }
            };
        }
    }

    public class Sword : Missile
    {
        public static readonly string MIDROW_OBJECT_NAME = "missileSword";
        public static readonly int BASE_DAMAGE = 3;

        // add entry to Missile.missileData
        // add entry to DB.drones
        // set this.skin
        static Sword()
        {
            DB.drones[MIDROW_OBJECT_NAME] = Enum.Parse<Spr>("drones_sword");
        }
        public Sword()
        {
            base.skin = MIDROW_OBJECT_NAME;
        }

        // to circumvent missileData
        public override Spr? GetIcon()
        {
            return Enum.Parse<Spr>("icons_missile_heavy_sword");
        }

        public override string GetDialogueTag()
        {
            return MIDROW_OBJECT_NAME;
        }

        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = new List<Tooltip>()
            {
                new TTGlossary(MainManifest.glossary[MIDROW_OBJECT_NAME].Head, BASE_DAMAGE)
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
                    outgoingDamage = BASE_DAMAGE,
                    targetPlayer = targetPlayer
                }
            };
        }
    }
    public class ExcaliburMissile : Missile
    {
        public static readonly string MIDROW_OBJECT_NAME = "missileExcalibur";
        public static readonly int BASE_DAMAGE = 3;

        // add entry to Missile.missileData
        // add entry to DB.drones
        // set this.skin
        static ExcaliburMissile()
        {
            DB.drones[MIDROW_OBJECT_NAME] = (Spr)MainManifest.sprites["midrow/excalibur"].Id;
        }
        public ExcaliburMissile()
        {
            base.skin = MIDROW_OBJECT_NAME;
        }

        // to circumvent missileData
        public override Spr? GetIcon()
        {
            return (Spr)MainManifest.sprites["icons/missile_excalibur"].Id;
        }

        public override string GetDialogueTag()
        {
            return MIDROW_OBJECT_NAME;
        }

        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = new List<Tooltip>()
            {
                new TTGlossary(MainManifest.glossary[MIDROW_OBJECT_NAME].Head, BASE_DAMAGE)
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
                new APiercingMissileHit
                {
                    worldX = x,
                    outgoingDamage = BASE_DAMAGE,
                    targetPlayer = targetPlayer
                }
            };
        }
    }

}
