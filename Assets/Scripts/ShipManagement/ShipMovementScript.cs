﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipMovementScript : MonoBehaviour {

    private GameManagerScript Game;

    //TODO: Refactor old
    public Collider CollidedWith;
    public Collider ObstacleEnter;
    public Collider ObstacleExit;
    public Collider ObstacleHitEnter;
    public Collider ObstacleHitExit;

    public List<System.Func<bool>> FuncsToUpdate = new List<System.Func<bool>>();

    public bool isMoving;

    public void Initialize()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    void Update ()
    {
        Selection.UpdateSelection();
        UpdateMovement();
        UpdateSubscribedFuncs();

        ClearCollision();
    }

    private void UpdateMovement()
    {
        if (isMoving)
        {
            Selection.ThisShip.AssignedManeuver.UpdateMovementExecution();
        }
    }

    private void UpdateSubscribedFuncs()
    {
        List<System.Func<bool>> subscribedFuncs = new List<System.Func<bool>>();
        subscribedFuncs.AddRange(FuncsToUpdate);

        foreach (var func in subscribedFuncs)
        {
            bool isFinished = func();
            if (isFinished) FuncsToUpdate.Remove(func);
        }        
    }

    private void ClearCollision()
    {
        CollidedWith = null;
    }

    //Assignment and launch of execution of meneuver

    public void AssignManeuver()
    {
        string parameters = EventSystem.current.currentSelectedGameObject.name;

        Selection.ThisShip.AssignedManeuver = MovementFromString(parameters);

        Selection.ThisShip.InfoPanel.transform.Find("DialAssigned" + Selection.ThisShip.Owner.Id).gameObject.SetActive(true);
        Roster.HighlightShipOff(Selection.ThisShip);

        Game.UI.HideDirectionMenu();

        if (Roster.AllManuersAreAssigned(Phases.CurrentPhasePlayer))
        {
            Game.UI.ShowNextButton();
            Game.UI.HighlightNextButton();
        }
    }

    public Movement.GenericMovement MovementFromStruct(Movement.MovementStruct movementStruct)
    {
        Movement.GenericMovement result = null;

        if (movementStruct.Bearing == Movement.ManeuverBearing.Straight)
        {
            result = new Movement.StraightMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.KoiogranTurn)
        {
            result = new Movement.KoiogranTurnMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Turn)
        {
            result = new Movement.TurnMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Bank)
        {
            result = new Movement.BankMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }

        return result;
    }

    public Movement.GenericMovement MovementFromString(string parameters)
    {
        Movement.MovementStruct movementStruct = ManeuverFromString(parameters);

        return MovementFromStruct(movementStruct);
    }

    public Movement.MovementStruct ManeuverFromString(string parameters)
    {
        string[] arrParameters = parameters.Split('.');

        Movement.ManeuverSpeed speed = Movement.ManeuverSpeed.Speed1;

        switch (arrParameters[0])
        {
            case "1":
                speed = Movement.ManeuverSpeed.Speed1;
                break;
            case "2":
                speed = Movement.ManeuverSpeed.Speed2;
                break;
            case "3":
                speed = Movement.ManeuverSpeed.Speed3;
                break;
            case "4":
                speed = Movement.ManeuverSpeed.Speed4;
                break;
            case "5":
                speed = Movement.ManeuverSpeed.Speed5;
                break;
        }

        Movement.ManeuverDirection direction = Movement.ManeuverDirection.Forward;

        switch (arrParameters[1])
        {
            case "F":
                direction = Movement.ManeuverDirection.Forward;
                break;
            case "L":
                direction = Movement.ManeuverDirection.Left;
                break;
            case "R":
                direction = Movement.ManeuverDirection.Right;
                break;
        }

        Movement.ManeuverBearing bearing = Movement.ManeuverBearing.Straight;

        switch (arrParameters[2])
        {
            case "S":
                bearing = Movement.ManeuverBearing.Straight;
                break;
            case "R":
                bearing = Movement.ManeuverBearing.KoiogranTurn;
                break;
            case "B":
                bearing = Movement.ManeuverBearing.Bank;
                break;
            case "T":
                bearing = Movement.ManeuverBearing.Turn;
                break;
        }

        Movement.MovementStruct result = new Movement.MovementStruct()
        {
            Speed = speed,
            Direction = direction,
            Bearing = bearing,
            ColorComplexity = Selection.ThisShip.Maneuvers[parameters]
        };

        Movement.ManeuverColor color = Selection.ThisShip.GetColorComplexityOfManeuver(result);
        result.ColorComplexity = color;

        return result;
    }

    public void PerformStoredManeuver()
    {
        Phases.StartTemporarySubPhase("Movement", typeof(SubPhases.MovementExecutionSubPhase));
    }

}
