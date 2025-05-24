using System.Collections;
using UnityEngine;

public class Gennon : Enemy
{
    [SerializeField]
    float rotSpeed;
    [SerializeField]
    AudioSource chargeSound;
    [SerializeField]
    GameObject gene;
    [SerializeField]
    GameObject beam;
    [SerializeField]
    SpriteRenderer thirdEye;
    [SerializeField]
    Sprite open;
    [SerializeField]
    GameObject eye;
    Vector2 direction;
    Sprite oldSprite;
    float angle;
    bool start;
    bool look;
    bool wasStunned;
    Coroutine attackCo;

    void Awake()
    {
        start = true;
        look = true;
        manager = FindObjectOfType<GameManager>();
        thirdEye.sprite = oldSprite;
        Shield();
    }

    void FixedUpdate()
    {
        if(start)
        {
            transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, zOffset), new Vector3(transform.position.x, -1, zOffset), speed);

            if(transform.position.y == -1)
            {
                start = false;
                attackCo = StartCoroutine(AttackCo());
            }
        }

        else if(look && !start && !stunned)
        {
            direction = transform.position - manager.ship.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle - 90), rotSpeed);
        }

        if(stunned && !wasStunned)
        {
            StopCoroutine(attackCo);
            anim.SetBool("OpenEye", false);
            beam.SetActive(false);
            eye.SetActive(false);
            gene.SetActive(true);
            thirdEye.sprite = oldSprite;
            look = true;
            wasStunned = true;
        }

        else if(!stunned && wasStunned)
        {
            attackCo = StartCoroutine(AttackCo());
            wasStunned = false;
        }
    }

    IEnumerator AttackCo()
    {
        if(shield)
        {
            DestroyShield();
        }
        
        anim.SetBool("OpenEye", true);
        thirdEye.sprite = open;
        chargeSound.Play();

        yield return new WaitForSeconds(0.15f);

        look = false;

        yield return new WaitForSeconds(0.5f);

        eye.SetActive(true);
        gene.SetActive(false);
        anim.SetBool("OpenEye", false);

        yield return new WaitForSeconds(0.25f);
        beam.SetActive(true);
        beam.GetComponent<Beam>().iD = this;
    }
}
