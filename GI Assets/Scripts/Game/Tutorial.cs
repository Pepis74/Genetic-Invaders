using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public bool dead;
    public bool tutorialActive;
    [SerializeField]
    GameObject[] tutorialObjects = new GameObject[5];
    [SerializeField]
    GameObject OkButton;
    [SerializeField]
    Text textBox;
    [TextArea(3, 10)]
    [SerializeField]
    string[] sentences;
    [SerializeField]
    string[] sentencesES;
    [SerializeField]
    Enemy practice;
    AudioSource sound;
    WaitForSeconds w2dot5 = new WaitForSeconds(2.5f);
    WaitForSeconds w0dot02 = new WaitForSeconds(0.02f);
    WaitForSeconds w5 = new WaitForSeconds(5);
    WaitForSeconds w12 = new WaitForSeconds(12);
    WaitForSeconds w7dot5 = new WaitForSeconds(7.5f);
    WaitForSeconds w10 = new WaitForSeconds(10);
    char[] charArray;
    string[] trueSentences = new string[13];
    int count;
    int language;
    GameManager manager;
    int a;

    void Start()
    {
        language = ES3.Load<int>("languageIndex");

        switch (language)
        {
            case 0:
                trueSentences = sentences;

                break;

            case 1:
                trueSentences = sentencesES;

                break;
        }

        if (!ES3.KeyExists("tutorial") || ES3.Load<bool>("tutorial"))
        {
            StartCoroutine(TutorialCo());
            sound = GetComponent<AudioSource>();
            manager = FindObjectOfType<GameManager>();
        }

        else
        {
            Destroy(gameObject);
        }

        /*if (tutorialActive)
        {
            StartCoroutine(Tutorial1Co());
            sound = GetComponent<AudioSource>();
            manager = FindObjectOfType<GameManager>();
            ES3.Save<bool>("tutorial", false);
        }

        else
        {
            Destroy(gameObject);
        }*/
    }

    void Update()
    {
        if (dead)
        {
            StartCoroutine(WriteCo());
            dead = false;
        }
    }

    public void TutorialButton()
    {
        OkButton.SetActive(false);
        StartCoroutine(WriteCo());

        switch (count)
        {
            case 1:
                tutorialObjects[3].SetActive(true);
                break;

            case 2:
                tutorialObjects[3].SetActive(false);
                tutorialObjects[0].SetActive(true);
                break;

            case 3:
                tutorialObjects[0].SetActive(false);
                tutorialObjects[1].SetActive(true);
                break;

            case 4:
                tutorialObjects[1].SetActive(false);
                tutorialObjects[2].SetActive(true);
                break;

            case 5:
                tutorialObjects[2].SetActive(false);
                tutorialObjects[4].SetActive(true);
                break;

            case 6:
                tutorialObjects[4].SetActive(false);
                practice.transform.GetComponent<Animator>().SetBool("Start", true);
                break;
        }
    }

    IEnumerator TutorialCo()
    {
        yield return w2dot5;
        textBox.transform.GetComponentInParent<Animator>().SetBool("Start", true);
        StartCoroutine(WriteCo());
    }

    IEnumerator WriteCo()
    {
        textBox.text = "";
        charArray = trueSentences[count].ToCharArray();

        for (int i = 0; i < charArray.Length; i++)
        {
            textBox.text += charArray[i];

            if (a == 2)
            {
                sound.Play();
                a = 0;
            }

            else
            {
                a += 1;
            }

            yield return w0dot02;
        }

        count += 1;

        if (count == 10)
        {
            manager.enemies.Add(practice);
        }

        else if (count == 13)
        {
            manager.HPManager(-float.MaxValue, null);
            manager.EnergyManager(-int.MaxValue);
            manager.StartCoroutine("ScoreCo");
            manager.StartCoroutine("EnemySpawnCo");
            manager.StartCoroutine("ObsSpawnCo");
            ES3.Save<bool>("tutorial", false);
            textBox.transform.parent.GetComponent<Animator>().SetBool("Start", false);
            yield return new WaitForSeconds(0.74f);
            Destroy(gameObject);
        }

        else
        {
            OkButton.SetActive(true);
        }
    }
}
