using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public bool touchAbleB;
    public bool touchAbleC;
    public GameObject newInventoryImg;
    public int carbons;
    public AudioSource errorSound;
    [SerializeField]
    Color transparentColor;
    [SerializeField]
    Text[] texts;
    [SerializeField]
    string[] menuTexts;
    [SerializeField]
    string[] menuTextsES;
    [SerializeField]
    Transform settingsMenu;
    [SerializeField]
    GameObject warnText;
    [SerializeField]
    bool active;
    [SerializeField]
    Text carbonText;
    [SerializeField]
    GameObject[] toDeactivate = new GameObject[3];
    [SerializeField]
    Text highScoreText;
    [SerializeField]
    GameObject playButton;
    float canvasSpeed;
    [SerializeField]
    float backSpeed;
    [SerializeField]
    GameObject creditsMenu;
    [SerializeField]
    GameObject loading;
    [SerializeField]
    Animator anim;
    [SerializeField]
    Transform[] canvasToMove = new Transform[3];
    [SerializeField]
    Transform[] backToMove = new Transform[3];
    [SerializeField]
    AudioSource buttonSound;
    [SerializeField]
    GameObject newAssignmentsImg;
    WaitForSeconds playWait = new WaitForSeconds(0.66f);
    WaitForSeconds touchWait = new WaitForSeconds(0.1f);
    WaitForSeconds swipeWait = new WaitForSeconds(0.3f);
    AsyncOperation operation;
    Vector2[] oldCanvasPos = new Vector2[3];
    Vector2[] oldBackPos = new Vector2[3];
    float canvasToMoveTo;
    float backToMoveTo;
    Vector2 oldTouchPos;
    Vector2 touchPos;
    Coroutine warnTextCo;
    int screen;
    int language;
    bool touchAbleA = true;

    void Start()
    {
        if(active)
        {
            SetLanguage();

            if (ES3.KeyExists("carbons"))
            {
                carbons = ES3.Load<int>("carbons");
            }

            else
            {
                carbons = 0;
                ES3.Save<int>("carbons", carbons);
            }

            if (active && ES3.KeyExists("highScore"))
            {
                highScoreText.text = "" + ES3.Load<int>("highScore");
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

            if (ES3.KeyExists("newInventory") && ES3.Load<bool>("newInventory") == true)
            {
                newInventoryImg.SetActive(true);
            }

            if (ES3.KeyExists("newAssignments") && ES3.Load<bool>("newAssignments") == true)
            {
                newAssignmentsImg.SetActive(true);
            }

            if (carbonText != null)
            {
                CarbonsUpdate();
            }
        }
    }

    void FixedUpdate()
    {
        if (active)
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

                for (int i = 0; i < backToMove.Length; i++)
                {
                    backToMove[i].position = Vector2.MoveTowards(backToMove[i].position, new Vector2(oldBackPos[i].x + backToMoveTo, 0), backSpeed * 2);
                }
            }
        }
    }

    public void Play()
    {
        for (int i = 0; i < toDeactivate.Length; i++)
        {
            toDeactivate[i].SetActive(false);
        }

        touchAbleB = false;
        loading.SetActive(true);
        operation = SceneManager.LoadSceneAsync(2);
        operation.allowSceneActivation = false;
        anim.SetBool("Start", true);
        StartCoroutine(PlayCo());
    }

    public void PlaySound()
    {
        buttonSound.Play();
    }

    public void CarbonsUpdate()
    {
        carbonText.text = "" + carbons;
    }

    public void WarnText(string toSay)
    {
        if(warnTextCo != null)
        {
            StopCoroutine(warnTextCo);
        }

        warnText.GetComponent<Animator>().SetBool("Fade", false);
        warnText.GetComponentInChildren<Text>().text = toSay;
        warnText.SetActive(true);
        warnText.GetComponent<Animator>().SetBool("Fade", true);
        warnTextCo = StartCoroutine(WarnTextCo());
    }

    public void ToLeft()
    {
        if(touchAbleA)
        {
            touchAbleA = false;

            for (int i = 0; i < oldCanvasPos.Length; i++)
            {
                oldCanvasPos[i] = canvasToMove[i].position;
                oldBackPos[i] = backToMove[i].position;
            }

            backToMoveTo = 5.625f;
            canvasToMoveTo = Screen.width;

            screen += 1;

            StartCoroutine(StopScreen());
        }
    }

    public void ToRight()
    {
        if(touchAbleA)
        {
            touchAbleA = false; 

            for (int i = 0; i < oldCanvasPos.Length; i++)
            {
                oldCanvasPos[i] = canvasToMove[i].position;
                oldBackPos[i] = backToMove[i].position;
            }

            if (screen == 0)
            {
                ES3.Save<bool>("newAssignments", false);
                newAssignmentsImg.SetActive(false);
            }

            backToMoveTo = -5.625f;
            canvasToMoveTo = -Screen.width;
            screen -= 1;

            StartCoroutine(StopScreen());
        }
    }

    public void CloseSettings()
    {
        touchAbleB = true;

        for (int i = 0; i < settingsMenu.childCount; i++)
        {
            settingsMenu.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void OpenSettings()
    {
        touchAbleB = false;

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < settingsMenu.childCount; i++)
        {
            settingsMenu.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void CloseCredits()
    {
        touchAbleB = true;

        creditsMenu.SetActive(false);

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void OpenCredits()
    {
        touchAbleB = false;

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        creditsMenu.SetActive(true);
    }

    public void SetLanguage()
    {
        language = ES3.Load<int>("languageIndex");

        switch (language)
        {
            case 0:

                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = menuTexts[i];
                }

                break;

            case 1:

                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = menuTextsES[i];
                }

                break;
        }
    }

    IEnumerator PlayCo()
    {
        yield return playWait;
        anim.SetBool("Start", false);
        Destroy(anim.gameObject);
        operation.allowSceneActivation = true;
    }

    IEnumerator WarnTextCo()
    {
        yield return new WaitForSeconds(2.49f);
        warnText.GetComponent<Animator>().SetBool("Fade", false);
        warnText.SetActive(false);
    }

    IEnumerator TouchWaitCo()
    {
        yield return touchWait;
        
        if(Input.touchCount > 0)
        {
            touchPos = Input.touches[0].position;
            
            if(Vector2.Distance(new Vector2(touchPos.x, 0), new Vector2(oldTouchPos.x, 0)) > 75)
            {
                touchAbleA = false;

                for (int i = 0; i < oldCanvasPos.Length; i++)
                {
                    oldCanvasPos[i] = canvasToMove[i].position;
                    oldBackPos[i] = backToMove[i].position;
                }

                if (touchPos.x > oldTouchPos.x && screen < 1)
                {
                    backToMoveTo = 5.625f;
                    canvasToMoveTo = Screen.width;
                    screen += 1;
                }

                else if (touchPos.x < oldTouchPos.x && screen > -1)
                {
                    backToMoveTo = -5.625f;
                    canvasToMoveTo = -Screen.width;

                    if (screen == 0)
                    {
                        ES3.Save<bool>("newAssignments", false);
                        newAssignmentsImg.SetActive(false);
                    }

                    screen -= 1;
                }

                yield return swipeWait;

                canvasToMoveTo = 0;
                touchAbleA = true;
               
            }
        }

        touchAbleC = true;
    }

    IEnumerator StopScreen()
    {
        yield return swipeWait;
        canvasToMoveTo = 0;
        backToMoveTo = 0;
        touchAbleA = true;
        touchAbleC = true;
    }
}
