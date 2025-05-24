using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus : MonoBehaviour
{
    [SerializeField]
    Transform spawn;
    [SerializeField]
    GameObject proj;
    [SerializeField]
    GameObject top;
    [SerializeField]
    AudioSource shoot;
    [SerializeField]
    AudioSource wink;
    [SerializeField]
    AudioSource dash;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(Coroutine());
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(6);
        GetComponentInChildren<Enemy>().Deactivate();

        yield return new WaitForSeconds(0.5f);
        wink.Play();

        yield return new WaitForSeconds(0.5f);
        dash.Play();

        yield return new WaitForSeconds(0.2f);
        Destroy(top);
        anim.SetBool("Start", true);

        yield return new WaitForSeconds(0.87f);
        Instantiate(proj, spawn.position, proj.transform.rotation);
        shoot.Play();

        yield return new WaitForSeconds(0.61f);
        Destroy(gameObject);
    }
}
