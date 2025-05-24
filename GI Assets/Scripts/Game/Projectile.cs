using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool devilish;
    public Enemy target;
    public float offset;
    public GameManager manager;
    public bool startEnded;

    void Update()
    {
        if(target == null && startEnded)
        {
            Destroy(gameObject);
        }
    }
}
