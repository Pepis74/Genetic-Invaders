using System.Collections;
using UnityEngine;

public class HealingProj : MonoBehaviour
{
    public Transform target;
    Collider2D col;
    [SerializeField]
    int damage;
    [SerializeField]
    float speed;
    [SerializeField]
    GameObject par;
    [SerializeField]
    float parDur;
    SpriteRenderer spriteRenderer;
    Vector2 direction;
    float angle;
    GameObject ins;
    GameManager manager;

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
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed);

            direction = target.position - transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        else
        {
            if (ins != null)
            {
                Destroy(ins);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == target)
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if(enemy.letterChain.Count > 0)
            {
                if (enemy.currentHP < enemy.totalHP && enemy.letterChain.Count < 6)
                {
                    enemy.AddLetter();
                }

                enemy.HPManager(-damage);
            }

            ins = Instantiate(par, new Vector2(transform.position.x, transform.position.y), par.transform.rotation);
            Destroy(col);
            Destroy(spriteRenderer);
            Destroy(gameObject);
        }
    }
}
