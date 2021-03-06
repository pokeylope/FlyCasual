﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class RoundStartSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Round start";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            InformAboutNewRoundStart();

            Phases.CallRoundStartTrigger();
            Phases.FinishSubPhase(this.GetType());
        }

        private void InformAboutNewRoundStart()
        {
            Phases.RoundCounter++;
            Game.UI.AddTestLogEntry("Round " + Phases.RoundCounter + " is started");
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = new PlanningStartSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip)
        {
            return false;
        }

    }

}
