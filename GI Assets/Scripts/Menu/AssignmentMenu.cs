using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignmentMenu : MonoBehaviour
{
    [SerializeField]
    Text[] texts;
    [SerializeField]
    string[] menuTexts;
    [SerializeField]
    string[] menuTextsES;
    [SerializeField]
    string[] warnTexts;
    [SerializeField]
    string[] warnTextsES;
    [SerializeField]
    string[] descriptions;
    [SerializeField]
    string[] descriptionsES;
    [SerializeField]
    int[] progressScores;
    [SerializeField]
    Button[] progressButtons;
    [SerializeField]
    List<Assignment> assignments;
    [SerializeField]
    List<GameObject> toDeactivate;
    [SerializeField]
    GameObject completedIcon;
    [SerializeField]
    Button claimButton;
    [SerializeField]
    Text description;
    [SerializeField]
    Text claimText;
    [SerializeField]
    Image progressBar;
    [SerializeField]
    Image assignmentIcon;
    [SerializeField]
    GameObject inventory;
    [SerializeField]
    Image rewardIcon;
    [SerializeField]
    Sprite[] rewardIcons;
    [SerializeField]
    Text timeLeftText;
    [SerializeField]
    GameObject assignmentItem;
    [SerializeField]
    Transform contents;
    [SerializeField]
    Transform info;
    [SerializeField]
    string[] infoTexts;
    [SerializeField]
    string[] infoTextsES;
    [SerializeField]
    string[] infoNames;
    [SerializeField]
    string[] infoNamesES;
    [SerializeField]
    GameObject confirmButton;
    Assignment dailyAssignment;
    bool[] progressClaims = new bool[8];
    string[] trueDescs = new string[20];
    string[] trueWarnTexts = new string[6];
    InventoryManager manager;
    MenuManager menu;
    List<Item>[] filteredLists = new List<Item>[3];
    System.DateTime target;
    List<Assignment> viableAssignments = new List<Assignment>();
    Item itemToAdd;
    Item itemToFind;
    int randomNumber;
    int oldDay;
    int highScore;
    bool left;
    bool viable;
    int language;
    string[] trueInfoNames = new string[3];
    string[] trueInfoTexts = new string[3];
    Transform oldButton;
    Vector2 itemStartPos;
    RectTransform itemRect;
    System.TimeSpan diff;
    GameObject ins;

    void Start()
    {
        if (ES3.KeyExists("highScore"))
        {
            highScore = ES3.Load<int>("highScore");
            progressBar.fillAmount = highScore / 5000f;

            if (ES3.KeyExists("progressClaims"))
            {
                progressClaims = ES3.Load<bool[]>("progressClaims");
            }
        }

        SetLanguage();

        if (ES3.KeyExists("oldDayA") && ES3.KeyExists("timeTargetA"))
        {
            oldDay = ES3.Load<int>("oldDayA");
            target = ES3.Load<System.DateTime>("timeTargetA");
        }

        if (System.DateTime.UtcNow.ToLocalTime().DayOfYear == oldDay)
        {
            dailyAssignment = ES3.Load<Assignment>("dailyAssignment");
            UpdateAssignment(false);
        }

        manager = FindObjectOfType<InventoryManager>();
        menu = FindObjectOfType<MenuManager>();
    }

    void Update()
    {
        if(System.DateTime.UtcNow.ToLocalTime().DayOfYear != oldDay)
        {
            ChangeAssignment();
        }

        diff = target.Subtract(System.DateTime.UtcNow.ToLocalTime());
        timeLeftText.text = trueWarnTexts[0] + diff.ToString(@"hh\:mm\:ss");
    }

    void ChangeAssignment()
    {
        viable = false;
        oldDay = System.DateTime.UtcNow.ToLocalTime().DayOfYear;
        ES3.Save<int>("oldDayA", oldDay);
        target = new System.DateTime(System.DateTime.UtcNow.ToLocalTime().Year, System.DateTime.UtcNow.ToLocalTime().Month, System.DateTime.UtcNow.ToLocalTime().Day, 0, 0, 0);
        target = target.AddDays(1);
        ES3.Save<System.DateTime>("timeTargetA", target);

        for(int a = 0; a < assignments.Count; a++)
        {
            if(assignments[a].aminoObjective == 0 && assignments[a].gunObjective == 0)
            {
                viableAssignments.Add(assignments[a]);
            }

            else if(assignments[a].aminoObjective != 0)
            {
                for(int b = 0; b < manager.unlockedLists[0].Count; b++)
                {
                    if(manager.unlockedLists[0][b].aminoNumber == assignments[a].aminoObjective)
                    {
                        viableAssignments.Add(assignments[a]);
                    }
                }

                if(assignments[a].gunObjective != 0)
                {
                    for (int b = 0; b < manager.unlockedLists[1].Count; b++)
                    {
                        if (manager.unlockedLists[1][b].aminoNumber == assignments[a].gunObjective)
                        {
                            viable = true;
                        }
                    }

                    if(!viable)
                    {
                        viableAssignments.Remove(assignments[a]);
                    }
                }
            }

            else
            {
                for (int b = 0; b < manager.unlockedLists[1].Count; b++)
                {
                    if (manager.unlockedLists[1][b].aminoNumber == assignments[a].gunObjective)
                    {
                        viableAssignments.Add(assignments[a]);
                    }
                }
            }
        }

        randomNumber = Random.Range(0, viableAssignments.Count);
        dailyAssignment = viableAssignments[randomNumber];
        ES3.Save<Assignment>("dailyAssignment", dailyAssignment);
        UpdateAssignment(true);
    }

    public void UpdateAssignment(bool newAssignment)
    {
        if(newAssignment)
        {
            ES3.Save<bool>("completed", false);
            ES3.Save<bool>("claimed", false);
        }
        
        assignmentIcon.sprite = dailyAssignment.icon;
        description.text = trueDescs[dailyAssignment.assignmentNumber];
        rewardIcon.sprite = rewardIcons[dailyAssignment.reward];

        if(ES3.KeyExists("completed") && ES3.Load<bool>("completed") == true)
        {
            completedIcon.SetActive(true);

            if(ES3.KeyExists("claimed") && ES3.Load<bool>("claimed") == true)
            {
                claimText.text = trueWarnTexts[5];
            }

            else
            {
                claimText.text = trueWarnTexts[4];
                claimButton.enabled = true;
            }
        }
    }

    public void AssignmentClaim()
    {
        for (int i = 0; i < filteredLists.Length; i++)
        {
            filteredLists[i] = new List<Item>();
        }

        for (int a = 0; a < manager.lockedLists.Length; a++)
        {
            for (int b = 0; b < manager.lockedLists[a].Count; b++)
            {
                if (manager.lockedLists[a][b].unlockNumber > 0)
                {
                    filteredLists[a].Add(manager.lockedLists[a][b]);
                }
            }
        }

        if (dailyAssignment.reward < 3)
        {
            if(filteredLists[dailyAssignment.reward].Count > 0)
            {
                info.GetChild(0).GetComponent<Image>().sprite = rewardIcons[dailyAssignment.reward];
                info.GetChild(1).GetComponent<Text>().text = trueInfoNames[dailyAssignment.reward];
                info.GetChild(2).GetComponent<Text>().text = trueInfoTexts[dailyAssignment.reward];

                left = false;
                itemStartPos.x = manager.oGItemStartPos.x + 112.4f;
                itemStartPos.y = manager.oGItemStartPos.y + 100;

                for (int i = 0; i < filteredLists[dailyAssignment.reward].Count; i++)
                {
                    ins = Instantiate(assignmentItem, Vector3.zero, assignmentItem.transform.rotation);
                    ins.GetComponent<Image>().sprite = filteredLists[dailyAssignment.reward][i].icon;
                    ins.GetComponent<Image>().preserveAspect = true;
                    ins.transform.GetChild(0).GetComponent<Text>().text = manager.trueNames[filteredLists[dailyAssignment.reward][i].aminoNumber - 1];
                    ins.GetComponent<InventoryButton>().item = filteredLists[dailyAssignment.reward][i];
                    ins.transform.SetParent(contents);
                    itemRect = ins.GetComponent<RectTransform>();
                    itemRect.localScale = Vector3.one;

                    if (filteredLists[dailyAssignment.reward][i].unlockNumber > 0)
                    {
                        ins.transform.GetChild(1).GetComponent<Image>().fillAmount = filteredLists[dailyAssignment.reward][i].currentUnlockNumber / filteredLists[dailyAssignment.reward][i].unlockNumber;
                        ins.transform.GetChild(2).GetComponent<Text>().text = filteredLists[dailyAssignment.reward][i].currentUnlockNumber + "/" + filteredLists[dailyAssignment.reward][i].unlockNumber;

                        if (filteredLists[dailyAssignment.reward][i].currentUnlockNumber > 1)
                        {
                            ins.GetComponent<InventoryButton>().description = true;
                            ins.transform.GetChild(4).gameObject.SetActive(true);
                        }
                    }

                    if (left)
                    {
                        itemStartPos.x += 112.4f;
                        left = false;
                    }

                    else
                    {
                        itemStartPos.x -= 112.4f;
                        itemStartPos.y -= 100;
                        left = true;
                    }

                    itemRect.localPosition = itemStartPos;
                }

                if (filteredLists[dailyAssignment.reward].Count > 12)
                {
                    float count = filteredLists[dailyAssignment.reward].Count;
                    float division = count / 2f;
                    int num = Mathf.CeilToInt(division - 6);

                    RectTransform rectTrans = contents.GetComponent<RectTransform>();
                    rectTrans.sizeDelta = new Vector2(0, num * 100);
                    rectTrans.anchoredPosition = new Vector2(0, num * -100);
                }

                for (int i = 0; i < toDeactivate.Count; i++)
                {
                    toDeactivate[i].SetActive(false);
                }

                inventory.SetActive(true);
                menu.touchAbleB = false;
            }

            else
            {
                menu.WarnText(trueWarnTexts[3] + trueInfoNames[dailyAssignment.reward]);
            }
        }

        else if(dailyAssignment.reward == 3)
        {
            ES3.Save<bool>("claimed", true);
            claimButton.enabled = false;
            manager.lootboxNum += 1;
            ES3.Save<int>("lootboxNum", manager.lootboxNum);
            manager.lootboxIcon.GetComponentInChildren<Text>().text = "" + manager.lootboxNum;
            claimText.text = trueWarnTexts[5];

            menu.WarnText(trueWarnTexts[1]);
        }

        else if (dailyAssignment.reward == 4)
        {
            ES3.Save<bool>("claimed", true);
            claimButton.enabled = false;
            menu.carbons += 1;
            ES3.Save<int>("carbons", menu.carbons);
            menu.CarbonsUpdate();
            claimText.text = trueWarnTexts[5];

            menu.WarnText(trueWarnTexts[2]);
        }
    }

    public void ProgressClaim(Item toClaim, int claimNumber)
    {
        progressClaims[claimNumber] = true;
        ES3.Save<bool[]>("progressClaims", progressClaims);
        progressButtons[claimNumber].enabled = false;
        progressButtons[claimNumber].GetComponentInChildren<Text>().text = trueWarnTexts[5];
        manager.lockedLists[toClaim.type].Remove(toClaim);
        manager.unlockedLists[toClaim.type].Add(toClaim);

        menu.newInventoryImg.SetActive(true);
        ES3.Save<bool>("newInventory", true);
        ES3.Save<List<Item>[]>("unlockedLists", manager.unlockedLists);
        ES3.Save<List<Item>[]>("lockedLists", manager.lockedLists);

        switch(language)
        {
            case 0:
                menu.WarnText("You have obtained the " + manager.trueNames[toClaim.aminoNumber - 1].ToLowerInvariant() + " " + trueInfoNames[toClaim.type].TrimEnd('s').ToLower());
                break;

            case 1:
                menu.WarnText("Has obtenido el " + trueInfoNames[toClaim.type].TrimEnd('s').ToLower() + " " + manager.trueNames[toClaim.aminoNumber - 1].ToLowerInvariant());
                break;
        }

        manager.InventoryUpdate();
    }

    public void ConfirmPress()
    {
        ES3.Save<bool>("claimed", true);
        claimButton.enabled = false;

        if (itemToAdd.currentUnlockNumber + 1 == itemToAdd.unlockNumber)
        {
            manager.lockedLists[dailyAssignment.reward].Remove(itemToAdd);
            manager.unlockedLists[dailyAssignment.reward].Add(itemToAdd);
            menu.newInventoryImg.SetActive(true);
            ES3.Save<bool>("newInventory", true);
        }

        else
        {
            itemToFind = manager.lockedLists[dailyAssignment.reward].Find(x => x.name == itemToAdd.name);
            itemToFind.currentUnlockNumber += 1;
        }

        ES3.Save<List<Item>[]>("unlockedLists", manager.unlockedLists);
        ES3.Save<List<Item>[]>("lockedLists", manager.lockedLists);

        manager.InventoryUpdate();

        claimText.text = trueWarnTexts[5];
        inventory.SetActive(false);
        menu.touchAbleB = true;

        for (int i = 0; i < toDeactivate.Count; i++)
        {
            toDeactivate[i].SetActive(true);
        }

        switch (language)
        {
            case 0:
                menu.WarnText("You have obtained one " + manager.trueNames[itemToAdd.aminoNumber - 1].ToLowerInvariant() + " bit");
                break;

            case 1:
                menu.WarnText("Has obtenido un trozo de " + manager.trueNames[itemToAdd.aminoNumber - 1].ToLowerInvariant());
                break;
        }
    }

    public void InventoryPress(Item item, bool description, GameObject button)
    {
        if(oldButton != null)
        {
            oldButton.transform.GetChild(5).gameObject.SetActive(false);
        }
        
        itemToAdd = item;
        info.GetChild(0).GetComponent<Image>().sprite = item.icon;
        info.GetChild(1).GetComponent<Text>().text = manager.trueNames[item.aminoNumber - 1];

        if (description)
        {
            info.GetChild(2).GetComponent<Text>().text = manager.trueDescs[item.aminoNumber - 1];
        }

        else
        {
            info.GetChild(2).GetComponent<Text>().text = "???";
        }

        button.transform.GetChild(5).gameObject.SetActive(true);
        confirmButton.SetActive(true);
        oldButton = button.transform;
    }

    public void SetLanguage()
    {
        language = ES3.Load<int>("languageIndex");

        switch (language)
        {
            case 0:
                trueWarnTexts = warnTexts;
                trueDescs = descriptions;
                trueInfoNames = infoNames;
                trueInfoTexts = infoTexts;

                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = menuTexts[i];
                }

                for (int i = 0; i < progressButtons.Length; i++)
                {
                    if (highScore >= progressScores[i] && !progressClaims[i])
                    {
                        progressButtons[i].enabled = true;
                        progressButtons[i].GetComponentInChildren<Text>().text = trueWarnTexts[4];
                    }

                    else if (highScore >= progressScores[i])
                    {
                        progressButtons[i].GetComponentInChildren<Text>().text = trueWarnTexts[5];
                    }
                }

                break;

            case 1:
                trueWarnTexts = warnTextsES;
                trueDescs = descriptionsES;
                trueInfoNames = infoNamesES;
                trueInfoTexts = infoTextsES;

                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = menuTextsES[i];
                }

                for (int i = 0; i < progressButtons.Length; i++)
                {
                    if (highScore >= progressScores[i] && !progressClaims[i])
                    {
                        progressButtons[i].enabled = true;
                        progressButtons[i].GetComponentInChildren<Text>().text = trueWarnTexts[4];
                    }

                    else if (highScore >= progressScores[i])
                    {
                        progressButtons[i].GetComponentInChildren<Text>().text = trueWarnTexts[5];
                    }
                }

                break;
        }
    }
}
