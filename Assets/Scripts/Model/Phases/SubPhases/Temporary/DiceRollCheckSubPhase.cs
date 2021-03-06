﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubPhases
{

    public class DiceRollCheckSubPhase : GenericSubPhase
    {
        protected DiceKind dicesType;
        protected int dicesCount;

        protected DiceRoll CurrentDiceRoll;
        protected DelegateDiceroll checkResults;

        protected UnityEngine.Events.UnityAction finishAction;


        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            IsTemporary = true;
            finishAction = FinishAction;
            checkResults = CheckResults;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Game.PrefabsList.CheckDiceResultsMenu.SetActive(true);

            DiceRoll DiceRollCheck;
            DiceRollCheck = new DiceRoll(dicesType, dicesCount, DiceRollCheckType.Check);
            DiceRollCheck.Roll(checkResults);
        }

        public void ShowConfirmDiceResultsButton()
        {
            // BUG after koiogran asteroid?
            if (Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).GetType() == typeof(Players.HumanPlayer))
            {
                Button closeButton = Game.PrefabsList.CheckDiceResultsMenu.transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(finishAction);

                closeButton.gameObject.SetActive(true);
            }
            else
            {
                finishAction.Invoke();
            }
        }

        protected virtual void CheckResults(DiceRoll diceRoll)
        {
            CurrentDiceRoll = diceRoll;
            ShowConfirmDiceResultsButton();
        }

        protected virtual void FinishAction()
        {
            HideDiceResultMenu();
            Phases.FinishSubPhase(this.GetType());
        }

        public void HideDiceResultMenu()
        {
            Game.PrefabsList.CheckDiceResultsMenu.SetActive(false);
            HideDiceModificationButtons();
            CurrentDiceRoll.RemoveDiceModels();
        }

        public void HideDiceModificationButtons()
        {
            foreach (Transform button in Game.PrefabsList.CheckDiceResultsMenu.transform.Find("DiceModificationsPanel"))
            {
                if (button.name.StartsWith("Button"))
                {
                    MonoBehaviour.Destroy(button.gameObject);
                }
            }
            Game.PrefabsList.CheckDiceResultsMenu.transform.Find("DiceModificationsPanel/Confirm").gameObject.SetActive(false);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            return result;
        }

    }

}
