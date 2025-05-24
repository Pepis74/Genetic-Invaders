using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Projectile
{
    Collider2D col;
    [SerializeField]
    int damage;
    [SerializeField]
    GameObject par;
    SpriteRenderer spriteRenderer;
    Vector2 direction;
    float angle;
    List<GameObject> ins = new List<GameObject>();
    WaitForSeconds w0dot2 = new WaitForSeconds(0.2f);

    void Start()
    {
        col = GetComponent<Collider2D>();
        manager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(DestroyCo());
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>())
        {
            other.GetComponent<Enemy>().HPManager(damage);
            ins.Add(Instantiate(par, new Vector2(other.transform.position.x, other.transform.position.y), par.transform.rotation));
        }
    }

    IEnumerator DestroyCo()
    {
        yield return w0dot2;
        Destroy(col);
        yield return w0dot2;
        Destroy(gameObject);
    }
}
