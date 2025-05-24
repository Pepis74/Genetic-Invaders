using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [SerializeField]
    Image hScoreUI;
    [SerializeField]
    Image scoreUI;
    [SerializeField]
    Sprite hSIcon;
    [SerializeField]
    Sprite scoreIcon;
    [SerializeField]
    GameObject[] toDestroyLoad = new GameObject[4];
    [SerializeField]
    GameObject loading;
    [SerializeField]
    Text scoreTxt;
    [SerializeField]
    Text highscoreTxt;
    GameManager manager;
    AssignmentManager assignment;
    Warning warn;

    void Awake()
    {
        manager = FindObjectOfType<GameManager>();
        assignment = FindObjectOfType<AssignmentManager>();
        warn = FindObjectOfType<Warning>();

        if (manager.score > warn.highScore)
        {
            warn.highScore = manager.score;
            ES3.Save<int>("highScore", warn.highScore);

            scoreUI.sprite = hSIcon;
            scoreUI.rectTransform.localScale = new Vector2(1, 1.156f);
            scoreTxt.text = warn.highScore + "!";
            hScoreUI.gameObject.SetActive(false);
            
        }

        else if(manager.score <= warn.highScore)
        {
            scoreTxt.text =  "" + manager.score;
            highscoreTxt.text = "" + warn.highScore;
        }
    }

    public void Retry()
    {
        Load(2);
    }

    public void Back()
    {
        Load(1);
    }
    
    void Load(int sceneIndex)
    {
        ES3.Save<int>("highScore", warn.highScore);
        ES3.Save<int>("lootboxNum", warn.lootboxNum);

        if(assignment.dailyAssignment != null && !assignment.dailyAssignment.singleGame)
        {
            ES3.Save<int>("kills", assignment.kills);
        }
        
        ES3.Save<bool>("completed", assignment.completed);

        for (int i = 0; i < toDestroyLoad.Length; i++)
        {
            Destroy(toDestroyLoad[i]);
        }

        loading.SetActive(true);
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
