﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class IonCannonTurret : GenericSecondaryWeapon
    {
        public IonCannonTurret() : base()
        {
            Type = UpgradeType.Turret;

            Name = "Ion Cannon Turret";
            ShortName = "Ion Cannon Turret";
            ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/f/f3/Ion_Cannon_Turret.png";
            Cost = 5;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;

            CanShootOutsideArc = true;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            SubscribeOnHit();
        }

        private void SubscribeOnHit()
        {
            Host.OnAttackHitAsAttacker += RegisterIonTurretEffect;
        }

        private void RegisterIonTurretEffect()
        {
            if (Combat.ChosenWeapon == this)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Ion Cannon Turret effect",
                    TriggerType = TriggerTypes.OnAttackHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = IonTurretEffect
                });
            }
        }

        private void IonTurretEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.AssignToken(
                new Tokens.IonToken(),
                delegate {
                    Game.Wait(2, DefenderSuffersDamage);
                }
            );
        }

        private void DefenderSuffersDamage()
        {
            Combat.Defender.AssignedDamageDiceroll.AddDice(DiceSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Combat.Defender.Owner.PlayerNo,
                EventHandler = Combat.Defender.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = Combat.Attacker,
                    DamageType = DamageTypes.ShipAttack
                },
                Skippable = true
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
        }

    }

}