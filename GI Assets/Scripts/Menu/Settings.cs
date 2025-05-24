using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [SerializeField]
    Dropdown dropdown;
    [SerializeField]
    Text[] texts;
    [SerializeField]
    string[] menuTexts;
    [SerializeField]
    string[] menuTextsES;
    [SerializeField]
    Slider musicSlider;
    [SerializeField]
    Slider effectsSlider;
    [SerializeField]
    Toggle toggle;
    [SerializeField]
    AudioMixer main;
    int language;
    MenuManager manager;
    AssignmentMenu assignment;
    InventoryManager inventory;
    bool start = true;
    float savedVol;

    void Start()
    {
        SetLanguage();

        manager = FindObjectOfType<MenuManager>();
        assignment = FindObjectOfType<AssignmentMenu>();
        inventory = FindObjectOfType<InventoryManager>();

        if(ES3.KeyExists("musicVol"))
        {
            savedVol = ES3.Load<float>("musicVol");
            musicSlider.value = savedVol;
            main.SetFloat("musicVol", Mathf.Log10(savedVol) * 20);
        }

        else
        {
            musicSlider.value = 1;
        }

        if (ES3.KeyExists("effectsVol"))
        {
            savedVol = ES3.Load<float>("effectsVol");
            effectsSlider.value = savedVol;
            main.SetFloat("effectsVol", Mathf.Log10(savedVol) * 20);
        }

        else
        {
            effectsSlider.value = 1;
        }

        if (ES3.KeyExists("languageIndex"))
        {
            dropdown.value = ES3.Load<int>("languageIndex");
        }

        if(ES3.KeyExists("tutorial"))
        {
            toggle.isOn = ES3.Load<bool>("tutorial");
        }

        start = false;
    }

    public void SetTutorial(bool checkbox)
    {
        ES3.Save<bool>("tutorial", checkbox);
        manager.PlaySound();
    }

    public void SetLanguage(int index)
    {
        if(!start)
        {
            ES3.Save<int>("languageIndex", index);
            manager.PlaySound();

            SetLanguage();
            manager.SetLanguage();
            assignment.SetLanguage();
            inventory.SetLanguage();

            inventory.InventoryUpdate();
            assignment.UpdateAssignment(false);
        }
    }

    public void SetMusicVolume(float volume)
    {
        main.SetFloat("musicVol", Mathf.Log10(volume) * 20);
        ES3.Save<float>("musicVol", volume);
    }

    public void SetEffectsVolume(float volume)
    {
        main.SetFloat("effectsVol", Mathf.Log10(volume) * 20);
        ES3.Save<float>("effectsVol", volume);
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public void SetLanguage()
    {
        language = ES3.Load<int>("languageIndex");

        switch(language)
        {
            case 0:

                for(int i = 0; i < texts.Length; i++)
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
}
