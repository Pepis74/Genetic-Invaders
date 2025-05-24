using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dasher : Projectile
{
    Collider2D col;
    [SerializeField]
    float dur;
    [SerializeField]
    float speed;
    [SerializeField]
    int damage;
    [SerializeField]
    GameObject par;
    [SerializeField]
    float parDur;
    SpriteRenderer spriteRenderer;
    ColliderOnTouch colManager;
    GameObject ins;
    Vector2 direction;
    float angle;
    bool wasBoosting;

    void Start()
    {
        col = GetComponent<Collider2D>();
        colManager = FindObjectOfType<ColliderOnTouch>();
        manager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(colManager.boosting)
        {
            wasBoosting = true;
        }

        else
        {
            wasBoosting = false;
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed);

            direction = target.transform.position - transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            startEnded = true;
        }

        else if (manager.enemies.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>() == target)
        {
            Enemy target = other.GetComponent<Enemy>();
            target.HPManager(damage);
            target.statusWait = dur;

            if (wasBoosting)
            {
                target.StartCoroutine("StunCo");
            }

            ins = Instantiate(par, new Vector2(transform.position.x, transform.position.y), par.transform.rotation);
            Destroy(gameObject);
        }
    }
}
