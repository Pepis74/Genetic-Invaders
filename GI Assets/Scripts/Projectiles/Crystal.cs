using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Projectile
{
    Collider2D col;
    [SerializeField]
    float multiplier;
    [SerializeField]
    float dur;
    [SerializeField]
    Color color;
    [SerializeField]
    float speed;
    [SerializeField]
    int damage;
    [SerializeField]
    GameObject par;
    [SerializeField]
    float parDur;
    SpriteRenderer spriteRenderer;
    Vector2 direction;
    float angle;
    GameObject ins;

    void Start()
    {
        col = GetComponent<Collider2D>();
        manager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            target.statusColor = color;
            target.statusWait = dur;
            target.ElementalCoroutines(1);
            target.multiplier = multiplier;
            ins = Instantiate(par, new Vector2(transform.position.x, transform.position.y), par.transform.rotation);
            Destroy(gameObject);
        }
    }
}
