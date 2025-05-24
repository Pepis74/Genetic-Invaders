using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProj : MonoBehaviour
{
    public float damage;
    public Enemy iD;
    [SerializeField]
    float speed;
    [SerializeField]
    GameObject par;
    [SerializeField]
    float offset;
    [SerializeField]
    float parDur;
    float rot;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rig;
    GameManager manager;
    GameObject ins;


    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        manager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rot = (transform.rotation.eulerAngles.z + 270) * Mathf.Deg2Rad;
        rig.velocity = new Vector2(Mathf.Cos(rot) * speed, Mathf.Sin(rot) * speed);
        StartCoroutine(DestroyCo());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ship")
        {
            if(manager.currentHP > 0)
            {
                manager.HPManager(damage, iD);
            }
            
            ins = Instantiate(par, new Vector2(transform.position.x, transform.position.y + offset), par.transform.rotation);
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
