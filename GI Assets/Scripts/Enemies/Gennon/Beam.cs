using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public Enemy iD;
    [SerializeField]
    float parDur;
    [SerializeField]
    GameObject par1;
    [SerializeField]
    GameObject par2;
    GameManager manager;
    GameObject ins1;
    GameObject ins2;
    float damage;
    bool contact;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        damage = GetComponentInParent<Enemy>().damage;

        if (contact && manager.currentHP > 0)
        {
            manager.HPManager(damage / 100, null);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ship")
        {
            contact = true;
            manager.HPManager(damage, iD);
            ins1 = Instantiate(par1, other.transform.position, par1.transform.rotation);
            StartCoroutine(DestroyCo());
            ins2 = Instantiate(par2, other.transform.position, par2.transform.rotation);
            ins2.transform.SetParent(other.transform);
            ins2.transform.localScale = Vector3.one;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ship")
        {
            contact = false;
            Destroy(ins2);
        }
    }

    IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(parDur);
        Destroy(ins1);
    }
}
