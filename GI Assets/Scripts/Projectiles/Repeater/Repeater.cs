using System.Collections;
using UnityEngine;

public class Repeater : Projectile
{
    Collider2D col;
    [SerializeField]
    GameObject secondProj;
    [SerializeField]
    float speed;
    [SerializeField]
    int damage;
    [SerializeField]
    GameObject par;
    [SerializeField]
    float parDur;
    float dist;
    float minimumDist;
    SpriteRenderer spriteRenderer;
    Enemy closestEnemy;
    GameObject ins;
    Vector2 direction;
    float angle;

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
            if (manager.enemies.Count > 1)
            {
                minimumDist = float.MaxValue;

                for (int i = 0; i < manager.enemies.Count; i++)
                {
                    dist = Vector2.Distance(transform.position, manager.enemies[i].transform.position);

                    if (dist < minimumDist && manager.enemies[i] != target)
                    {
                        minimumDist = dist;
                        closestEnemy = manager.enemies[i];
                    }
                }

                ins = Instantiate(secondProj, transform.position, transform.rotation);
                ins.GetComponent<Projectile>().target = closestEnemy;
                Destroy(gameObject);
            }

            else
            {
                ins = Instantiate(par, new Vector2(transform.position.x, transform.position.y), par.transform.rotation);
                Destroy(gameObject);
            }

            other.GetComponent<Enemy>().HPManager(damage);
        }
    }
}
