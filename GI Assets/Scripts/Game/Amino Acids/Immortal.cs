using System.Collections;
using UnityEngine;

public class Immortal : MonoBehaviour
{
    GameManager manager;
    Warning audioS;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        audioS = FindObjectOfType<Warning>();
        StartCoroutine(DestroyCo());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Enemy>())
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if(enemy.currentHP - 10 <= 0)
            {
                audioS.GetComponent<AudioSource>().Play();
                manager.HPManager(-50, null);
                manager.able = true;
                manager.ship.GetComponentInChildren<Collider2D>().enabled = true;
                manager.ship.GetChild(3).gameObject.SetActive(false);
                Destroy(manager.insDeath);
                Destroy(gameObject);
                manager.StartCoroutine("ImmortalCo");
            }

            enemy.HPManager(10);
        }
    }

    IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(0.75f);

        manager.StartCoroutine("DeathCo");
        Destroy(gameObject);
    }
}
