using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateralBeam : MonoBehaviour
{
    [SerializeField]
    GameObject par;
    float damage;
    GameManager manager;
    Enemy iD;
    GameObject ins;
    WaitForSeconds w1dot5 = new WaitForSeconds(1.5f);

    void Start()
    {
        iD = GetComponentInParent<Enemy>();
        damage = iD.damage;
        manager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ship")
        {
            manager.HPManager(damage, iD);
            ins = Instantiate(par, other.transform.position, par.transform.rotation);
        }
    }
}
