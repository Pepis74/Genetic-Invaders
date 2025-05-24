using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    MenuManager manager;

    void Start()
    {
        manager = FindObjectOfType<MenuManager>();
    }

    public void Play()
    {
        manager.PlaySound();
    }
}
