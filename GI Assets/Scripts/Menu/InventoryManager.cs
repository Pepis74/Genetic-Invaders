using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public List<Item>[] unlockedLists = new List<Item>[3];
    public List<Item>[] lockedLists = new List<Item>[3];
    public int lootboxNum;
    public GameObject lootboxIcon;
    public string[] trueDescs;
    public string[] trueNames;
    [SerializeField]
    GameObject loading;
    [SerializeField]
    Text[] texts;
    [SerializeField]
    string[] menuTexts;
    [SerializeField]
    string[] menuTextsES;
    [SerializeField]
    int version;
    [SerializeField]
    [TextArea(3, 10)]
    string[] descriptions;
    [SerializeField]
    [TextArea(3, 10)]
    string[] descriptionsES;
    [SerializeField]
    string[] names;
    [SerializeField]
    string[] namesES;
    [SerializeField]
    GameObject toMenu;
    [SerializeField]
    List<Item> unlockedAminos = new List<Item>();
    [SerializeField]
    List<Item> lockedAminos = new List<Item>();
    [SerializeField]
    List<Item> unlockedGuns = new List<Item>();
    [SerializeField]
    List<Item> lockedGuns = new List<Item>();
    [SerializeField]
    List<Item> unlockedSkins = new List<Item>();
    [SerializeField]
    List<Item> lockedSkins = new List<Item>();
    [SerializeField]
    Transform[] unlockedContents = new Transform[3];
    [SerializeField]
    Transform[] lockedContents = new Transform[3];
    [SerializeField]
    GameObject unlockedItem;
    [SerializeField]
    GameObject lockedItem;
    public Vector2 oGItemStartPos;
    [SerializeField]
    GameObject aminos;
    [SerializeField]
    GameObject guns;
    [SerializeField]
    GameObject skins;
    [SerializeField]
    GameObject[] unlockedButtons = new GameObject[3];
    [SerializeField]
    GameObject[] lockedButtons = new GameObject[3];
    [SerializeField]
    Transform aminoInfo;
    [SerializeField]
    Transform gunInfo;
    [SerializeField]
    Transform skinInfo;
    [SerializeField]
    Color current;
    [SerializeField]
    Color notCurrent;
    [SerializeField]
    GameObject inventory;
    [SerializeField]
    SpriteRenderer[] shipGuns = new SpriteRenderer[2];
    [SerializeField]
    SpriteRenderer[] shipSkins = new SpriteRenderer[2];
    RectTransform itemRect;
    Transform propellers;
    GameObject selectedAmino;
    GameObject selectedGun;
    GameObject selectedSkin;
    MenuManager manager;
    List<Item>[] oldunlocked = new List<Item>[3];
    List<Item>[] oldlocked = new List<Item>[3];
    int versiondiff;
    bool left;
    int language;
    GameObject ins;
    Vector2 itemStartPos;

    void Start()
    {
        SetLanguage();

        manager = FindObjectOfType<MenuManager>();

        ES3.Save<string[]>("trueNames", trueNames);

        if (ES3.KeyExists("unlockedLists"))
        {
            if (version == ES3.Load<int>("version"))
            {
                unlockedLists = ES3.Load<List<Item>[]>("unlockedLists");
                lockedLists = ES3.Load<List<Item>[]>("lockedLists");
            }

            else
            {
                oldunlocked = ES3.Load<List<Item>[]>("unlockedLists");
                oldlocked = ES3.Load<List<Item>[]>("lockedLists");

                unlockedLists[0] = unlockedAminos;
                unlockedLists[1] = unlockedGuns;
                unlockedLists[2] = unlockedSkins;
                lockedLists[0] = lockedAminos;
                lockedLists[1] = lockedGuns;
                lockedLists[2] = lockedSkins;

                for (int a = 0; a < unlockedLists.Length; a++)
                {
                    versiondiff = (unlockedLists[a].Count + lockedLists[a].Count) - (oldunlocked[a].Count + oldlocked[a].Count);

                    if (versiondiff > 0)
                    {
                        for(int b = 0; b < versiondiff; b++)
                        {
                            oldlocked[a].Add(lockedLists[a][lockedLists[a].Count + b]);
                        }

                        unlockedLists[a] = oldunlocked[a];
                        lockedLists[a] = oldlocked[a];
                    }
                }
            }
        }

        else
        {
            unlockedLists[0] = unlockedAminos;
            unlockedLists[1] = unlockedGuns;
            unlockedLists[2] = unlockedSkins;
            lockedLists[0] = lockedAminos;
            lockedLists[1] = lockedGuns;
            lockedLists[2] = lockedSkins;

            ES3.Save<List<Item>[]>("unlockedLists", unlockedLists);
            ES3.Save<List<Item>[]>("lockedLists", lockedLists);
        }

        propellers = shipSkins[0].transform.GetChild(0);

        for (int i = 0; i < unlockedLists[1].Count; i++)
        {
            if (unlockedLists[1][i].selected)
            {
                shipGuns[0].sprite = unlockedLists[1][i].icon;
                shipGuns[1].sprite = unlockedLists[1][i].icon;
                ins = Instantiate(unlockedLists[1][i].projectile, transform.position, transform.rotation);
                ins.SetActive(false);
            }
        }

        for (int i = 0; i < unlockedLists[2].Count; i++)
        {
            if (unlockedLists[2][i].selected)
            {
                shipSkins[0].sprite = unlockedLists[2][i].icon;
                shipSkins[1].sprite = unlockedLists[2][i].icon;
                ins = Instantiate(unlockedLists[2][i].propellers, propellers.position, unlockedLists[2][i].propellers.transform.rotation);
                ins.transform.SetParent(propellers);
            }
        }

        if(ES3.KeyExists("lootboxNum"))
        {
            lootboxNum = ES3.Load<int>("lootboxNum");
            lootboxIcon.GetComponentInChildren<Text>().text = "" + lootboxNum;
        }

        else
        {
            lootboxNum = 0;
            ES3.Save<int>("lootboxNum", lootboxNum);
        }

        InventoryUpdate();

        ES3.Save<string[]>("itemNames", names);
        ES3.Save<string[]>("itemNamesES", namesES);
        ES3.Save<int>("version", version);
    }

    public void InventoryUpdate()
    {
        for(int i = 0; i < 3; i++)
        {
            unlockedButtons[i].GetComponent<Image>().color = current;
            lockedButtons[i].GetComponent<Image>().color = notCurrent;
            unlockedButtons[i].transform.GetChild(1).gameObject.SetActive(true);
            lockedButtons[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        for(int a = 0; a < unlockedContents.Length; a++)
        {
            for (int b = 0; b < unlockedContents[a].transform.childCount; b++)
            {
                Destroy(unlockedContents[a].transform.GetChild(b).gameObject);
            }

            unlockedContents[a].GetComponent<RectTransform>().anchoredPosition = new Vector3(-0.1f, 0, 0);
            unlockedContents[a].GetComponent<RectTransform>().sizeDelta = new Vector2(-0.1f, 0);
        }

        for (int a = 0; a < lockedContents.Length; a++)
        {
            for (int b = 0; b < lockedContents[a].transform.childCount; b++)
            {
                Destroy(lockedContents[a].transform.GetChild(b).gameObject);
            }

            lockedContents[a].GetComponent<RectTransform>().anchoredPosition = new Vector3(-0.1f, 0, 0);
            lockedContents[a].GetComponent<RectTransform>().sizeDelta = new Vector2(-0.1f, 0);
        }

        for (int a = 0; a < unlockedLists.Length; a++)
        {
            left = false;
            itemStartPos.x = oGItemStartPos.x + 112.4f;
            itemStartPos.y = oGItemStartPos.y + 100;

            for (int b = 0; b < unlockedLists[a].Count; b++)
            {
                ins = Instantiate(unlockedItem, Vector3.zero, unlockedItem.transform.rotation);
                ins.GetComponent<InventoryButton>().item = unlockedLists[a][b];
                ins.GetComponent<Image>().sprite = unlockedLists[a][b].icon;
                ins.GetComponent<Image>().preserveAspect = true;
                ins.GetComponentInChildren<Text>().text = trueNames[unlockedLists[a][b].aminoNumber - 1];
                ins.transform.SetParent(unlockedContents[a]);
                itemRect = ins.GetComponent<RectTransform>();
                itemRect.localScale = Vector3.one;

                if (unlockedLists[a][b].selected)
                {
                    ins.transform.GetChild(1).gameObject.SetActive(true);

                    switch(a)
                    {
                        case 0:
                            selectedAmino = ins;
                            break;

                        case 1:
                            selectedGun = ins;
                            break;

                        case 2:
                            selectedSkin = ins;
                            break;
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

            if (unlockedLists[a].Count == 0)
            {
                lockedButtons[a].GetComponent<Image>().color = current;
                lockedButtons[a].transform.GetChild(1).gameObject.SetActive(true);
                unlockedButtons[a].SetActive(false);
            }

            else
            {
                unlockedButtons[a].SetActive(true);

                if (unlockedLists[a].Count > 12)
                {
                    float count = unlockedLists[a].Count;
                    float division = count / 2f;
                    int num = Mathf.CeilToInt(division - 6);

                    RectTransform rectTrans = unlockedContents[a].GetComponent<RectTransform>();
                    rectTrans.sizeDelta = new Vector2(0, num * 100);
                    rectTrans.anchoredPosition = new Vector2(0, num * -100);
                }
            }
        }

        for (int a = 0; a < lockedLists.Length; a++)
        {
            left = false;
            itemStartPos.x = oGItemStartPos.x + 112.4f;
            itemStartPos.y = oGItemStartPos.y + 100;

            for (int b = 0; b < lockedLists[a].Count; b++)
            {
                ins = Instantiate(lockedItem, Vector3.zero, lockedItem.transform.rotation);
                ins.GetComponent<Image>().sprite = lockedLists[a][b].icon;
                ins.GetComponent<Image>().preserveAspect = true;
                ins.transform.GetChild(0).GetComponent<Text>().text = trueNames[lockedLists[a][b].aminoNumber - 1];
                ins.GetComponent<InventoryButton>().item = lockedLists[a][b];

                if (lockedLists[a][b].unlockNumber > 0)
                {
                    ins.transform.GetChild(1).GetComponent<Image>().fillAmount = lockedLists[a][b].currentUnlockNumber / lockedLists[a][b].unlockNumber;
                    ins.transform.GetChild(2).GetComponent<Text>().text = lockedLists[a][b].currentUnlockNumber + "/" + lockedLists[a][b].unlockNumber;

                    if(lockedLists[a][b].currentUnlockNumber > 0)
                    {
                        ins.GetComponent<InventoryButton>().description = true;
                        ins.transform.GetChild(4).gameObject.SetActive(true);
                    }
                }

                else
                {
                    for(int c = 0; c < ins.transform.childCount - 1; c++)
                    {
                        Destroy(ins.transform.GetChild(c + 1).gameObject);
                    }
                }
                
                ins.transform.SetParent(lockedContents[a]);
                itemRect = ins.GetComponent<RectTransform>();
                itemRect.localScale = Vector3.one;

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

            if (lockedLists[a].Count == 0)
            {
                unlockedButtons[a].GetComponent<Image>().color = current;
                unlockedButtons[a].transform.GetChild(1).gameObject.SetActive(true);
                lockedButtons[a].SetActive(false);
            }

            else
            {
                lockedButtons[a].SetActive(true);

                if (lockedLists[a].Count > 12)
                {
                    float count = lockedLists[a].Count;
                    float division = count / 2f;
                    int num = Mathf.CeilToInt(division - 6);

                    RectTransform rectTrans = lockedContents[a].GetComponent<RectTransform>();
                    rectTrans.sizeDelta = new Vector2(0, num * 100);
                    rectTrans.anchoredPosition = new Vector2(0, num * -100);
                }
            }
        }

    }

    public void IconPress(Item item, GameObject button)
    {
        switch(item.type)
        {
            case 0:
                if (selectedAmino != null)
                {
                    selectedAmino.GetComponent<InventoryButton>().item.selected = false;
                    selectedAmino.transform.GetChild(1).gameObject.SetActive(false);
                }

                selectedAmino = button;
                aminoInfo.GetChild(0).GetComponent<Image>().sprite = item.icon;
                aminoInfo.GetChild(1).GetComponent<Text>().text = trueNames[item.aminoNumber - 1];
                aminoInfo.GetChild(3).GetComponent<Text>().text = trueDescs[item.aminoNumber - 1];
                break;

            case 1:
                selectedGun.GetComponent<InventoryButton>().item.selected = false;
                selectedGun.transform.GetChild(1).gameObject.SetActive(false);
                selectedGun = button;
                gunInfo.GetChild(0).GetComponent<Image>().sprite = item.icon;
                gunInfo.GetChild(1).GetComponent<Text>().text = trueNames[item.aminoNumber - 1];
                gunInfo.GetChild(3).GetComponent<Text>().text = trueDescs[item.aminoNumber - 1];

                for (int i = 0; i < shipGuns.Length; i++)
                {
                    shipGuns[i].sprite = item.icon;
                }

                break;

            case 2:
                selectedSkin.GetComponent<InventoryButton>().item.selected = false;
                selectedSkin.transform.GetChild(1).gameObject.SetActive(false);
                selectedSkin = button;
                skinInfo.GetChild(0).GetComponent<Image>().sprite = item.icon;
                skinInfo.GetChild(1).GetComponent<Text>().text = trueNames[item.aminoNumber - 1];
                skinInfo.GetChild(3).GetComponent<Text>().text = trueDescs[item.aminoNumber - 1];
                Destroy(propellers.GetChild(0).gameObject);
                ins = Instantiate(item.propellers, propellers.position, item.propellers.transform.rotation);
                ins.transform.SetParent(propellers);

                for (int i = 0; i < shipSkins.Length; i++)
                {
                    shipSkins[i].sprite = item.icon;
                }

                break;
        }

        item.selected = true;
        button.transform.GetChild(1).gameObject.SetActive(true);

        ES3.Save<List<Item>[]>("unlockedLists", unlockedLists);
    }

    public void LockedPress(Item item, bool description)
    {
        switch (item.type)
        {
            case 0:
                aminoInfo.GetChild(0).GetComponent<Image>().sprite = item.icon;
                aminoInfo.GetChild(1).GetComponent<Text>().text = trueNames[item.aminoNumber - 1];

                if(description)
                {
                    aminoInfo.GetChild(3).GetComponent<Text>().text = trueDescs[item.aminoNumber - 1];
                }

                else
                {
                    aminoInfo.GetChild(3).GetComponent<Text>().text = "???";
                }
               
                break;
                
            case 1:
                gunInfo.GetChild(0).GetComponent<Image>().sprite = item.icon;
                gunInfo.GetChild(1).GetComponent<Text>().text = trueNames[item.aminoNumber - 1];

                if (description)
                {
                    gunInfo.GetChild(3).GetComponent<Text>().text = trueDescs[item.aminoNumber - 1];
                }

                else
                {
                    gunInfo.GetChild(3).GetComponent<Text>().text = "???";
                }

                break;

            case 2:
                skinInfo.GetChild(0).GetComponent<Image>().sprite = item.icon;
                skinInfo.GetChild(1).GetComponent<Text>().text = trueNames[item.aminoNumber - 1];

                if (description)
                {
                    skinInfo.GetChild(3).GetComponent<Text>().text = trueDescs[item.aminoNumber - 1];
                }

                else
                {
                    skinInfo.GetChild(3).GetComponent<Text>().text = "???";
                }

                break;
        }
    }

    public void TypePress(int type)
    {
        switch (type)
        {
            case 0:
                skins.SetActive(false);
                guns.SetActive(false);
                aminos.SetActive(true);
                break;

            case 1:
                
                skins.SetActive(false);
                guns.SetActive(true);
                aminos.SetActive(false);
                break;

            case 2:
                skins.SetActive(true);
                guns.SetActive(false);
                aminos.SetActive(false);
                break;
        }

        inventory.SetActive(true);
        ES3.Save<bool>("newInventory", false);
        manager.newInventoryImg.SetActive(false);
        toMenu.SetActive(false);
        manager.touchAbleB = false;
    }

    public void GoToLocked(int type)
    {
        unlockedButtons[type].GetComponent<Image>().color = notCurrent;
        lockedButtons[type].GetComponent<Image>().color = current;
        unlockedButtons[type].transform.GetChild(1).gameObject.SetActive(false);
        lockedButtons[type].transform.GetChild(1).gameObject.SetActive(true);
    }

    public void GoToUnlocked(int type)
    {
        unlockedButtons[type].GetComponent<Image>().color = current;
        lockedButtons[type].GetComponent<Image>().color = notCurrent;
        unlockedButtons[type].transform.GetChild(1).gameObject.SetActive(true);
        lockedButtons[type].transform.GetChild(1).gameObject.SetActive(false);
    }

    public void CloseInventory()
    {
        manager.touchAbleB = true;
        lootboxIcon.SetActive(true);
        toMenu.SetActive(true);
        inventory.SetActive(false);
    }

    public void Store()
    {
        manager.touchAbleB = false;
        toMenu.SetActive(false);
        inventory.SetActive(false);
        loading.SetActive(true);
        SceneManager.LoadSceneAsync(4);
    }

    public void Lootbox()
    {
        if(lootboxNum > 0)
        {
            SceneManager.LoadScene(3);
        }

        else
        {
            switch(language)
            {
                case 0:
                    manager.WarnText("You don't have any lootboxes");
                    break;

                case 1:
                    manager.WarnText("No tienes cajas de botín");
                    break;
            }

            manager.errorSound.Play();
        }
    }

    public void SetLanguage()
    {
        language = ES3.Load<int>("languageIndex");

        switch(language)
        {
            case 0:

                trueDescs = descriptions;
                trueNames = names;

                for(int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = menuTexts[i];
                }

                break;

            case 1:

                trueDescs = descriptionsES;
                trueNames = namesES;

                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = menuTextsES[i];
                }

                break;
        }
    }
}
