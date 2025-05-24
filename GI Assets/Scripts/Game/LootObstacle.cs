using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootObstacle : MonoBehaviour
{
    [SerializeField]
    GameObject par;
    [SerializeField]
    float speed;
    bool running;
    DeathManager dManager;
    Warning warn;
    WaitForSeconds waitForDestroy = new WaitForSeconds(1.5f);
    WaitForSeconds waitForParticle = new WaitForSeconds(1);
    SpriteRenderer sprite;
    GameObject ins;
    Rigidbody2D rig;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        warn = FindObjectOfType<Warning>();
        dManager = FindObjectOfType<DeathManager>();
        rig = GetComponent<Rigidbody2D>();
        rig.velocity = new Vector2(0, -speed);
        StartCoroutine(DestroyCo());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ship")
        {
            warn.lootboxNum += 1;

            running = true;
            StartCoroutine(WarnTextCo());

            ins = Instantiate(par, transform.position, par.transform.rotation);
            Destroy(sprite);
            StartCoroutine(DestroyParticleCo());
        }
    }

    IEnumerator DestroyCo()
    {
        yield return waitForDestroy;
        yield return new WaitUntil(() => !running);
        Destroy(gameObject);
    }

    IEnumerator DestroyParticleCo()
    {
        yield return waitForParticle;
        Destroy(ins);
        Destroy(gameObject);
    }

    IEnumerator WarnTextCo()
    {
        yield return new WaitUntil(() => !warn.running);

        warn.WarnText(4);

        running = false;
    }
}
