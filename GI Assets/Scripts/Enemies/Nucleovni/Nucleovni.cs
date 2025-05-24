using System.Collections;
using UnityEngine;

public class Nucleovni : Enemy
{
    [SerializeField]
    GameObject projPar;
    [SerializeField]
    Transform spawn;
    [SerializeField]
    GameObject proj;
    GameObject insPart;
    WaitForSeconds w0dot5 = new WaitForSeconds(0.5f);
    WaitForSeconds w1dot1 = new WaitForSeconds(1.1f);
    WaitForSeconds w0dot15 = new WaitForSeconds(0.15f);
    WaitForSeconds w4 = new WaitForSeconds(4);
    Vector2 direction;
    Rigidbody2D rig;
    Vector2 defaultPos;
    Transform gun;
    bool stopRot;
    float angle;
    bool attack;
    bool wasStunned;
    bool attackAfterStun;
    bool start;
    Coroutine randomCo = null;
    Coroutine attackCo = null;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        gun = transform.GetChild(0);
    }

    void LateUpdate()
    {
        if(!stopRot && !stunned)
        {
            direction = gun.position - manager.ship.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gun.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
       
        if(start)
        {
            transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, zOffset), new Vector3(defaultPos.x, defaultPos.y, zOffset), speed * Time.deltaTime * 1.25f);

            if(new Vector2(transform.position.x, transform.position.y) == defaultPos && !stunned)
            {
                randomCo = StartCoroutine(RandomCo());
                attackCo = StartCoroutine(AttackCo());
                start = false;
            }
        }

        if(stunned && !wasStunned)
        {
            if(randomCo != null)
            {
                StopCoroutine(randomCo);
            }

            if (attackCo != null)
            {
                StopCoroutine(attackCo);
            }
            
            anim.SetBool("Charge", false);
            stopRot = false;
            attack = false;

            if (insPart != null)
            {
                Destroy(insPart);
            }

            attackAfterStun = true;
            wasStunned = true;
        }

        else if(!stunned && wasStunned)
        {
            randomCo = StartCoroutine(RandomCo());
            attackCo = StartCoroutine(AttackCo());
            wasStunned = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle")
        {
            Return();
        }
    }

    public void Return()
    {
        defaultPos = new Vector2(Random.Range(-1, 1), Random.Range(0.75f, 1.25f));

        if(randomCo != null)
        {
            StopCoroutine(randomCo);
            StopCoroutine(attackCo);
        }
        
        anim.SetBool("Charge", false);
        stopRot = false;
        attack = false;

        if (insPart != null)
        {
            Destroy(insPart);
        }

        rig.velocity = Vector2.zero;
        start = true;
    }

    IEnumerator RandomCo()
    {
        while(!attack)
        {
            rig.velocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed));

            yield return w0dot5;
        }
    }

    IEnumerator AttackCo()
    {
        while(true)
        {
            if(attackAfterStun)
            {
                yield return w4;
                attackAfterStun = false;
            }

            attack = true;
            rig.velocity = Vector2.zero;
            sound.Play();
            anim.SetBool("Charge", true);
            insPart = Instantiate(projPar, spawn.position, projPar.transform.rotation);
            insPart.transform.SetParent(gun);

            yield return w1dot1;

            stopRot = true;

            yield return w0dot15;

            anim.SetBool("Charge", false);
            Destroy(insPart);
            insProj = Instantiate(proj, spawn.position, gun.rotation);
            insProj.GetComponent<EnemyProj>().damage = damage;
            insProj.GetComponent<EnemyProj>().iD = this;
            attack = false;
            StartCoroutine(RandomCo());
            stopRot = false;
            yield return w4;
        }
    }
}
