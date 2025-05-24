using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Store : MonoBehaviour
{
    [SerializeField]
    Text[] texts;
    [SerializeField]
    string[] engineTexts;
    [SerializeField]
    string[] engineTextsES;
    [SerializeField]
    string[] skinTexts;
    [SerializeField]
    string[] skinTextsES;
    [SerializeField]
    Transform[] canvasToMove;
    [SerializeField]
    Transform[] skinPanels;
    [SerializeField]
    Text[] bitsText;
    public Text carbonsText;
    [SerializeField]
    GameObject surePanel;
    [SerializeField]
    Text[] skinCostTexts;
    [SerializeField]
    Text[] skinUnlockedTexts;
    [SerializeField]
    Text[] skinNameTexts;
    [SerializeField]
    Text[] skinBuyTexts;
    [SerializeField]
    Text[] skinBuyBitsTexts;
    [SerializeField]
    Text[] orTexts;
    [SerializeField]
    GameObject warnText;
    [SerializeField]
    GameObject lootboxButton;
    [SerializeField]
    AudioSource errorSound;
    Item[] itemsToBuy = new Item[24];
    string[] names;
    string[] namesES;
    float canvasSpeed;
    bool touchAbleA = true;
    bool touchAbleB = true;
    bool touchAbleC = true;
    int transactionTypeA;
    int skinNumberA;
    int priceA;
    float canvasToMoveTo;
    Vector2 oldTouchPos;
    Vector2[] oldCanvasPos = new Vector2[3];
    Vector2 touchPos;
    List<Item>[] unlockedLists = new List<Item>[3];
    List<Item>[] lockedLists = new List<Item>[3];
    List<Text[]> skinTextsList = new List<Text[]>();
    int[] bitsToBuy = new int[24];
    int language;
    int lootboxNum;
    int screen;
    public int carbons;
    int oldDay;
    bool cantBuy;
    System.TimeSpan diff;
    bool purchasedLootbox;
    System.DateTime target;
    Coroutine warnTextCo;
    WaitForSeconds touchWait = new WaitForSeconds(0.1f);
    WaitForSeconds swipeWait = new WaitForSeconds(0.3f);

    void Start()
    {
        language = ES3.Load<int>("languageIndex");

        skinTextsList.Add(skinCostTexts);
        skinTextsList.Add(skinUnlockedTexts);
        skinTextsList.Add(skinBuyTexts);
        skinTextsList.Add(skinBuyBitsTexts);
        skinTextsList.Add(orTexts);

        lootboxNum = ES3.Load<int>("lootboxNum");

        carbons = ES3.Load<int>("carbons");
        carbonsText.text = "" + carbons;

        unlockedLists = ES3.Load<List<Item>[]>("unlockedLists");
        lockedLists = ES3.Load<List<Item>[]>("lockedLists");

        for(int i = 0; i < lockedLists[2].Count; i++)
        {
            if(skinPanels[lockedLists[2][i].aminoNumber - 28] != null)
            {
                skinPanels[lockedLists[2][i].aminoNumber - 28].GetChild(0).gameObject.SetActive(true);
                skinPanels[lockedLists[2][i].aminoNumber - 28].GetChild(1).gameObject.SetActive(false);
                skinPanels[lockedLists[2][i].aminoNumber - 28].GetChild(2).GetComponentInChildren<Text>().text = lockedLists[2][i].currentUnlockNumber + "/" + lockedLists[2][i].unlockNumber;
                skinPanels[lockedLists[2][i].aminoNumber - 28].GetChild(2).GetChild(0).GetComponent<Image>().fillAmount = lockedLists[2][i].currentUnlockNumber / lockedLists[2][i].unlockNumber;
            }     
        }

        canvasSpeed = Screen.width * 0.066666666666666594827586206897f;

        float width = Screen.currentResolution.width;
        float height = Screen.currentResolution.height;

        float currentRes = width / height;

        if (currentRes < 0.51f)
        {
            canvasToMove[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(439.18f, 0);
            canvasToMove[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(-439.18f, 0);
        }

        names = ES3.Load<string[]>("itemNames");
        namesES = ES3.Load<string[]>("itemNamesES");

        if (language == 0)
        {
            for(int i = 0; i < texts.Length; i++)
            {
                texts[i].text = engineTexts[i];
            }

            for(int a = 0; a < skinTextsList.Count; a++)
            {
                if(a == 0)
                {
                    for (int b = 0; b < skinTextsList[a].Length; b++)
                    {
                        if (skinTextsList[a][b] != null)
                        {
                            skinTextsList[a][b].text = skinTexts[a] + "0C";
                        }       
                    }
                }

                else if (a == 2)
                {
                    for (int b = 0; b < skinTextsList[a].Length; b++)
                    {
                        if (skinTextsList[a][b] != null)
                        {
                            skinTextsList[a][b].text = skinTexts[a] + skinTextsList[a][b].transform.parent.GetComponent<BuyButton>().price + "C";
                        }               
                    }
                }

                else
                {
                    for (int b = 0; b < skinTextsList[a].Length; b++)
                    {
                        if (skinTextsList[a][b] != null)
                        {
                            skinTextsList[a][b].text = skinTexts[a];
                        }             
                    }
                }
            }

            for (int i = 0; i < skinNameTexts.Length; i++)
            {
                if(skinNameTexts[i] != null)
                {
                    skinNameTexts[i].text = names[i + 27];
                }               
            }
        }

        else
        {
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].text = engineTextsES[i];
            }

            for (int a = 0; a < skinTextsList.Count; a++)
            {
                if (a == 0)
                {
                    for (int b = 0; b < skinTextsList[a].Length; b++)
                    {
                        if (skinTextsList[a][b] != null)
                        {
                            skinTextsList[a][b].text = skinTextsES[a] + "0C";
                        }
                    }
                }

                else if (a == 2)
                {
                    for (int b = 0; b < skinTextsList[a].Length; b++)
                    {
                        if (skinTextsList[a][b] != null)
                        {
                            skinTextsList[a][b].text = skinTextsES[a] + skinTextsList[a][b].transform.parent.GetComponent<BuyButton>().price + "C";
                        }
                    }
                }

                else
                {
                    for (int b = 0; b < skinTextsList[a].Length; b++)
                    {
                        if (skinTextsList[a][b] != null)
                        {
                            skinTextsList[a][b].text = skinTextsES[a];
                        }
                    }
                }
            }

            for (int i = 0; i < skinNameTexts.Length; i++)
            {
                if (skinNameTexts[i] != null)
                {
                    skinNameTexts[i].text = namesES[i + 27];
                }
            }
        }

        if (ES3.KeyExists("oldDayS") && ES3.KeyExists("timeTargetS"))
        {
            oldDay = ES3.Load<int>("oldDayS");
            target = ES3.Load<System.DateTime>("timeTargetS");
        }

        if (ES3.KeyExists("purchasedLootbox"))
        {
            purchasedLootbox = ES3.Load<bool>("purchasedLootbox");
        }

        else
        {
            ES3.Save<bool>("purchasedLootbox", purchasedLootbox);
        }

        if(purchasedLootbox)
        {
            lootboxButton.SetActive(false);
            texts[13].gameObject.SetActive(true);

            if(language == 0)
            {
                texts[8].text = engineTexts[17];
            }

            else
            {
                texts[8].text = engineTextsES[17];
            } 
        }
    }

    void Update()
    {
        if (System.DateTime.UtcNow.ToLocalTime().DayOfYear != oldDay)
        {
            oldDay = System.DateTime.UtcNow.ToLocalTime().DayOfYear;
            ES3.Save<int>("oldDayS", oldDay);
            target = new System.DateTime(System.DateTime.UtcNow.ToLocalTime().Year, System.DateTime.UtcNow.ToLocalTime().Month, System.DateTime.UtcNow.ToLocalTime().Day, 0, 0, 0);
            target = target.AddDays(1);
            ES3.Save<System.DateTime>("timeTargetS", target);
            purchasedLootbox = false;
            ES3.Save<bool>("purchasedLootbox", purchasedLootbox);
            texts[13].gameObject.SetActive(false);
            lootboxButton.SetActive(true);

            if (language == 0)
            {
                texts[8].text = engineTexts[8];
            }

            else
            {
                texts[8].text = engineTextsES[8];
            }  
        }

        diff = target.Subtract(System.DateTime.UtcNow.ToLocalTime());

        if (language == 0)
        {
            texts[13].text = engineTexts[13] + diff.ToString(@"hh\:mm\:ss");
        }

        else
        {
            texts[13].text = engineTextsES[13] + diff.ToString(@"hh\:mm\:ss");
        }
    }

    void FixedUpdate()
    {
        if (Input.touchCount > 0 && touchAbleA && touchAbleB && touchAbleC)
        {
            oldTouchPos = Input.touches[0].position;
            StartCoroutine(TouchWaitCo());
            touchAbleC = false;
        }

        if (canvasToMoveTo != 0)
        {
            for (int i = 0; i < canvasToMove.Length; i++)
            {
                canvasToMove[i].position = Vector2.MoveTowards(canvasToMove[i].position, new Vector2(oldCanvasPos[i].x + canvasToMoveTo, canvasToMove[i].position.y), canvasSpeed * 2);
            }
        }
    }
    
    public void BuyCarbons(int buyIndex)
    {
        IAPManager.instance.BuyProduct(buyIndex);
    }

    public void ToLeft()
    {
        if (touchAbleA && touchAbleB)
        {
            touchAbleA = false;

            for (int i = 0; i < oldCanvasPos.Length; i++)
            {
                oldCanvasPos[i] = canvasToMove[i].position;
            }

            canvasToMoveTo = Screen.width;
            screen += 1;

            StartCoroutine(StopScreen());
        }
    }

    public void ToRight()
    {
        if (touchAbleA && touchAbleB)
        {
            touchAbleA = false;

            for (int i = 0; i < oldCanvasPos.Length; i++)
            {
                oldCanvasPos[i] = canvasToMove[i].position;
            }

            canvasToMoveTo = -Screen.width;
            screen -= 1;

            StartCoroutine(StopScreen());
        }
    }

    public void AddBits(int skinNumber)
    {
        if(itemsToBuy[skinNumber] == null)
        {
            itemsToBuy[skinNumber] = lockedLists[2].Find(x => x.aminoNumber == skinNumber + 28);
        }

        if(bitsToBuy[skinNumber] != itemsToBuy[skinNumber].unlockNumber - itemsToBuy[skinNumber].currentUnlockNumber)
        {
            bitsToBuy[skinNumber] += 1;
            bitsText[skinNumber].text = "" + bitsToBuy[skinNumber];

            if(language == 0)
            {
                skinCostTexts[skinNumber].text = skinTexts[0] + bitsToBuy[skinNumber] + "C";
            }

            else
            {
                skinCostTexts[skinNumber].text = skinTextsES[0] + bitsToBuy[skinNumber] + "C";
            }
        }

        else
        {
            errorSound.Play();
        }
    }

    public void RemoveBits(int skinNumber)
    {
        if(bitsToBuy[skinNumber] != 0)
        {
            bitsToBuy[skinNumber] -= 1;
            bitsText[skinNumber].text = "" + bitsToBuy[skinNumber];

            if (language == 0)
            {
                skinCostTexts[skinNumber].text = skinTexts[0] + bitsToBuy[skinNumber] + "C";
            }

            else
            {
                skinCostTexts[skinNumber].text = skinTextsES[0] + bitsToBuy[skinNumber] + "C";
            }
        }

        else
        {
            errorSound.Play();
        }
    }

    public void Buy(int transactionTypeB, int skinNumberB, int priceB)
    {
        if(!surePanel.activeSelf)
        {
            cantBuy = false;

            if (priceB == 0)
            {
                priceB = bitsToBuy[skinNumberB];

                if (priceB == 0)
                {
                    if (language == 0)
                    {
                        WarnText("You can't buy 0 bits");
                    }

                    else
                    {
                        WarnText("No puedes comprar 0 trozos");
                    }

                    errorSound.Play();
                    cantBuy = true;
                }
            }

            if (!cantBuy)
            {
                if (carbons - priceB >= 0)
                {
                    transactionTypeA = transactionTypeB;
                    skinNumberA = skinNumberB;
                    priceA = priceB;
                    surePanel.SetActive(true);
                    touchAbleB = false;
                }

                else
                {
                    switch (language)
                    {
                        case 0:
                            WarnText("You don't have enough carbons");
                            break;

                        case 1:
                            WarnText("No tienes suficientes carbonos");
                            break;
                    }

                    errorSound.Play();
                }
            }
        }
    }

    public void Confirm()
    {
        switch(transactionTypeA)
        {
            case 0:
                lootboxNum += 1;
                purchasedLootbox = true;
                ES3.Save<int>("lootboxNum", lootboxNum);    
                ES3.Save<bool>("purchasedLootbox", purchasedLootbox);

                lootboxButton.SetActive(false);
                texts[13].gameObject.SetActive(true);

                if (language == 0)
                {
                    texts[8].text = engineTexts[17];
                }

                else
                {
                    texts[8].text = engineTextsES[17];
                }
                break;

            case 1:
                itemsToBuy[skinNumberA].currentUnlockNumber += bitsToBuy[skinNumberA];

                if(itemsToBuy[skinNumberA].currentUnlockNumber == itemsToBuy[skinNumberA].unlockNumber)
                {
                    lockedLists[2].Remove(itemsToBuy[skinNumberA]);
                    unlockedLists[2].Add(itemsToBuy[skinNumberA]);
                    skinPanels[skinNumberA].GetChild(0).gameObject.SetActive(false);
                    skinPanels[skinNumberA].GetChild(1).gameObject.SetActive(true);
                    ES3.Save<bool>("newInventory", true);
                }

                ES3.Save<List<Item>[]>("unlockedLists", unlockedLists);
                ES3.Save<List<Item>[]>("lockedLists", lockedLists);

                bitsToBuy[skinNumberA] = 0;
                bitsText[skinNumberA].text = "" + bitsToBuy[skinNumberA];

                if (language == 0)
                {
                    skinCostTexts[skinNumberA].text = skinTexts[0] + bitsToBuy[skinNumberA] + "C";
                }

                else
                {
                    skinCostTexts[skinNumberA].text = skinTextsES[0] + bitsToBuy[skinNumberA] + "C";
                }

                skinPanels[skinNumberA].GetChild(2).GetComponentInChildren<Text>().text = itemsToBuy[skinNumberA].currentUnlockNumber + "/" + itemsToBuy[skinNumberA].unlockNumber;
                skinPanels[skinNumberA].GetChild(2).GetChild(0).GetComponent<Image>().fillAmount = itemsToBuy[skinNumberA].currentUnlockNumber / itemsToBuy[skinNumberA].unlockNumber;
                break;

            case 2:
                if (itemsToBuy[skinNumberA] == null)
                {
                    itemsToBuy[skinNumberA] = lockedLists[2].Find(x => x.aminoNumber == skinNumberA + 28);
                }

                lockedLists[2].Remove(itemsToBuy[skinNumberA]);
                unlockedLists[2].Add(itemsToBuy[skinNumberA]);
                ES3.Save<List<Item>[]>("unlockedLists", unlockedLists);
                ES3.Save<List<Item>[]>("lockedLists", lockedLists);
                skinPanels[skinNumberA].GetChild(0).gameObject.SetActive(false);
                skinPanels[skinNumberA].GetChild(1).gameObject.SetActive(true);
                ES3.Save<bool>("newInventory", true);
                skinPanels[skinNumberA].GetChild(2).GetComponentInChildren<Text>().text = itemsToBuy[skinNumberA].unlockNumber + "/" + itemsToBuy[skinNumberA].unlockNumber;
                skinPanels[skinNumberA].GetChild(2).GetChild(0).GetComponent<Image>().fillAmount = 1;
                break;
        }

        carbons -= priceA;
        ES3.Save<int>("carbons", carbons);
        carbonsText.text = "" + carbons;

        surePanel.SetActive(false);
        touchAbleB = true;

        switch (language)
        {
            case 0:
                WarnText("Transaction completed!");
                break;

            case 1:
                WarnText("¡Transacción completada!");
                break;
        }
    }

    public void Cancel()
    {
        surePanel.SetActive(false);
        touchAbleB = true;
    }

    public void Back()
    {
        SceneManager.LoadScene(1);
    }

    void WarnText(string toSay)
    {
        if (warnTextCo != null)
        {
            StopCoroutine(warnTextCo);
        }

        warnText.GetComponent<Animator>().SetBool("Fade", false);
        warnText.GetComponentInChildren<Text>().text = toSay;
        warnText.SetActive(true);
        warnText.GetComponent<Animator>().SetBool("Fade", true);
        warnTextCo = StartCoroutine(WarnTextCo());
    }

    IEnumerator TouchWaitCo()
    {
        yield return touchWait;

        if (Input.touchCount > 0)
        {
            touchPos = Input.touches[0].position;

            if (Vector2.Distance(new Vector2(touchPos.x, 0), new Vector2(oldTouchPos.x, 0)) > 75)
            {
                touchAbleA = false;

                for (int i = 0; i < oldCanvasPos.Length; i++)
                {
                    oldCanvasPos[i] = canvasToMove[i].position;
                }

                if (touchPos.x > oldTouchPos.x && screen < 1)
                {
                    canvasToMoveTo = Screen.width;
                    screen += 1;
                }

                else if (touchPos.x < oldTouchPos.x && screen > -1)
                {
                    canvasToMoveTo = -Screen.width;
                    screen -= 1;
                }

                yield return swipeWait;

                canvasToMoveTo = 0;
                touchAbleA = true;

            }
        }

        touchAbleC = true;
    }

    IEnumerator WarnTextCo()
    {
        yield return new WaitForSeconds(2.49f);
        warnText.GetComponent<Animator>().SetBool("Fade", false);
        warnText.SetActive(false);
    }

    IEnumerator StopScreen()
    {
        yield return swipeWait;
        canvasToMoveTo = 0;
        touchAbleA = true;
        touchAbleC = true;
    }
}
