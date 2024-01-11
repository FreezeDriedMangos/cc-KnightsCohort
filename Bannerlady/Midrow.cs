using KnightsCohort.actions;
using KnightsCohort.Knight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsCohort.Bannerlady.Midrow
{
    public class WarBanner : Banner
    {
        public override Spr GetSprite() { return (Spr)MainManifest.sprites["midrow/banner_of_war"].Id; }
        public override Spr? GetIcon() { return (Spr)MainManifest.sprites["icons/banner_war"].Id; }

        public override void ModifyAction(AAttack aattack, State s, Combat c, bool wasPlayer)
        {
            if (wasPlayer)
            {
                c.QueueImmediate(new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = true });
            }
        }
    }

    public class TatteredWarBanner : WarBanner
    {
        public override bool Tattered() { return true; }
        public override Spr GetSprite() { return (Spr)MainManifest.sprites["midrow/tattered_banner_of_war"].Id; }
        public override Spr? GetIcon() { return (Spr)MainManifest.sprites["icons/tattered_banner_war"].Id; }
    }

    public class MercyBanner : Banner
    {
        public override Spr GetSprite() { return (Spr)MainManifest.sprites["midrow/banner_of_mercy"].Id; }
        public override Spr? GetIcon() { return (Spr)MainManifest.sprites["icons/banner_mercy"].Id; }

        public override void ModifyAction(AAttack aattack, State s, Combat c, bool wasPlayer)
        {
            if (wasPlayer)
            {
                c.QueueImmediate(new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = -1, targetPlayer = true });
            }
        }
    }

    public class MartyrBanner : Banner
    {
        public override Spr GetSprite() { return (Spr)MainManifest.sprites["midrow/banner_of_martyr"].Id; }
        public override Spr? GetIcon() { return (Spr)MainManifest.sprites["icons/banner_martyr"].Id; }

        public override void ModifyAction(AAttack aattack, State s, Combat c, bool wasPlayer)
        {
            if (!wasPlayer)
            {
                Part? p = VowsController.AAttackPostfix_GetHitShipPart(aattack, s, c);
                if (p == null) return;
                if (p.type == Enum.Parse<PType>("empty")) return;
                if (s.ship.Get(Enum.Parse<Status>("autododgeLeft")) > 0 || s.ship.Get(Enum.Parse<Status>("autododgeRight")) > 0) return;

                c.QueueImmediate(new AStatus() { status = (Status)MainManifest.statuses["honor"].Id, statusAmount = 1, targetPlayer = true });
            }
        }
    }

    public class BannerOfInspiration : Banner
    {
        public override Spr GetSprite() { return (Spr)MainManifest.sprites["midrow/banner_of_inspiration"].Id; }
        public override Spr? GetIcon() { return (Spr)MainManifest.sprites["icons/banner_inspiration"].Id; }

        public override void ModifyAction(AAttack aattack, State s, Combat c, bool wasPlayer)
        {
            aattack.status = Enum.Parse<Status>("overdrive");
            aattack.statusAmount = 1;
        }
    }

    public class BannerOfShielding : Banner
    {
        public override Spr GetSprite() { return (Spr)MainManifest.sprites["midrow/banner_of_shielding"].Id; }
        public override Spr? GetIcon() { return (Spr)MainManifest.sprites["icons/banner_shielding"].Id; }

        public override void ModifyAction(AAttack aattack, State s, Combat c, bool wasPlayer)
        {
            c.QueueImmediate(new AStatus()
            {
                status = Enum.Parse<Status>("tempShield"),
                targetPlayer = wasPlayer,
                statusAmount = 1,
            });
        }
    }

    public class PirateBanner : Banner
    {
        public override Spr GetSprite() { return (Spr)MainManifest.sprites["midrow/banner_of_pirate"].Id; }
        public override Spr? GetIcon() { return (Spr)MainManifest.sprites["icons/banner_pirate"].Id; }

        public override List<CardAction>? GetActionsOnDestroyed(State s, Combat c, bool wasPlayer, int worldX)
        {
            return new()
            {
                new AStatus()
                {
                    targetPlayer = true,
                    status = (Status)MainManifest.statuses["honor"].Id,
                    statusAmount = 1,
                }
            };
        }
    }

    public class TatteredMartyrBanner : MartyrBanner
    {
        public override bool Tattered() { return true; }
        public override Spr GetSprite() { return (Spr)MainManifest.sprites["midrow/tattered_banner_of_martyr"].Id; }
        public override Spr? GetIcon() { return (Spr)MainManifest.sprites["icons/tattered_banner_martyr"].Id; }
    }



    public class PiercingMissile : Missile
    {
        public virtual string MIDROW_OBJECT_NAME { get => "defaultPiercingMissile"; }
        public virtual int BASE_DAMAGE { get => 1; }
        public virtual string MIDROW_SPRITE { get => "midrow/arrow"; }
        public virtual string ICON_SPRITE { get => "icons/missile_arrow"; }

        public PiercingMissile()
        {
            DB.drones[MIDROW_OBJECT_NAME] = (Spr)MainManifest.sprites[MIDROW_SPRITE].Id;
            base.skin = MIDROW_OBJECT_NAME;
        }

        // to circumvent missileData
        public override Spr? GetIcon()
        {
            return (Spr)MainManifest.sprites[ICON_SPRITE].Id;
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

    public class ArrowMissile : PiercingMissile
    {
        public override string MIDROW_OBJECT_NAME { get => "missileArrow"; }
        public override int BASE_DAMAGE { get => 1; }
        public override string MIDROW_SPRITE { get => "midrow/arrow"; }
        public override string ICON_SPRITE { get => "icons/arrow"; }
    }
    public class BroadheadArrowMissile : PiercingMissile
    {
        public override string MIDROW_OBJECT_NAME { get => "missileBroadheadArrow"; }
        public override int BASE_DAMAGE { get => 2; }
        public override string MIDROW_SPRITE { get => "midrow/broadhead_arrow"; }
        public override string ICON_SPRITE { get => "icons/broadhead_arrow"; }
    }
}
