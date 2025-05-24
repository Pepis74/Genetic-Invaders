using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateralSprite : MonoBehaviour
{
    public bool first = true;
    GameManager manager;
    ColliderOnTouch col;
    Rigidbody2D rig;
    Coroutine stopVelocityCo;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        col = FindObjectOfType<ColliderOnTouch>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ship" && first)
        {
            rig = collision.GetComponentInParent<Rigidbody2D>();

            manager.able = false;

            if (collision.transform.position.x >= 0)
            {
                rig.AddForce(new Vector2(500, 0));
            }

            else
            {
                rig.AddForce(new Vector2(-500, 0));
            }

            if(stopVelocityCo != null)
            {
                StopCoroutine(stopVelocityCo);
            }

            stopVelocityCo = StartCoroutine(StopVelocityCo());
            col.force = true;
            first = false;
        }
    }

    IEnumerator StopVelocityCo()
    {
        yield return new WaitForSeconds(0.25f);
        rig.velocity = Vector2.zero;
        manager.able = true;
        col.force = false;
    }
}
