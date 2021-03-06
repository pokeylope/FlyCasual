﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public delegate void DiceModification();

public static partial class Combat
{

    public static void ShowDiceResultMenu(UnityEngine.Events.UnityAction closeAction)
    {
        Button closeButton = Game.PrefabsList.CombatDiceResultsMenu.transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
        Game.PrefabsList.CombatDiceResultsMenu.SetActive(true);
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(closeAction);
    }

    public static void ShowDiceModificationButtons()
    {
        Selection.ActiveShip.GenerateAvailableActionEffectsList();

        if (Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).Type == Players.PlayerType.Human)
        {
            float offset = 0;
            Vector3 defaultPosition = Game.PrefabsList.CombatDiceResultsMenu.transform.Find("DiceModificationsPanel").position;

            foreach (var actionEffect in Selection.ActiveShip.GetAvailableActionEffectsList())
            {
                Vector3 position = defaultPosition + new Vector3(0, -offset, 0);
                CreateDiceModificationsButton(actionEffect, position);
                offset += 40;
            }
        }

        Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).UseDiceModifications();
    }

    public static void ToggleConfirmDiceResultsButton(bool isActive)
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCombatSubPhase).ToggleConfirmDiceResultsButton(isActive);
        }
    }

    private static void CreateDiceModificationsButton(ActionsList.GenericAction actionEffect, Vector3 position)
    {
        GameObject newButton = MonoBehaviour.Instantiate(Game.PrefabsList.GenericButton, Game.PrefabsList.CombatDiceResultsMenu.transform.Find("DiceModificationsPanel"));
        newButton.name = "Button" + actionEffect.EffectName;
        newButton.transform.GetComponentInChildren<Text>().text = actionEffect.EffectName;
        newButton.GetComponent<RectTransform>().position = position;
        newButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            Tooltips.EndTooltip();
            newButton.GetComponent<Button>().interactable = false;
            Selection.ActiveShip.AddAlreadyExecutedActionEffect(actionEffect);
            actionEffect.ActionEffect(delegate { });
        });
        Tooltips.AddTooltip(newButton, actionEffect.ImageUrl);
        newButton.GetComponent<Button>().interactable = true;
        newButton.SetActive(true);
    }


    //REMOVE
    public static void HideDiceModificationButtons()
    {
        foreach (Transform button in Game.PrefabsList.CombatDiceResultsMenu.transform.Find("DiceModificationsPanel"))
        {
            if (button.name.StartsWith("Button"))
            {
                MonoBehaviour.Destroy(button.gameObject);
            }
        }
        Game.PrefabsList.CombatDiceResultsMenu.transform.Find("DiceModificationsPanel/Confirm").gameObject.SetActive(false);
    }


    // REMOVE
    public static void HideDiceResultMenu()
    {
        Game.PrefabsList.CombatDiceResultsMenu.SetActive(false);
        HideDiceModificationButtons();
        CurentDiceRoll.RemoveDiceModels();
    }

}
