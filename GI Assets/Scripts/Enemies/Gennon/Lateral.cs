using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lateral : Enemy
{
    [SerializeField]
    AudioSource chargeSound;
    [SerializeField]
    GameObject leftBeam;
    [SerializeField]
    GameObject rightBeam;
    [SerializeField]
    GameObject topPropellers;
    [SerializeField]
    GameObject bottomPropellers;
    [SerializeField]
    float fastMultiplier;
    Vector2 direction;
    Vector2 toGoTo;
    Sprite oldSprite;
    int randomInt;
    float angle;
    bool attack;
    bool wasStunned;
    bool attacking;
    LateralSprite latSprite;
    Coroutine attackCo;

    void Awake()
    {
        manager = FindObjectOfType<GameManager>();
        toGoTo = Vector2.zero;
        latSprite = GetComponentInChildren<LateralSprite>();
    }

    void FixedUpdate()
    {
        if (toGoTo != Vector2.one && !stunned)
        {
            transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, zOffset), new Vector3(toGoTo.x, toGoTo.y, zOffset), speed);

            if (new Vector2(transform.position.x, transform.position.y) == toGoTo)
            {
                toGoTo = Vector2.one;
                topPropellers.SetActive(false);
                bottomPropellers.SetActive(false);

                if (attack)
                {
                    attack = false;
                    latSprite.first = true;
                    anim.SetBool("Back", true);
                    leftBeam.SetActive(false);
                    rightBeam.SetActive(false);
                    bottomPropellers.SetActive(true);
                    speed /= fastMultiplier;
                    toGoTo = Vector2.zero;
                }

                else
                {
                    attackCo = StartCoroutine(AttackCo());
                }
            }
        }

        if (stunned && !wasStunned)
        {
            if(attackCo != null)
            {
                StopCoroutine(attackCo);
            }

            anim.SetBool("Left", false);
            anim.SetBool("Right", false);
            anim.SetBool("Back", false);
            leftBeam.SetActive(false);
            rightBeam.SetActive(false);
            topPropellers.SetActive(false);
            bottomPropellers.SetActive(false);
            wasStunned = true;
        }

        else if (!stunned && wasStunned)
        {
            if(attack && attacking)
            {
                attackCo = StartCoroutine(AttackCo());
            }

            else if(attack && !attacking)
            {
                if (randomInt == 0)
                {
                    leftBeam.SetActive(true);
                }

                else
                {
                    rightBeam.SetActive(true);
                }

                topPropellers.SetActive(true);
            }

            else if (!attack)
            {
                bottomPropellers.SetActive(true);
            }

            transform.rotation = Quaternion.Euler(0, 0, 0);

            wasStunned = false;
        }
    }

    IEnumerator AttackCo()
    {
        attacking = true;
        attack = true;
        randomInt = Random.Range(0, 2);

        anim.SetBool("Back", false);

        if (randomInt == 0)
        {
            anim.SetBool("Left", true);
        }

        else
        {
            anim.SetBool("Right", true);
        }

        chargeSound.Play();

        yield return new WaitForSeconds(25/24);
        anim.SetBool("Left", false);
        anim.SetBool("Right", false);

        if (randomInt == 0)
        {
            leftBeam.SetActive(true);
        }

        else
        {
            rightBeam.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        speed *= fastMultiplier;
        topPropellers.SetActive(true);       
        toGoTo = new Vector2(0, -4);
        attacking = false;
    }
}
