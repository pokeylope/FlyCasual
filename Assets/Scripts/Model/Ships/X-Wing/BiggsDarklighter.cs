﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class BiggsDarklighter : XWing
        {
            public BiggsDarklighter() : base()
            {
                PilotName = "Biggs Darklighter";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/9/90/Biggs-darklighter.png";
                IsUnique = true;
                PilotSkill = 5;
                Cost = 25;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal += CanPerformAttack;
                OnDestroyed += RemoveBiggsDarklighterAbility;
            }

            public void CanPerformAttack(ref bool result, GenericShip attacker, GenericShip defender)
            {
                bool abilityIsActive = false;
                if (defender.ShipId != this.ShipId)
                {
                    if (defender.Owner.PlayerNo == this.Owner.PlayerNo)
                    {
                        Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(defender, this);
                        if (positionInfo.Range <= 1)
                        {
                            if (!attacker.ShipsBumped.Contains(this))
                            {
                                if (Combat.ChosenWeapon.IsShotAvailable(this)) abilityIsActive = true;
                            }
                        }
                    }
                }

                if (abilityIsActive)
                {
                    if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                    {
                        Messages.ShowErrorToHuman("Biggs DarkLighter: You cannot attack target ship");
                    }
                    result = false;
                }
            }

            private void RemoveBiggsDarklighterAbility(GenericShip ship)
            {
                RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal -= CanPerformAttack;
                OnDestroyed -= RemoveBiggsDarklighterAbility;
            }
        }
    }
}
