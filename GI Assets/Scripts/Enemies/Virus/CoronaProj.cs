using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoronaProj : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    GameObject par;
    Vector2 direction;
    GameManager manager;
    GameObject ins;
    float angle;
    bool destroyed;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, manager.ship.transform.position, speed);

        direction = manager.ship.transform.position - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ship" && !destroyed)
        {
            destroyed = true;
            manager.StartCoroutine("CoronaCo");
            ins = Instantiate(par, transform.position, par.transform.rotation);
            Destroy(transform.GetChild(0).gameObject);
            StartCoroutine(DestroyCo());
        }
    }

    IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(ins);
        Destroy(gameObject);
    }
}
