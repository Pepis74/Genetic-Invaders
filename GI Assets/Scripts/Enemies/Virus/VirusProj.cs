using System.Collections;
using UnityEngine;

public class VirusProj : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    GameObject defaultProj;
    [SerializeField]
    Sprite defaultGun;
    [SerializeField]
    GameObject par;
    Vector2 direction;
    GameManager manager;
    AssignmentManager assignment;
    GameObject ins;
    AAManager aminos;
    Warning warn;
    bool running;
    float angle;
    bool destroyed;

    void Start()
    {
        assignment = FindObjectOfType<AssignmentManager>();
        manager = FindObjectOfType<GameManager>();
        aminos = FindObjectOfType<AAManager>();
        warn = FindObjectOfType<Warning>();
    }

    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, manager.ship.transform.position, speed);

        direction = manager.ship.transform.position - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ship" && !destroyed)
        {
            destroyed = true;
            aminos.selectedAmino = 0;

            manager.totalHP = 100;

            manager.HPManager(0, null);

            manager.totalEnergy = 6;

            if(manager.currentEnergy > manager.totalEnergy)
            {
                manager.currentEnergy = manager.totalEnergy;
            }

            manager.energyBar.fillAmount = manager.currentEnergy / manager.totalEnergy;
            manager.proj = defaultProj;
            manager.gun.GetComponentInChildren<SpriteRenderer>().sprite = defaultGun;
            ins = Instantiate(par, transform.position, par.transform.rotation);
            Destroy(transform.GetChild(0).gameObject);
            running = true;
            assignment.DecompleteItems();
            StartCoroutine(WarnTextCo());
            StartCoroutine(DestroyCo());
        }
    }

    IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(ins);

        yield return new WaitUntil(() => !running);

        Destroy(gameObject);
    }

    IEnumerator WarnTextCo()
    {
        yield return new WaitUntil(() => !warn.running);

        warn.WarnText(3);
        running = false;
    }
}
