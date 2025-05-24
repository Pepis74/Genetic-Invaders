using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    GameObject par;
    [SerializeField]
    float speed;
    [SerializeField]
    int damage;
    GameManager manager;
    WaitForSeconds waitForDestroy = new WaitForSeconds(1.5f);
    WaitForSeconds waitForParticle = new WaitForSeconds(1);
    SpriteRenderer sprite;
    GameObject ins;
    Rigidbody2D rig;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        manager = FindObjectOfType<GameManager>();
        rig = GetComponent<Rigidbody2D>();
        rig.velocity = new Vector2(0, -speed);
        StartCoroutine(DestroyCo());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ship")
        {
            if (manager.currentHP > 0)
            {
                manager.HPManager(damage, null);
            }

            ins = Instantiate(par, transform.position, par.transform.rotation);
            Destroy(sprite);
            StartCoroutine(DestroyParticleCo());
        }
    }

    IEnumerator DestroyCo()
    {
        yield return waitForDestroy;
        Destroy(gameObject);
    }

    IEnumerator DestroyParticleCo()
    {
        yield return waitForParticle;
        Destroy(ins);
        Destroy(gameObject);
    }
}
