﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Players;
using System.Linq;

public static partial class Roster {

    private static List<GameObject> rosterPlayer1 = new List<GameObject>();
    private static List<GameObject> rosterPlayer2 = new List<GameObject>();
    
    public static GameObject CreateRosterInfo(Ship.GenericShip newShip)
    {
        GameObject newPanel = MonoBehaviour.Instantiate(Game.PrefabsList.RosterPanel, Game.PrefabsList.RostersHolder.transform.Find("TeamPlayer" + newShip.Owner.Id).Find("RosterHolder").transform);

        //Generic info
        newPanel.transform.Find("ShipInfo/ShipPilotSkillText").GetComponent<Text>().text = newShip.PilotSkill.ToString();

        newPanel.transform.Find("ShipInfo/ShipId").GetComponent<Text>().text = newShip.ShipId.ToString();
        newPanel.transform.Find("ShipIdText/Text").GetComponent<Text>().text = newShip.ShipId.ToString();
        newPanel.transform.Find("ShipIdText/Text").GetComponent<Text>().color = (newShip.Owner.PlayerNo == PlayerNo.Player1) ? Color.green : Color.red;
        newPanel.transform.Find("ShipIdText").localPosition = new Vector3((newShip.Owner.PlayerNo == PlayerNo.Player1) ? 205 : -50, 0, 0);
        newPanel.transform.Find("ShipIdText").gameObject.SetActive(true);

        newPanel.transform.Find("ShipInfo/ShipFirepowerText").GetComponent<Text>().text = newShip.Firepower.ToString();
        newPanel.transform.Find("ShipInfo/ShipAgilityText").GetComponent<Text>().text = newShip.Agility.ToString();
        newPanel.transform.Find("ShipInfo/ShipHullText").GetComponent<Text>().text = newShip.MaxHull.ToString();
        newPanel.transform.Find("ShipInfo/ShipShieldsText").GetComponent<Text>().text = newShip.MaxShields.ToString();

        //Tooltips
        GameObject pilotNameGO = newPanel.transform.Find("ShipInfo/ShipPilotNameText").gameObject;
        pilotNameGO.GetComponent<Text>().text = newShip.PilotName;
        Tooltips.AddTooltip(pilotNameGO, newShip.ImageUrl);
        SubscribeActions(pilotNameGO);

        GameObject shipTypeGO = newPanel.transform.Find("ShipInfo/ShipTypeText").gameObject;
        shipTypeGO.GetComponent<Text>().text = newShip.Type;
        Tooltips.AddTooltip(shipTypeGO, newShip.ManeuversImageUrl);
        SubscribeActions(shipTypeGO);

        //Mark
        newPanel.transform.Find("Mark").localPosition = new Vector3((newShip.Owner.PlayerNo == PlayerNo.Player1) ? 198 : -8, 0, 0);
        SubscribeMarkByHover(newPanel);

        //Hull and shields
        float panelWidth = 200 - 10;
        float hullAndShield = newShip.MaxHull + newShip.MaxShields;
        float panelWidthNoDividers = panelWidth - (1 * (hullAndShield - 1));
        float damageIndicatorWidth = panelWidthNoDividers / hullAndShield;

        GameObject damageIndicatorBar = newPanel.transform.Find("ShipInfo/DamageBarPanel").gameObject;
        GameObject damageIndicator = damageIndicatorBar.transform.Find("DamageIndicator").gameObject;
        damageIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(damageIndicatorWidth, 10);
        for (int i = 0; i < hullAndShield; i++)
        {
            GameObject newDamageIndicator = MonoBehaviour.Instantiate(damageIndicator, damageIndicatorBar.transform);
            newDamageIndicator.transform.position = damageIndicator.transform.position + new Vector3(i * (damageIndicatorWidth + 1), 0, 0);
            if (i < newShip.Hull) {
                newDamageIndicator.GetComponent<Image>().color = Color.yellow;
                newDamageIndicator.name = "DamageIndicator.Hull." + (i+1).ToString();
            } else
            {
                newDamageIndicator.GetComponent<Image>().color = new Color(0, 202, 255);
                newDamageIndicator.name = "DamageIndicator.Shield." + (i-newShip.Hull+1).ToString();
            }
            newDamageIndicator.SetActive(true);
        }
        MonoBehaviour.Destroy(damageIndicator);

        //Finish
        newPanel.transform.Find("ShipInfo").tag = "ShipId:" + newShip.ShipId.ToString();
        AddToRoster(newShip, newPanel);
        newPanel.transform.Find("ShipInfo").gameObject.SetActive(true);

        return newPanel;
    }

