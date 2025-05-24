using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    Enemy target;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Enemy>() && other.GetComponent<Enemy>().tut == null)
        {          
            target = other.GetComponent<Enemy>();
            target.manager.SetTarget(target);
        }
    }
}
