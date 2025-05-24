using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOnTouch : MonoBehaviour
{
    public float boostSpeed;
    public Vector2 touchPos;
    public Vector2 oldPos;
    public float speed;
    public float touchWait = 0.1f;
    public float doubleTapWait = 1;
    public bool force;
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject touchObj;
    [SerializeField]
    GameObject trails;
    Camera mainCamera;
    GameObject ins;
    Vector2 oldDashPos;
    GameManager manager;
    WaitForSeconds boostDur = new WaitForSeconds(0.15f);
    WaitForSeconds wait = new WaitForSeconds(0.05f);
    WaitForSeconds afterBoost = new WaitForSeconds(0.25f);
    Animator anim;
    int counter;
    bool first;
    public bool boosting;
    public bool able;
    bool secondAble;
    public float minimumDist;
    Tutorial tutorial;
    float dist;
    Coroutine pauseCo;
    Coroutine boostCo;

    void Start()
    {
        anim = GetComponent<Animator>();
        mainCamera = GetComponent<Camera>();
        manager = FindObjectOfType<GameManager>();
        tutorial = FindObjectOfType<Tutorial>();
        able = true;
        secondAble = true;
        first = true;
    }

    void Update()
    {
        if (Input.touchCount > 0 && manager.able)
        {
            touchPos = mainCamera.ScreenToWorldPoint(Input.touches[0].position);

            if (Vector2.Distance(touchPos, oldPos) > 0.1f)
            {
                if (ins != null)
                {
                    Destroy(ins);
                }

                ins = Instantiate(touchObj, touchPos, touchObj.transform.rotation);
            }

            minimumDist = float.MaxValue;

            for (int i = 0; i < manager.enemies.Count; i++)
            {
                dist = Vector2.Distance(touchPos, manager.enemies[i].transform.position);

                if (dist < minimumDist)
                {
                    minimumDist = dist;
                }
            }

            if (!boosting)
            {
                if (touchPos.x < -1.53f && minimumDist > 0.7f || touchPos.x > 1.53f && minimumDist > 0.7f || touchPos.y > -3.25f && minimumDist > 0.7f || touchPos.y < -4.37f && minimumDist > 0.7f)
                {
                    float width = Screen.currentResolution.width;
                    float height = Screen.currentResolution.height;

                    float currentRes = width / height;

                    if (tutorial == null)
                    {
                        if (touchPos.x > 0)
                        {
                            manager.rig.velocity = new Vector2(speed * Time.deltaTime, 0);
                        }

                        else
                        {
                            manager.rig.velocity = new Vector2(-speed * Time.deltaTime, 0);
                        }

                        if (first)
                        {
                            counter += 1;
                            pauseCo = StartCoroutine(DoubleTapCo());

                            if (counter > 1)
                            {
                                counter = 0;
                                pauseMenu.SetActive(true);
                                Time.timeScale = 0;
                            }
                        }

                        if (able && manager.currentEnergy - 3 >= 0 && secondAble)
                        {
                            able = false;
                            oldDashPos = touchPos;
                            boostCo = StartCoroutine(TouchWaitCo());
                        }
                    }

                    else if(currentRes < 0.51f)
                    {
                        if(touchPos.x < 0.6f || touchPos.x > 1.9f || touchPos.y > -1.62f || touchPos.y < -2.09f)
                        {
                            if (touchPos.x > 0)
                            {
                                manager.rig.velocity = new Vector2(speed * Time.deltaTime, 0);
                            }

                            else
                            {
                                manager.rig.velocity = new Vector2(-speed * Time.deltaTime, 0);
                            }

                            if (able && manager.currentEnergy - 3 >= 0 && secondAble)
                            {
                                able = false;
                                oldDashPos = touchPos;
                                boostCo = StartCoroutine(TouchWaitCo());
                            }
                        }
                    }

                    else
                    {
                        if (touchPos.x < 0.73f || touchPos.x > 2.13f || touchPos.y > -1.7f || touchPos.y < -2.22f)
                        {
                            if (touchPos.x > 0)
                            {
                                manager.rig.velocity = new Vector2(speed * Time.deltaTime, 0);
                            }

                            else
                            {
                                manager.rig.velocity = new Vector2(-speed * Time.deltaTime, 0);
                            }

                            if (able && manager.currentEnergy - 3 >= 0 && secondAble)
                            {
                                able = false;
                                oldDashPos = touchPos;
                                boostCo = StartCoroutine(TouchWaitCo());
                            }
                        }
                    }
                }

                if (Vector2.Distance(new Vector2(touchPos.x, 0), new Vector2(oldPos.x, 0)) > 2)
                {
                    if(pauseCo != null)
                    {
                        StopCoroutine(pauseCo);
                        counter = 0;
                    }
                    
                    if(Vector2.Distance(new Vector2(touchPos.x, 0), new Vector2(oldPos.x, 0)) > 0.5f && boostCo != null)
                    {
                        StopCoroutine(boostCo);
                        able = true;
                    }
                }
            }

            oldPos = touchPos;
            first = false;
        }

        else if (!boosting)
        {
            Destroy(ins);
            first = true;

            if (!force)
            {
                manager.rig.velocity = Vector2.zero;
            }
        }
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    IEnumerator DoubleTapCo()
    {
        yield return new WaitForSeconds(doubleTapWait);
        counter = 0;
    }

    IEnumerator TouchWaitCo()
    {
        yield return new WaitForSeconds(touchWait);

        if (Input.touchCount > 0)
        {
            if (Vector2.Distance(new Vector2(touchPos.x, 0), new Vector2(oldDashPos.x, 0)) > 0.2f)
            {
                boosting = true;
                secondAble = false;
                trails.SetActive(true);
                manager.dashSound.Play();

                if (touchPos.x > oldDashPos.x)
                {
                    manager.rig.velocity = new Vector2(boostSpeed, 0);
                    anim.SetBool("Right", true);
                }

                else
                {
                    manager.rig.velocity = new Vector2(-boostSpeed, 0);
                    anim.SetBool("Left", true);
                }

                StartCoroutine(AnimCo());

                manager.EnergyManager(3);

                yield return boostDur;

                manager.rig.velocity = Vector2.zero;
                trails.SetActive(false);

                yield return afterBoost;

                secondAble = true;
                boosting = false;
            }
        }

        able = true;

        yield return new WaitForSeconds(0.15f);
    }

    IEnumerator AnimCo()
    {
        yield return new WaitForSeconds(touchWait);
        anim.SetBool("Right", false);
        anim.SetBool("Left", false);
    }
}