    public static void SubscribeActions(GameObject panel)
    {
        panel.AddComponent<EventTrigger>();
        EventTrigger trigger = panel.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { SelectShipByRosterClick((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    private static void AddToRoster(Ship.GenericShip newShip, GameObject newPanel)
    {

        List<GameObject> rosterPlayer = (newShip.Owner.PlayerNo == PlayerNo.Player1) ? rosterPlayer1 : rosterPlayer2;
        rosterPlayer.Add(newPanel);

        OrganizeRosters();

    }

    //ORGANIZE

    public static void OrganizeRosters()
    {
        OrganizeRosterPanelSizes();
        OrganizeRosterPositions();
    }

    private static void OrganizeRosterPanelSizes()
    {
        foreach (GameObject panel in rosterPlayer1)
        {
            panel.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta = new Vector2(200, CalculateRosterPanelSize(panel));
        }

        //same
        foreach (GameObject panel in rosterPlayer2)
        {
            panel.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta = new Vector2(200, CalculateRosterPanelSize(panel));
        }

    }

    private static int CalculateRosterPanelSize(GameObject panel)
    {
        int panelHeight = 80;

        //Upgrades

        float upgradesVisible = 0;

        foreach (Transform icon in panel.transform.Find("ShipInfo/UpgradesBar").transform)
        {
            if (icon.gameObject.activeSelf)
            {
                upgradesVisible++;
            }
        }

        int upgdaresHeight = Mathf.CeilToInt(upgradesVisible / 2) * 20;
        panelHeight += upgdaresHeight;

        //Tokens

        panel.transform.Find("ShipInfo/TokensBar").GetComponent<RectTransform>().localPosition = new Vector2(10, -65 - upgdaresHeight);

        int iconsCount = 0;
        foreach (Transform icon in panel.transform.Find("ShipInfo/TokensBar").transform)
        {
            if (icon.gameObject.activeSelf)
            {
                iconsCount++;
            }
        }

        int iconsLines = (iconsCount + 4) / 5;
        panelHeight += 35 * iconsLines + 3;

        panel.transform.Find("Mark").GetComponent<RectTransform>().sizeDelta = new Vector2(10, panelHeight);

        return panelHeight;
    }

    private static void OrganizeRosterPositions()
    {
        for (int i = 1; i < 3; i++)
        {
            Vector3 defaultPosition = Game.PrefabsList.RostersHolder.transform.Find("TeamPlayer" + i + "/RosterHolder").transform.position + new Vector3(5f, 0f, 0f);

            List<GameObject> rosterPlayer = (i == 1) ? rosterPlayer1 : rosterPlayer2;
            rosterPlayer = rosterPlayer
                .OrderByDescending(x => x.transform.Find("ShipInfo/ShipPilotSkillText").GetComponent<Text>().text)
                .ThenBy(x => x.transform.Find("ShipInfo/ShipId").GetComponent<Text>().text)
                .ToList();

            float offset = 5;
            foreach (var item in rosterPlayer)
            {
                if (item.activeSelf)
                {
                    item.transform.position = defaultPosition + new Vector3(0f, -offset, 0f);
                    offset += item.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta.y + 5;
                }
            }
        }
    }

    public static void SelectShipByRosterClick(PointerEventData data)
    {
        foreach (var item in data.hovered)
        {
            if (item.tag != "Untagged") {
                if (Selection.TryToChangeShip(item.tag)) return;
            }
        }
        Game.UI.HideTemporaryMenus();
    }

    public static void SubscribeMarkByHover(GameObject sender)
    {
        EventTrigger trigger = sender.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            sender.AddComponent<EventTrigger>();
            trigger = sender.GetComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { HoverShipByRosterClick((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { Selection.TryUnmarkPreviousHoveredShip(); });
        trigger.triggers.Add(entry);
    }

    public static void HoverShipByRosterClick(PointerEventData data)
    {
        foreach (var item in data.hovered)
        {
            if (item.tag.StartsWith("ShipId:"))
            {
                Selection.TryMarkShip(item.tag);
            }
        }
    }

    //UPDATE

    public static void UpdateRosterShieldsDamageIndicators(Ship.GenericShip thisShip)
    {
        thisShip.InfoPanel.transform.Find("ShipInfo/ShipShieldsText").GetComponent<Text>().text = thisShip.Shields.ToString();
        foreach (Transform damageIndicator in thisShip.InfoPanel.transform.Find("ShipInfo/DamageBarPanel").transform)
        {
            string[] damageIndicatorData = damageIndicator.name.Split('.');
            string type = damageIndicatorData[1];
            int value = int.Parse(damageIndicatorData[2]);
            if (type == "Shield")
            {
                damageIndicator.gameObject.SetActive(value <= thisShip.Shields);
            }
        }
    }

    public static void UpdateShipStats(Ship.GenericShip thisShip)
    {
        if (thisShip.InfoPanel != null)
        {
            thisShip.InfoPanel.transform.Find("ShipInfo/ShipPilotSkillText").GetComponent<Text>().text = thisShip.PilotSkill.ToString();
            thisShip.InfoPanel.transform.Find("ShipInfo/ShipFirepowerText").GetComponent<Text>().text = thisShip.Firepower.ToString();
            thisShip.InfoPanel.transform.Find("ShipInfo/ShipAgilityText").GetComponent<Text>().text = thisShip.Agility.ToString();
        }
    }

    public static void UpdateRosterHullDamageIndicators(Ship.GenericShip thisShip)
    {
        thisShip.InfoPanel.transform.Find("ShipInfo/ShipHullText").GetComponent<Text>().text = thisShip.Hull.ToString();
        foreach (Transform damageIndicator in thisShip.InfoPanel.transform.Find("ShipInfo/DamageBarPanel").transform)
        {
            string[] damageIndicatorData = damageIndicator.name.Split('.');
            string type = damageIndicatorData[1];
            int value = int.Parse(damageIndicatorData[2]);
            if (type == "Hull")
            {
                damageIndicator.gameObject.SetActive(value <= thisShip.Hull);
            }
        }

        //Todo: move
        thisShip.ToggleDamaged(thisShip.Hull == 1);
    }

    public static void UpdateTokensIndicator(Ship.GenericShip thisShip, System.Type type)
    {
        List<GameObject> keys = new List<GameObject>();
        foreach (Transform icon in thisShip.InfoPanel.transform.Find("ShipInfo/TokensBar").transform)
        {
            keys.Add(icon.gameObject);
        }
        foreach (GameObject icon in keys)
        {
            icon.gameObject.SetActive(false);
            MonoBehaviour.Destroy(icon);
        }

        int columnCounter = 0;
        int rowCounter = 0;
        foreach (var token in thisShip.GetAssignedTokens())
        {
            for (int i = 0; i < token.Count; i++)
            {
                GameObject tokenPanel = MonoBehaviour.Instantiate(Game.PrefabsList.PanelToken, thisShip.InfoPanel.transform.Find("ShipInfo").Find("TokensBar"));
                tokenPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
                tokenPanel.name = token.Name;
                Tooltips.AddTooltip(tokenPanel, token.Tooltip);
                tokenPanel.transform.Find(token.Name).gameObject.SetActive(true);

                if (token.GetType().BaseType == typeof(Tokens.GenericTargetLockToken))
                {
                    tokenPanel.transform.Find(token.Name).Find("Letter").GetComponent<Text>().text = (token as Tokens.GenericTargetLockToken).Letter.ToString();
                }

                tokenPanel.SetActive(true);
                tokenPanel.GetComponent<RectTransform>().localPosition = new Vector3(columnCounter * 37, tokenPanel.GetComponent<RectTransform>().localPosition.y + -37 * rowCounter, tokenPanel.GetComponent<RectTransform>().localPosition.z);
                columnCounter++;
                if (columnCounter == 5)
                {
                    rowCounter++;
                    columnCounter = 0;
                }
            }
        }

        OrganizeRosters();
    }

    public static void UpdateUpgradesPanel(Ship.GenericShip newShip, GameObject newPanel)
    {
        int index = 1;
        foreach (var upgrade in newShip.InstalledUpgrades)
        {
            GameObject upgradeNamePanel = newPanel.transform.Find("ShipInfo/UpgradesBar/Upgrade"+index).gameObject;
            upgradeNamePanel.GetComponent<Text>().text = upgrade.Value.ShortName;
            upgradeNamePanel.SetActive(true);
            index++;
        }
        OrganizeRosters();
    }

    public static void SubscribeUpgradesPanel(Ship.GenericShip newShip, GameObject newPanel)
    {
        int index = 1;
        foreach (var upgrade in newShip.InstalledUpgrades)
        {
            GameObject upgradeNamePanel = newPanel.transform.Find("ShipInfo/UpgradesBar/Upgrade" + index).gameObject;

            SubscribeActions(upgradeNamePanel);
            Tooltips.AddTooltip(upgradeNamePanel, upgrade.Value.ImageUrl);

            index++;
        }
    }

    public static void DiscardUpgrade(Ship.GenericShip host, string upgradeShortName)
    {
        foreach (Transform upgradeLine in host.InfoPanel.transform.Find("ShipInfo/UpgradesBar").transform)
        {
            if (upgradeLine.GetComponent<Text>().text == upgradeShortName && upgradeLine.GetComponent<Text>().color != Color.gray)
            {
                upgradeLine.GetComponent<Text>().color = Color.gray;
                return;
            }
        }
    }

    private static void HideAssignedDials()
    {
        foreach (var panel in rosterPlayer1) panel.transform.Find("DialAssigned1").gameObject.SetActive(false);
        foreach (var panel in rosterPlayer2) panel.transform.Find("DialAssigned2").gameObject.SetActive(false);
    }

    public static void RosterAllPanelsHighlightOff()
    {
        foreach (var ship in Roster.AllShips)
        {
            RosterPanelHighlightOff(ship.Value);
        }
    }

    public static void RosterPanelHighlightOn(Ship.GenericShip ship)
    {
        ship.InfoPanel.transform.Find("ShipInfo").GetComponent<Animator>().enabled = true;
    }

    public static void RosterPanelHighlightOff(Ship.GenericShip ship)
    {
        ship.InfoPanel.transform.Find("ShipInfo").GetComponent<Animator>().enabled = false;
        ship.InfoPanel.transform.Find("ShipInfo").GetComponent<Image>().color = new Color32(0, 0, 0, 200);
    }

    public static void MarkShip(Ship.GenericShip ship, Color color)
    {
        ship.InfoPanel.transform.Find("Mark").GetComponent<Canvas>().enabled = true;
        ship.InfoPanel.transform.Find("Mark").GetComponent<Image>().color = color;
    }

    public static void UnMarkShip(Ship.GenericShip ship)
    {
        ship.InfoPanel.transform.Find("Mark").GetComponent<Canvas>().enabled = false;
    }

}
