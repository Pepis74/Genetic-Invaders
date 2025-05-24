using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    public int lootboxNum;
    public int highScore;
    public bool running;
    [SerializeField]
    string[] texts;
    [SerializeField]
    string[] textsES;
    [SerializeField]
    Text textBox;
    [SerializeField]
    int[] progressBar;
    WaitForSeconds w3 = new WaitForSeconds(3);
    DeathManager dManager;
    GameManager gManager;
    float tempHighscore;
    string[] trueTexts;
    bool first;
    int language;

    void Start()
    {
        language = ES3.Load<int>("languageIndex");

        switch(language)
        {
            case 0:
                trueTexts = texts;

                break;

            case 1:
                trueTexts = textsES;

                break;
        }

        dManager = FindObjectOfType<DeathManager>();
        gManager = FindObjectOfType<GameManager>();

        if (ES3.KeyExists("highScore"))
        {
            highScore = ES3.Load<int>("highScore");
            tempHighscore = highScore;
        }

        else
        {
            highScore = 0;
        }

        if (ES3.KeyExists("lootboxNum"))
        {
            lootboxNum = ES3.Load<int>("lootboxNum");
        }

        else
        {
            lootboxNum = 0;
        }

        first = true;
    }

    void Update()
    {
        for(int i = 0; i < progressBar.Length; i++)
        {
            if (gManager.score >= progressBar[i] && tempHighscore < progressBar[i])
            {
                StartCoroutine(WarnTextCo(0));
                ES3.Save<bool>("newAssignments", true);
                tempHighscore = gManager.score;
            }
        }

        if (gManager.score > highScore && highScore > 0 && first)
        {
            StartCoroutine(WarnTextCo(1));
            first = false;
        }
    }

    public void WarnText(int toSay)
    {
        running = true;
        textBox.text = trueTexts[toSay];
        textBox.transform.parent.GetComponent<Animator>().SetBool("Start", true);
        StartCoroutine(TextBoxCo());
    }

    IEnumerator TextBoxCo()
    {
        yield return w3;
        textBox.transform.parent.GetComponent<Animator>().SetBool("Start", false);
        running = false;
    }

    IEnumerator WarnTextCo(int toSay)
    {
        yield return new WaitUntil(() => !running);
        WarnText(toSay);
    }
}
