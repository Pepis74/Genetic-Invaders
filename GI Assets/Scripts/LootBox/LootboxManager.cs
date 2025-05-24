using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LootboxManager : MonoBehaviour
{
    [SerializeField]
    Text title;
    [SerializeField]
    Text[] itemTexts = new Text[6];
    [SerializeField]
    GameObject oKButton;
    [SerializeField]
    GameObject lootbox;
    [SerializeField]
    GameObject[] toDeactivate;
    [SerializeField]
    GameObject aminoBit;
    [SerializeField]
    GameObject gunBit;
    [SerializeField]
    GameObject skinBit;
    [SerializeField]
    GameObject singleCarbon;
    [SerializeField]
    GameObject butano;
    [SerializeField]
    string[] codeTexts;
    [SerializeField]
    string[] codeTextsES;
    [SerializeField]
    string[] engineTexts;
    GameObject[] ins = new GameObject[3];
    Item chosenItem;
    Item itemToFind;
    Animator anim;
    AudioSource buttonSound;
    string[] trueTexts = new string[8];
    int carbons;
    int lootboxNum;
    Item[] itemsToChange = new Item[3];
    Text[] textsToChange = new Text[3];
    List<Item>[] unlockableLists = new List<Item>[3];
    List<Item>[] unlockedLists = new List<Item>[3];
    List<Item>[] lockedLists = new List<Item>[3];
    string[] trueNames = new string[51];
    int initialRandom;
    int language;
    int secondaryRandom;

    void Start()
    {
        language = ES3.Load<int>("languageIndex");

        switch(language)
        {
            case 0:
                trueTexts = codeTexts;
                toDeactivate[0].GetComponent<Text>().text = engineTexts[0];
                title.text = engineTexts[2];

                break;

            case 1:
                trueTexts = codeTextsES;
                toDeactivate[0].GetComponent<Text>().text = engineTexts[1];
                title.text = engineTexts[3];

                break;
        }

        for (int i = 0; i < unlockableLists.Length; i++)
        {
            unlockableLists[i] = new List<Item>();
        }

        trueNames = ES3.Load<string[]>("trueNames");
        unlockedLists = ES3.Load<List<Item>[]>("unlockedLists");
        lockedLists = ES3.Load<List<Item>[]>("lockedLists");
        carbons = ES3.Load<int>("carbons");
        lootboxNum = ES3.Load<int>("lootboxNum");
        anim = lootbox.GetComponent<Animator>();
        buttonSound = GetComponent<AudioSource>();

        for(int a = 0; a < lockedLists.Length; a++)
        {
            for(int b = 0; b < lockedLists[a].Count; b++)
            {
                if(lockedLists[a][b].unlockNumber > 0)
                {
                    unlockableLists[a].Add(lockedLists[a][b]);
                }
            }
        }

        StartCoroutine(ActivateButtonCo());
    }

    public void Open()
    {
        for(int i = 0; i < toDeactivate.Length; i++)
        {
            toDeactivate[i].SetActive(false);
        }

        lootboxNum -= 1;
        ES3.Save<int>("lootboxNum", lootboxNum);
        anim.SetBool("Open", true);

        StartCoroutine(OpenCo());
    }

    public void Ok()
    {
        oKButton.SetActive(false);
        PlaySound();

        for(int i = 0; i < ins.Length; i++)
        {
            Destroy(ins[i]);
        }

        itemTexts[0].transform.parent.gameObject.SetActive(false);

        if (lootboxNum > 0)
        {
            anim.SetBool("Open", false);
            StartCoroutine(ActivateButtonCo());
        }

        toDeactivate[0].GetComponent<Text>().text = trueTexts[0] + lootboxNum + trueTexts[1];
        toDeactivate[0].SetActive(true);
        toDeactivate[2].SetActive(true);
    }

    public void Back()
    {
        PlaySound();
        SceneManager.LoadScene(1);
    }

    void PlaySound()
    {
        buttonSound.Play();
    }

    IEnumerator ActivateButtonCo()
    {
        yield return new WaitForSeconds(0.67f);
        toDeactivate[1].SetActive(true);
    }

    IEnumerator OpenCo()
    {
        yield return new WaitForSeconds(1.55f);
         
        for (int i = 0; i < 3; i++)
        {
            initialRandom = Random.Range(0, 100);

            if (initialRandom < 44)
            {
                if(unlockableLists[0].Count > 0)
                {
                    secondaryRandom = Random.Range(0, unlockableLists[0].Count);

                    chosenItem = unlockableLists[0][secondaryRandom];
                    ins[i] = Instantiate(aminoBit, Vector2.zero, aminoBit.transform.rotation);
                    ins[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = chosenItem.color1;
                    ins[i].transform.GetChild(1).GetComponent<SpriteRenderer>().color = chosenItem.color2;

                    if (chosenItem.currentUnlockNumber + 1 == chosenItem.unlockNumber)
                    {
                        unlockableLists[0].Remove(chosenItem);
                        lockedLists[0].Remove(chosenItem);
                        unlockedLists[0].Add(chosenItem);
                        itemTexts[i + 3].text = trueTexts[2];
                        ES3.Save<bool>("newInventory", true);
                    }

                    else
                    {
                        itemToFind = lockedLists[0].Find(x => x.aminoNumber == chosenItem.aminoNumber);
                        itemToFind.currentUnlockNumber += 1;

                        if (chosenItem.currentUnlockNumber != 1)
                        {
                            textsToChange[i] = itemTexts[i + 3];
                            itemsToChange[i] = chosenItem;
                        }

                        else
                        {
                            itemTexts[i + 3].text = trueTexts[5];
                        }
                    }

                    switch(language)
                    {
                        case 0:
                            itemTexts[i].text = trueNames[chosenItem.aminoNumber - 1] + " BIT";

                            break;

                        case 1:
                            itemTexts[i].text = "TROZO DE " + trueNames[chosenItem.aminoNumber - 1];

                            break;
                    }
                }

                else
                {
                    AddCarbons(i);
                }
            }

            else if (initialRandom >= 44 && initialRandom < 88)
            {
                if (unlockableLists[1].Count > 0)
                {
                    secondaryRandom = Random.Range(0, unlockableLists[1].Count);

                    chosenItem = unlockableLists[1][secondaryRandom];
                    ins[i] = Instantiate(gunBit, Vector2.zero, gunBit.transform.rotation);
                    ins[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = chosenItem.color1;
                    ins[i].transform.GetChild(1).GetComponent<SpriteRenderer>().color = chosenItem.color2;

                    if (chosenItem.currentUnlockNumber + 1 == chosenItem.unlockNumber)
                    {
                        unlockableLists[1].Remove(chosenItem);
                        lockedLists[1].Remove(chosenItem);
                        unlockedLists[1].Add(chosenItem);
                        itemTexts[i + 3].text = trueTexts[2];
                        ES3.Save<bool>("newInventory", true);
                    }

                    else
                    {
                        itemToFind = lockedLists[1].Find(x => x.aminoNumber == chosenItem.aminoNumber);
                        itemToFind.currentUnlockNumber += 1;

                        if (chosenItem.currentUnlockNumber != 1)
                        {
                            textsToChange[i] = itemTexts[i + 3];
                            itemsToChange[i] = chosenItem;
                        }

                        else
                        {
                            itemTexts[i + 3].text = trueTexts[5];
                        }
                    }

                    switch (language)
                    {
                        case 0:
                            itemTexts[i].text = trueNames[chosenItem.aminoNumber - 1] + " BIT";

                            break;

                        case 1:
                            itemTexts[i].text = "TROZO DE " + trueNames[chosenItem.aminoNumber - 1];

                            break;
                    }
                }

                else
                {
                    AddCarbons(i);
                }
            }

            else if (initialRandom >= 88 & initialRandom < 99)
            {
                if(unlockableLists[2].Count > 0)
                {
                    secondaryRandom = Random.Range(0, unlockableLists[2].Count);

                    chosenItem = unlockableLists[2][secondaryRandom];
                    ins[i] = Instantiate(skinBit, Vector2.zero, skinBit.transform.rotation);
                    ins[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = chosenItem.color1;
                    ins[i].transform.GetChild(1).GetComponent<SpriteRenderer>().color = chosenItem.color2;

                    if (chosenItem.currentUnlockNumber + 1 == chosenItem.unlockNumber)
                    {
                        unlockableLists[2].Remove(chosenItem);
                        lockedLists[2].Remove(chosenItem);
                        unlockedLists[2].Add(chosenItem);
                        itemTexts[i + 3].text = trueTexts[2];
                        ES3.Save<bool>("newInventory", true);
                    }

                    else
                    {
                        itemToFind = lockedLists[2].Find(x => x.aminoNumber == chosenItem.aminoNumber);
                        itemToFind.currentUnlockNumber += 1;

                        if (chosenItem.currentUnlockNumber != 1)
                        {
                            textsToChange[i] = itemTexts[i + 3];
                            itemsToChange[i] = chosenItem;
                        }

                        else
                        {
                            itemTexts[i + 3].text = trueTexts[5];
                        }
                    }

                    switch (language)
                    {
                        case 0:
                            itemTexts[i].text = trueNames[chosenItem.aminoNumber - 1] + " BIT";

                            break;

                        case 1:
                            itemTexts[i].text = "TROZO DE " + trueNames[chosenItem.aminoNumber - 1];

                            break;
                    }
                }

                else
                {
                    AddCarbons(i);
                }
            }

            else if (initialRandom >= 99)
            {
                AddCarbons(i);
            }

            ins[i].GetComponent<Animator>().SetInteger("Pos", i);
        }

        for(int i = 0; i < textsToChange.Length; i++)
        {
            if(textsToChange[i] != null)
            {
                if(itemsToChange[i] != null)
                {
                    textsToChange[i].text = trueTexts[3] + (itemsToChange[i].unlockNumber - itemsToChange[i].currentUnlockNumber) + trueTexts[4];
                }

                else
                {
                    textsToChange[i].text = trueTexts[7] + carbons + "C";
                }
            }
        }

        ES3.Save<List<Item>[]>("unlockedLists", unlockedLists);
        ES3.Save<List<Item>[]>("lockedLists", lockedLists);
        ES3.Save<int>("carbons", carbons);

        yield return new WaitForSeconds(0.5f);

        itemTexts[0].transform.parent.gameObject.SetActive(true);
        oKButton.SetActive(true);
    }

    void AddCarbons(int i)
    {
        secondaryRandom = Random.Range(0, 4);

        if (secondaryRandom == 0)
        {
            ins[i] = Instantiate(butano, Vector2.zero, butano.transform.rotation);
            carbons += 4;
            itemTexts[i].text = "5 " + trueTexts[6];
        }

        else
        {
            ins[i] = Instantiate(singleCarbon, Vector2.zero, singleCarbon.transform.rotation);
            carbons += 1;
            itemTexts[i].text = "1 " + trueTexts[6];
        }

        textsToChange[i] = itemTexts[i + 3];
    }
}
