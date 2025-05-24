using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(1);

    void Start()
    {
        if(!ES3.KeyExists("languageIndex"))
        {
            ES3.Save<int>("languageIndex", 0);

            if (Application.systemLanguage == SystemLanguage.Spanish || Application.systemLanguage == SystemLanguage.Basque || Application.systemLanguage == SystemLanguage.Catalan)
            {
                ES3.Save<int>("languageIndex", 1);
            }
        }

        StartCoroutine(LoadCo());
    }

    IEnumerator LoadCo()
    {
        yield return wait;

        if(ES3.KeyExists("highScore"))
        {
            SceneManager.LoadSceneAsync(1);
        }

        else
        {
            SceneManager.LoadSceneAsync(2);
        }
    }
}
