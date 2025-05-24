using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serpacid : Enemy
{
    [SerializeField]
    float rotSpeed;
    [SerializeField]
    float distAfterAttack;
    [SerializeField]
    float distToAttack;
    WaitForSeconds attackWait = new WaitForSeconds(0.25f);
    WaitForSeconds restWait = new WaitForSeconds(2);
    bool attacking;
    Vector2 direction;
    public bool onCollider;
    float angle;
    Coroutine attackCo;

    void FixedUpdate()
    {
        if(speed > 0)
        {
            oldSpeed = speed;
        }

        if (onCollider)
        {
            if(manager.outside)
            {
                speed = 0;
            }

            else 
            {
                speed = oldSpeed;
            }
        }

        if (!attacking && !stunned)
        {
            transform.position = Vector2.MoveTowards(new Vector3(transform.position.x, transform.position.y, zOffset), new Vector3(manager.ship.transform.position.x, manager.ship.transform.position.y, zOffset), speed);

            direction = transform.position - manager.ship.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle - 90), rotSpeed);

            if (Vector2.Distance(transform.position, manager.ship.position) <= distToAttack && speed > 0)
            {
                attacking = true;
                attackCo = StartCoroutine(AttackCo());
            }
        }
        
        else if(attacking && Vector2.Distance(transform.position, manager.ship.position) > distToAttack || attacking && stunned)
        {
            StopCoroutine(attackCo);
            anim.SetBool("Attack", false);
            attacking = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle")
        {
            onCollider = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            onCollider = false;
        }
    }

    IEnumerator AttackCo()
    {
        anim.SetBool("Attack", true);
        sound.Play();
        yield return attackWait;
        anim.SetBool("Attack", false);

        if(manager.currentHP > 0)
        {
            manager.HPManager(damage, this);
        }

        yield return restWait;
        attacking = false;
    }
}