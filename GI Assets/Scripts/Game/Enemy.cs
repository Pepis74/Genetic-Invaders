using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float zOffset;
    public List<int> letterChain = new List<int>();
    public List<GameObject> insSubLet = new List<GameObject>();
    public GameManager manager;
    public float speed;
    public float oldSpeed;
    public GameObject insLet;
    public GameObject insProj;
    public Animator anim;
    public AudioSource sound;
    public float targetOffset;
    public Color statusColor;
    public float currentHP;
    public float totalHP;
    public float damage;
    public bool stunned;
    public bool venom;
    public float dmgRed;
    public float statusWait;
    public bool shield;
    public bool fire;
    public bool crystal;
    public float multiplier;
    public int letterNumber;
    public Tutorial tut;
    Coroutine fireCo;
    Coroutine crystalCo;
    Coroutine venomCo;
    Color color;
    [SerializeField]
    GameObject letterSpawn;
    [SerializeField]
    GameObject[] letters = new GameObject[4];
    [SerializeField]
    int enemyType;
    [SerializeField]
    GameObject shieldObj;
    [SerializeField]
    GameObject shieldBrk;
    [SerializeField]
    float aboveDist;
    [SerializeField]
    float belowDist;
    [SerializeField]
    GameObject par;
    [SerializeField]
    GameObject hPBorder;
    Image hPBar;
    GameObject insBar;
    GameObject insShield;
    Camera mainCamera;
    Transform canvas;
    Transform letterChild;
    int randomNumber;
    float increasingZAxis;
    WaitForSeconds waitPar = new WaitForSeconds(0.45f);
    WaitForSeconds w2dot5 = new WaitForSeconds(2.5f);
    AssignmentManager assignment;

    void Start()
    {
        tut = FindObjectOfType<Tutorial>();
        manager = FindObjectOfType<GameManager>();
        assignment = FindObjectOfType<AssignmentManager>();
        totalHP = letterNumber * 10;
        canvas = FindObjectOfType<Canvas>().transform;
        mainCamera = FindObjectOfType<Camera>();
        currentHP = totalHP;
        insBar = Instantiate(hPBorder, mainCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + aboveDist)), hPBorder.transform.rotation);
        insBar.transform.SetParent(canvas);
        insBar.GetComponent<RectTransform>().localScale = new Vector2(0.9f, 0.9f);
        manager.toDestroy.Add(insBar);
        insLet = Instantiate(letterSpawn, mainCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y - belowDist)), letterSpawn.transform.rotation);
        insLet.transform.SetParent(canvas);
        insLet.GetComponent<RectTransform>().localScale = new Vector2(0.9f, 0.9f);
        manager.toDestroy.Add(insLet);
        hPBar = insBar.transform.GetChild(0).GetComponent<Image>();
        insBar.SetActive(false);
        RandomLetterGenerator(letterNumber);

        switch(enemyType)
        {
            case 3:
                manager.alreadyGennon = true;
                break;

            case 5:
                manager.alreadyHealing = true;
                break;

            case 6:
                manager.alreadyVirus = true;
                break;

            case 7:
                manager.alreadyLateral = true;
                break;

            case 8:
                manager.alreadyCorona = true;
                break;
        }
    }

    void Update()
    {
        if (manager.target == this && shield)
        {
            for (int i = 0; i < manager.letters.transform.childCount; i++)
            {
                letterChild = manager.letters.transform.GetChild(i);
                color = letterChild.GetComponentInChildren<Text>().color;
                color.a = 0.4f;
                letterChild.GetComponentInChildren<Text>().color = color;
                letterChild.GetComponent<Button>().enabled = false;
            }
        }

        if (stunned && enemyType != 5 && enemyType != 6 && enemyType != 8  )
        {
            speed = 0;
            increasingZAxis += 240 * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, increasingZAxis);
        }

        if (fire && currentHP > 0)
        {
            HPManager(2.5f * multiplier * Time.deltaTime);
        }

        if (crystal && currentHP > 0 && letterChain.Count > 0)
        {
            HPManager(-3.75f * multiplier * Time.deltaTime);

            float div = currentHP / 10f;

            if (Mathf.Floor(div) > letterChain.Count - 1 && letterChain.Count < 6)
            {
                AddLetter();
            }
        }

        if (insBar != null && insLet != null)
        {
            insBar.transform.position = mainCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + aboveDist));
            insLet.transform.position = mainCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y - belowDist));
        }
    }

    public void Deactivate()
    {
        manager.enemies.Remove(this);

        if (manager.target = this)
        {
            for (int i = 0; i < manager.letters.transform.childCount; i++)
            {
                letterChild = manager.letters.transform.GetChild(i);
                color = letterChild.GetComponentInChildren<Text>().color;
                color.a = 0.4f;
                letterChild.GetComponentInChildren<Text>().color = color;
                letterChild.GetComponent<Button>().enabled = false;
            }

            manager.target = null;
            manager.consSource.SetParent(null);
            manager.consSource.position = new Vector2(0, 6);
        }

        Destroy(insBar);
        Destroy(insLet);
        Destroy(GetComponent<BoxCollider2D>());
    }

    public void HPManager(float damage)
    {
        currentHP -= damage;
        hPBar.fillAmount = currentHP / totalHP;

        if (currentHP <= 0)
        {
            if(tut != null)
            {
                tut.dead = true;
            }

            switch (enemyType)
            {
                case 3:
                    manager.alreadyGennon = false;
                    break;

                case 5:
                    manager.alreadyHealing = false;
                    break;

                case 6:
                    manager.alreadyVirus = false;
                    break;

                case 7:
                    manager.alreadyLateral = false;
                    break;

                case 8:
                    manager.alreadyCorona = false;
                    break;
            }

            if (manager.target = this)
            {
                for (int i = 0; i < manager.letters.transform.childCount; i++)
                {
                    letterChild = manager.letters.transform.GetChild(i);
                    color = letterChild.GetComponentInChildren<Text>().color;
                    color.a = 0.4f;
                    letterChild.GetComponentInChildren<Text>().color = color;
                    letterChild.GetComponent<Button>().enabled = false;
                }
            }

            if(insProj != null)
            {
                Destroy(insProj);
            }

            manager.enemies.Remove(this);
            manager.target = null;
            manager.consSource.SetParent(null);
            manager.consSource.position = new Vector2(0, 6);
            Destroy(insBar);
            Destroy(insLet);
            Destroy(GetComponent<BoxCollider2D>());
            Instantiate(par, transform.position, par.transform.rotation);
            assignment.AddKills(enemyType);

            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            else
            {
                Destroy(gameObject);
            }
        }

        else if (currentHP > totalHP)
        {
            currentHP = totalHP;
        }

        else
        {
            insBar.SetActive(true);
        }
    }

    public void RandomLetterGenerator(int numberOfLetters)
    {
        for (int a = 0; a < numberOfLetters; a++)
        {
            randomNumber = Random.Range(0, 4);

            insSubLet.Add(Instantiate(letters[randomNumber], insLet.transform.position, letters[randomNumber].transform.rotation));
            insSubLet[a].transform.SetParent(insLet.transform);
            insSubLet[a].transform.localScale = Vector3.one;
            insSubLet[a].transform.localPosition = new Vector2(-6 * (numberOfLetters - 1) + a * 12, 0);
            letterChain.Add(randomNumber);
        }
    }

    public void AddLetter()
    {
        for(int i = 0; i < insSubLet.Count; i++)
        {
            Destroy(insSubLet[i]);
        }

        insSubLet.Clear();

        for(int i = 0; i < letterChain.Count; i++)
        {
            insSubLet.Add(Instantiate(letters[letterChain[i]], insLet.transform.position, letters[letterChain[i]].transform.rotation));
            insSubLet[i].transform.SetParent(insLet.transform);
            insSubLet[i].transform.localScale = Vector3.one;
            insSubLet[i].transform.localPosition = new Vector2(-6 * letterChain.Count + i * 12, 0);
        }

        randomNumber = Random.Range(0, 4);
        insSubLet.Add(Instantiate(letters[randomNumber], insLet.transform.position, letters[randomNumber].transform.rotation));
        insSubLet[letterChain.Count].transform.SetParent(insLet.transform);
        insSubLet[letterChain.Count].transform.localScale = Vector3.one;
        insSubLet[letterChain.Count].transform.localPosition = new Vector2(6 * letterChain.Count, 0);
        letterChain.Add(randomNumber);
    }

    public void Shield()
    {
        shield = true;

        if(manager.enemies.Contains(this))
        {
            manager.enemies.Remove(this);
        }

        manager.shieldedEnemies.Add(this);
        shieldObj.SetActive(true);
    }

    public void DestroyShield()
    {
        shield = false;
        manager.shieldedEnemies.Remove(this);
        manager.enemies.Add(this);
        shieldObj.SetActive(false);
        insShield = Instantiate(shieldBrk, new Vector3(transform.position.x, transform.position.y, -10.5f), shieldBrk.transform.rotation);

        for (int i = 0; i < manager.letters.transform.childCount; i++)
        {
            letterChild = manager.letters.transform.GetChild(i);
            color = letterChild.GetComponentInChildren<Text>().color;
            color.a = 1;
            letterChild.GetComponentInChildren<Text>().color = color;
            letterChild.GetComponent<Button>().enabled = true;
        }

        StartCoroutine(ShieldCo());
    }

    public void ElementalCoroutines(int element)
    {
        switch(element)
        {
            case 0:
                if (fireCo != null)
                {
                    StopCoroutine(fireCo);
                }

                fireCo = StartCoroutine("FireCo");
                break;

            case 1:
                if (crystalCo != null)
                {
                    StopCoroutine(crystalCo);
                }

                crystalCo = StartCoroutine("CrystalCo");
                break;

            case 2:
                if (venomCo != null)
                {
                    StopCoroutine(venomCo);
                }

                venomCo = StartCoroutine("VenomCo");
                break;
        }
    }

    IEnumerator ShieldCo()
    {
        yield return waitPar;

        for(int i = 0; i < insShield.transform.childCount; i++)
        {
            Destroy(insShield.transform.GetChild(i).gameObject);
        }

        yield return waitPar;

        Destroy(insShield);
    }

    public IEnumerator StunCo()
    {
        oldSpeed = speed;
        stunned = true;

        yield return new WaitForSeconds(statusWait);

        stunned = false;
        speed = oldSpeed;
    }

    IEnumerator CrystalCo()
    {
        crystal = true;

        Image[] imgToChange = insBar.GetComponentsInChildren<Image>();

        for (int i = 0; i < imgToChange.Length; i++)
        {
            imgToChange[i].color = statusColor;
        }

        yield return new WaitForSeconds(statusWait);

        for (int i = 0; i < imgToChange.Length; i++)
        {
            imgToChange[i].color = Color.white;
        }

        crystal = false;
    }

    IEnumerator VenomCo()
    {
        damage *= dmgRed;

        Image[] imgToChange = insBar.GetComponentsInChildren<Image>();

        for (int i = 0; i < imgToChange.Length; i++)
        {
            imgToChange[i].color = statusColor;
        }

        venom = true;

        yield return new WaitForSeconds(statusWait);

        venom = false;

        for (int i = 0; i < imgToChange.Length; i++)
        {
            imgToChange[i].color = Color.white;
        }


        damage /= dmgRed;
    }

    IEnumerator FireCo()
    {
        fire = true;

        Image[] imgToChange = insBar.GetComponentsInChildren<Image>();

        for (int i = 0; i < imgToChange.Length; i++)
        {
            imgToChange[i].color = statusColor;
        }

        yield return new WaitForSeconds(statusWait);

        for (int i = 0; i < imgToChange.Length; i++)
        {
            imgToChange[i].color = Color.white;
        }

        fire = false;
    }
}
