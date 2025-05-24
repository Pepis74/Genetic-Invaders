using System.Collections;
using UnityEngine;

public class Healing : Enemy
{
    [SerializeField]
    GameObject sleepyPar;
    [SerializeField]
    Transform spawn;
    [SerializeField]
    GameObject proj;
    public Transform target;
    WaitForSeconds w0dot5 = new WaitForSeconds(0.5f);
    WaitForSeconds w2 = new WaitForSeconds(2);
    WaitForSeconds w0dot44 = new WaitForSeconds(0.44f);
    Vector2 direction;
    Rigidbody2D rig;
    Vector2 defaultPos;
    Transform gun;
    float minimumHP;
    public float enemyHP;
    float angle;
    bool start;
    bool return_;
    public bool rotate;
    bool first1 = true;
    bool first2 = true;
    bool crazy;
    int increasingRot;
    Coroutine attackCo;
    Coroutine randomCo;


    void Awake()
    {
        manager = FindObjectOfType<GameManager>();
        rig = GetComponent<Rigidbody2D>();
        gun = transform.GetChild(0);
        Shield();
    }

    void LateUpdate()
    {
        if(manager.enemies.Count > 0 && first1)
        {
            sleepyPar.SetActive(false);
            attackCo = StartCoroutine(AttackCo());
            randomCo = StartCoroutine(RandomCo());
            first1 = false;
        }

        else if(manager.enemies.Count == 0 && !sleepyPar.activeSelf && first2)
        {
            DestroyShield();
            StopCoroutine(attackCo);
            StopCoroutine(randomCo);
            return_ = false;
            rotate = false;
            rig.velocity = Vector2.zero;
            anim.SetBool("Run", true);
            StartCoroutine(RunCo());
            first2 = false;
        }
    }

    void FixedUpdate()
    {
        if(return_)
        {
            transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, zOffset), new Vector3(defaultPos.x, defaultPos.y, zOffset), speed * 0.02f);

            if (new Vector2(transform.position.x, transform.position.y) == defaultPos)
            {
                return_ = false;

                randomCo = StartCoroutine(RandomCo());
            }
        }

        if(rotate)
        {
            if(target != null)
            {
                direction = gun.position - target.position;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                gun.rotation = Quaternion.RotateTowards(gun.rotation, Quaternion.Euler(0, 0, angle - 90), 4.5f);

                if (gun.rotation == Quaternion.Euler(0, 0, angle - 90))
                {
                    rotate = false;
                }
            }

            else
            {
                rotate = false;
            }
        }

        else if(crazy && !stunned)
        {
            increasingRot += 5;
            gun.rotation = Quaternion.RotateTowards(gun.rotation, Quaternion.Euler(0, 0, increasingRot), 5);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            Return();
        }
    }

    public void Return()
    {
        if(!sleepyPar.activeSelf)
        {
            defaultPos = new Vector2(Random.Range(-1, 1), Random.Range(0.75f, 1.25f));

            if (randomCo != null)
            {
                StopCoroutine(randomCo);
            }

            rig.velocity = Vector2.zero;
            return_ = true;
        }     
    }

    IEnumerator RandomCo()
    {
        while(true)
        {
            rig.velocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed));

            yield return w0dot5;
        }
    }


    IEnumerator AttackCo()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            minimumHP = float.MaxValue;

            for(int i = 0; i < manager.enemies.Count; i++)
            {
                enemyHP = manager.enemies[i].currentHP / manager.enemies[i].totalHP;

                if(enemyHP < minimumHP)
                {
                    minimumHP = enemyHP;
                    target = manager.enemies[i].transform;
                }
            }

            rotate = true;

            yield return new WaitUntil(() => !rotate);

            insProj = Instantiate(proj, spawn.position, gun.rotation);
            insProj.GetComponent<HealingProj>().target = target;

            yield return w2;
        }
    }

    IEnumerator RunCo()
    {
        if(transform.position.y > 4.45f)
        {
            HPManager(100);
        }

        else
        {
            yield return w0dot44;
            speed *= 1.5f;
            randomCo = StartCoroutine(RandomCo());
            crazy = true;
        }
    }
}
