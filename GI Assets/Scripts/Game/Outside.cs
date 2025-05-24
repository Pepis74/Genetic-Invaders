using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outside : MonoBehaviour
{
    [SerializeField]
    GameObject warningL;
    [SerializeField]
    GameObject warningR;
    [SerializeField]
    int damage;
    [SerializeField]
    bool left;
    bool wasBoosting;
    BoxCollider2D col;
    GameManager manager;
    AAManager aminos;
    ColliderOnTouch colManager;
    WaitForSeconds wait = new WaitForSeconds(1.5f);

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        colManager = FindObjectOfType<ColliderOnTouch>();
        manager = FindObjectOfType<GameManager>();
        aminos = FindObjectOfType<AAManager>();
    }

    void Update()
    {
        if (wasBoosting && !colManager.boosting)
        {
            manager.rig.velocity *= 2;
            wasBoosting = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ship")
        {
            manager.outside = true;
            
            StartCoroutine(DamageCo());

            if(left)
            {
                warningL.SetActive(true);
            }

            else
            {
                warningR.SetActive(true);
            }

            if(colManager.boosting)
            {
                manager.rig.velocity /= 2;
                wasBoosting = true;
            }

            if(aminos.selectedAmino == 3)
            {
                Time.timeScale = 0.5f;
                colManager.touchWait *= 0.5f;
                colManager.doubleTapWait *= 0.5f;
                colManager.speed /= 0.25f;
            }
        }

        else if(other.GetComponent<Nucleovni>())
        {
            other.GetComponent<Nucleovni>().Return();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ship")
        {
            StopAllCoroutines();
            warningL.SetActive(false);
            warningR.SetActive(false);
            manager.outside = false;

            if (aminos.selectedAmino == 3)
            {
                Time.timeScale = 1;
                colManager.touchWait /= 0.5f;
                colManager.doubleTapWait /= 0.5f;
                colManager.speed *= 0.25f;
            }
        }
    }

    IEnumerator DamageCo()
    {
        while(true)
        {
            yield return wait;

            if(manager.currentHP > 0)
            {
                manager.HPManager(damage, null);
            }
        }
    }
}
