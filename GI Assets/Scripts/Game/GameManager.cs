using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public List<Enemy> shieldedEnemies = new List<Enemy>();
    public Rigidbody2D rig;
    public Enemy target;
    public GameObject proj;
    public Transform ship;
    public Transform gun;
    public Transform spawn;
    public float currentEnergy;
    public float recovery;
    public GameObject letters;
    public AudioSource dashSound;
    public bool able;
    public int score;
    public List<GameObject> toDestroy = new List<GameObject>();
    public Transform consSource;
    public float currentHP;
    public bool alreadyGennon;
    public bool alreadyHealing;
    public bool alreadyVirus;
    public bool alreadyLateral;
    public bool alreadyCorona;
    public AudioSource gunSound;
    public float totalEnergy;
    public float totalHP = 100;
    public GameObject insDeath;
    public Text energyText;
    public bool outside;
    public Image energyBar;
    [SerializeField]
    float[] recoveryTimes;
    [SerializeField]
    List<float> enemyTimes = new List<float>();
    [SerializeField]
    List<float> maxEnemyTimes = new List<float>();
    [SerializeField]
    GameObject menu;
    [SerializeField]
    GameObject deathPar;
    [SerializeField]
    AudioSource errorSound;
    [SerializeField]
    GameObject HealObsWarn;
    [SerializeField]
    GameObject LootObsWarn;
    [SerializeField]
    GameObject DmgObsWarn;
    [SerializeField]
    GameObject HealObs;
    [SerializeField]
    GameObject DmgObs;
    [SerializeField]
    GameObject LootObs;
    [SerializeField]
    float energyRecover;
    [SerializeField]
    Image hPBar;
    [SerializeField]
    float rotSpeed; 
    [SerializeField]
    int maxEnemies;
    [SerializeField]
    List<GameObject> enemiesObjects = new List<GameObject>();
    [SerializeField]
    Text scoreText;
    Enemy oldTarget;
    AAManager aminos;
    Animator cam;
    WaitForSeconds scoreWait = new WaitForSeconds(0.05f);
    WaitForSeconds spawnWait1 = new WaitForSeconds(1.5f);
    WaitForSeconds obsWait = new WaitForSeconds(7);
    WaitForSeconds warnWait = new WaitForSeconds(1);
    WaitForSeconds coronaWait = new WaitForSeconds(0.5f);
    GameObject insTarget;
    GameObject insEnemy;
    GameObject insObs;
    GameObject insPropellers;
    GameObject insCollider;
    GameObject insProj;
    Enemy closestEnemy;
    Vector2 direction;
    Transform letterChild;
    public List<Item>[] items = new List<Item>[2];
    Color color;
    Tutorial tutorial;
    int maxNumber = 1;
    float angle;
    float spawnIncrement;
    float firstScore;
    int oldScore;
    float minimumDist;
    float dist;
    int randomEnemy;
    int randomNumber;
    float multiplier;
    float randomPos;
    bool start;
    int numberInEnemies;
    bool immortal;
    bool recoveryActive;
    Coroutine regenCo;
    AssignmentManager assignment;

    void Start()
    {
        able = true;
        multiplier = 4;
        start = true;
        assignment = FindObjectOfType<AssignmentManager>();
        cam = FindObjectOfType<Camera>().GetComponent<Animator>();
        rig = ship.GetComponent<Rigidbody2D>();
        aminos = FindObjectOfType<AAManager>();
        tutorial = FindObjectOfType<Tutorial>();

        if(ES3.KeyExists("tutorial"))
        {
            if (ES3.KeyExists("unlockedLists"))
            {
                items[0] = ES3.Load<List<Item>[]>("unlockedLists")[1];

                for (int i = 0; i < items[0].Count; i++)
                {
                    if (items[0][i].selected)
                    {
                        gun.GetComponentInChildren<SpriteRenderer>().sprite = items[0][i].icon;
                        recovery = recoveryTimes[items[0][i].aminoNumber - 14];
                        proj = items[0][i].projectile;
                    }
                }

                items[1] = ES3.Load<List<Item>[]>("unlockedLists")[2];

                for (int i = 0; i < items[1].Count; i++)
                {
                    if (items[1][i].selected)
                    {
                        ship.GetComponent<SpriteRenderer>().sprite = items[1][i].icon;
                        Destroy(ship.GetChild(0).GetChild(0).gameObject);
                        Destroy(ship.GetChild(3).gameObject);
                        insCollider = Instantiate(items[1][i].collider, ship.position, items[1][i].collider.transform.rotation);
                        insCollider.transform.SetParent(ship);
                        insCollider.transform.localScale = Vector3.one;
                        insPropellers = Instantiate(items[1][i].propellers, ship.GetChild(0).position, items[1][i].propellers.transform.rotation);
                        insPropellers.transform.SetParent(ship.GetChild(0));
                    }
                }
            }

            if(!ES3.Load<bool>("tutorial"))
            {
                StartCoroutine(ScoreCo());
                StartCoroutine(EnemySpawnCo());
                StartCoroutine(ObsSpawnCo());
            }
        }

        /*if (ES3.KeyExists("unlockedLists"))
        {
            items[0] = ES3.Load<List<Item>[]>("unlockedLists")[1];

            for (int i = 0; i < items[0].Count; i++)
            {
                if (items[0][i].selected)
                {
                    gun.GetComponentInChildren<SpriteRenderer>().sprite = items[0][i].icon;
                    recovery = recoveryTimes[items[0][i].aminoNumber - 14];
                    proj = items[0][i].projectile;
                }
            }

            items[1] = ES3.Load<List<Item>[]>("unlockedLists")[2];

            for (int i = 0; i < items[1].Count; i++)
            {
                if (items[1][i].selected)
                {
                    ship.GetComponent<SpriteRenderer>().sprite = items[1][i].icon;
                    Destroy(ship.GetChild(0).GetChild(0).gameObject);
                    Destroy(ship.GetChild(3).gameObject);
                    insCollider = Instantiate(items[1][i].collider, ship.position, items[1][i].collider.transform.rotation);
                    insCollider.transform.SetParent(ship);
                    insCollider.transform.localScale = Vector3.one;
                    insPropellers = Instantiate(items[1][i].propellers, ship.GetChild(0).position, items[1][i].propellers.transform.rotation);
                    insPropellers.transform.SetParent(ship.GetChild(0));
                }
            }
        }

        if (!tutorial.tutorialActive)
        {
            StartCoroutine(ScoreCo());
            StartCoroutine(EnemySpawnCo());
            StartCoroutine(ObsSpawnCo());
        }*/

        currentHP = totalHP;
        currentEnergy = totalEnergy;
    }

    void Update()
    {
        if(enemies.Count + shieldedEnemies.Count > 0 && !immortal)
        {
            if(target == null)
            {
                minimumDist = float.MaxValue;
                
                if(enemies.Count > 0)
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        dist = Vector2.Distance(ship.position, enemies[i].transform.position);

                        if (dist < minimumDist)
                        {
                            minimumDist = dist;
                            closestEnemy = enemies[i];
                        }
                    }
                }
                
                else
                {
                    for (int i = 0; i < shieldedEnemies.Count; i++)
                    {
                        dist = Vector2.Distance(ship.position, shieldedEnemies[i].transform.position);

                        if (dist < minimumDist)
                        {
                            minimumDist = dist;
                            closestEnemy = shieldedEnemies[i];
                        }
                    }
                }

                SetTarget(closestEnemy);
            }

            if(target != null)
            {
                direction = target.transform.position - gun.position;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                gun.rotation = Quaternion.RotateTowards(gun.rotation, Quaternion.Euler(0, 0, angle - 90), rotSpeed);
            }
        }

        ship.rotation = Quaternion.Euler(Vector3.zero);
    }

    void FixedUpdate()
    {
        if(currentEnergy < totalEnergy)
        {
            currentEnergy += energyRecover;
            energyBar.fillAmount = currentEnergy / totalEnergy;

            if(currentEnergy > totalEnergy)
            {
                currentEnergy = totalEnergy;
            }

            if(currentEnergy < 3)
            {
                energyText.text = "0";
            }

            else if (currentEnergy >= 3 && currentEnergy < 6)
            {
                energyText.text = "1";
            }

            else if (currentEnergy >= 6 && currentEnergy < 9)
            {
                energyText.text = "2";
            }

            else if (currentEnergy >= 9 && currentEnergy < 12)
            {
                energyText.text = "3";
            }

            else if (currentEnergy == 12)
            {
                energyText.text = "4";
            }
        }

        if(start)
        {
            ship.position = Vector2.MoveTowards(ship.position, new Vector2(0, -3.67f), 2.5f * Time.deltaTime);

            if(new Vector2(ship.position.x, ship.position.y) == new Vector2(0, -3.67f))
            {
                start = false;
            }
        }
    }

    public void SetTarget(Enemy targetToSet)
    {
        if(!recoveryActive && !immortal)
        {
            target = targetToSet;
            consSource.position = new Vector3(target.transform.position.x, target.transform.position.y + target.targetOffset, -2);
            consSource.SetParent(target.transform);

            for (int i = 0; i < letters.transform.childCount; i++)
            {
                letterChild = letters.transform.GetChild(i);

                color = letterChild.GetComponentInChildren<Text>().color;
                color.a = 1;
                letterChild.GetComponentInChildren<Text>().color = color;
                letterChild.GetComponent<Button>().enabled = true;
            }
        }
    }

    public void A()
    {
        if (target.letterChain[0] == 1)
        {
            Attack();
        }

        else
        {
            if(!proj.GetComponent<Projectile>().devilish)
            {
                StartCoroutine(RecoveryCo());
            }

            else
            {
                able = false;

                errorSound.Play();

                for (int i = 0; i < letters.transform.childCount; i++)
                {
                    letterChild = letters.transform.GetChild(i);
                    color = letterChild.GetComponentInChildren<Text>().color;
                    color.a = 0.4f;
                    letterChild.GetComponentInChildren<Text>().color = color;
                    letterChild.GetComponent<Button>().enabled = false;
                }

                currentHP = 0;
                ship.GetComponentInChildren<Collider2D>().enabled = false;
                insDeath = Instantiate(deathPar, ship.position, deathPar.transform.rotation);
                insDeath.transform.localScale = new Vector3(4, 4, 1);
                ship.GetChild(3).gameObject.SetActive(true);

                if (aminos.selectedAmino == 7 && !aminos.usedImm)
                {
                    aminos.Immortal();
                }

                else
                {
                    StartCoroutine(DeathCo());
                }
            }
        }
    }

    public void U()
    {
        if (target.letterChain[0] == 0)
        {
            Attack();
        }

        else
        {
            if (!proj.GetComponent<Projectile>().devilish)
            {
                StartCoroutine(RecoveryCo());
            }

            else
            {
                able = false;

                errorSound.Play();

                for (int i = 0; i < letters.transform.childCount; i++)
                {
                    letterChild = letters.transform.GetChild(i);
                    color = letterChild.GetComponentInChildren<Text>().color;
                    color.a = 0.4f;
                    letterChild.GetComponentInChildren<Text>().color = color;
                    letterChild.GetComponent<Button>().enabled = false;
                }

                currentHP = 0;
                ship.GetComponentInChildren<Collider2D>().enabled = false;
                insDeath = Instantiate(deathPar, ship.position, deathPar.transform.rotation);
                insDeath.transform.localScale = new Vector3(4, 4, 1);
                ship.GetChild(3).gameObject.SetActive(true);

                if (aminos.selectedAmino == 7 && !aminos.usedImm)
                {
                    aminos.Immortal();
                }

                else
                {
                    StartCoroutine(DeathCo());
                }
            }
        }
    }

    public void G()
    {
        if (target.letterChain[0] == 3)
        {
            Attack();
        }

        else
        {
            if (!proj.GetComponent<Projectile>().devilish)
            {
                StartCoroutine(RecoveryCo());
            }

            else
            {
                able = false;

                errorSound.Play();

                for (int i = 0; i < letters.transform.childCount; i++)
                {
                    letterChild = letters.transform.GetChild(i);
                    color = letterChild.GetComponentInChildren<Text>().color;
                    color.a = 0.4f;
                    letterChild.GetComponentInChildren<Text>().color = color;
                    letterChild.GetComponent<Button>().enabled = false;
                }

                currentHP = 0;
                ship.GetComponentInChildren<Collider2D>().enabled = false;
                insDeath = Instantiate(deathPar, ship.position, deathPar.transform.rotation);
                insDeath.transform.localScale = new Vector3(4, 4, 1);
                ship.GetChild(3).gameObject.SetActive(true);

                if (aminos.selectedAmino == 7 && !aminos.usedImm)
                {
                    aminos.Immortal();
                }

                else
                {
                    StartCoroutine(DeathCo());
                }
            }
        }
    }

    public void C()
    {
        if (target.letterChain[0] == 2)
        {
            Attack();
        }

        else
        {
            if (!proj.GetComponent<Projectile>().devilish)
            {
                StartCoroutine(RecoveryCo());
            }

            else
            {
                able = false;

                errorSound.Play();

                for (int i = 0; i < letters.transform.childCount; i++)
                {
                    letterChild = letters.transform.GetChild(i);
                    color = letterChild.GetComponentInChildren<Text>().color;
                    color.a = 0.4f;
                    letterChild.GetComponentInChildren<Text>().color = color;
                    letterChild.GetComponent<Button>().enabled = false;
                }

                currentHP = 0;
                ship.GetComponentInChildren<Collider2D>().enabled = false;
                insDeath = Instantiate(deathPar, ship.position, deathPar.transform.rotation);
                insDeath.transform.localScale = new Vector3(4, 4, 1);
                ship.GetChild(3).gameObject.SetActive(true);

                if (aminos.selectedAmino == 7 && !aminos.usedImm)
                {
                    aminos.Immortal();
                }

                else
                {
                    StartCoroutine(DeathCo());
                }
            }
        }
    }

    public void HPManager(float damage, Enemy iD)
    {
        if(!immortal)
        {
            if (damage > 0)
            {
                if (aminos.selectedAmino == 5)
                {
                    damage *= 1.5f;
                }

                cam.SetBool("Damage", true);
                StartCoroutine(AnimCo());
            }

            currentHP -= damage;

            if (currentHP > totalHP)
            {
                currentHP = totalHP;
            }

            else if (currentHP <= 0 && tutorial == null)
            {
                able = false;
                currentHP = 0;

                for (int i = 0; i < letters.transform.childCount; i++)
                {
                    letterChild = letters.transform.GetChild(i);
                    color = letterChild.GetComponentInChildren<Text>().color;
                    color.a = 0.4f;
                    letterChild.GetComponentInChildren<Text>().color = color;
                    letterChild.GetComponent<Button>().enabled = false;
                }

                ship.GetComponentInChildren<Collider2D>().enabled = false;
                insDeath = Instantiate(deathPar, ship.position, deathPar.transform.rotation);
                insDeath.transform.localScale = new Vector3(4, 4, 1);
                ship.GetChild(3).gameObject.SetActive(true);

                if (aminos.selectedAmino == 7 && !aminos.usedImm)
                {
                    aminos.Immortal();
                }

                else
                {
                    StartCoroutine(DeathCo());
                }
            }

            hPBar.fillAmount = currentHP / totalHP;
            assignment.CheckHealth(hPBar.fillAmount, damage);

            if (aminos.selectedAmino == 9 && damage > 0)
            {
                if (regenCo != null)
                {
                    aminos.StopCoroutine(regenCo);
                }

                regenCo = aminos.StartCoroutine("RegenCountCo");
            }

            else if (aminos.selectedAmino == 11 && iD != null)
            {
                gunSound.Play();
                insProj = Instantiate(proj, spawn.position, gun.rotation);
                insProj.GetComponent<Projectile>().target = iD;
                insProj.transform.SetParent(spawn);
                insProj.transform.localPosition = new Vector2(0, insProj.GetComponent<Projectile>().offset);
                insProj.transform.SetParent(null);
            }
        }
    }

    public void EnergyManager(int waste)
    {
        currentEnergy -= waste;

        if(currentEnergy > totalEnergy)
        {
            currentEnergy = totalEnergy;
        }

        energyBar.fillAmount = currentEnergy / totalEnergy;
    }

    void Attack()
    {
        gunSound.pitch = Random.Range(0.8f, 1.1f);
        gunSound.Play();
        insProj = Instantiate(proj, spawn.position, gun.rotation);
        insProj.GetComponent<Projectile>().target = target;
        insProj.transform.SetParent(spawn);
        insProj.transform.localPosition = new Vector2(0, insProj.GetComponent<Projectile>().offset);
        insProj.transform.SetParent(null);

        target.letterChain.RemoveAt(0);

        if(target.letterChain.Count == 0)
        {
            for (int i = 0; i < letters.transform.childCount; i++)
                {
                    letterChild = letters.transform.GetChild(i);
                    color = letterChild.GetComponentInChildren<Text>().color;
                    color.a = 0.4f;
                    letterChild.GetComponentInChildren<Text>().color = color;
                    letterChild.GetComponent<Button>().enabled = false;
                }
        }

        Destroy(target.insSubLet[0]);
        target.insSubLet.RemoveAt(0);

        if (aminos.selectedAmino == 8)
        {
            aminos.Lucky();
        }

        else if (aminos.selectedAmino == 9)
        {
            if (regenCo != null)
            {
                aminos.StopCoroutine(regenCo);
            }

            regenCo = aminos.StartCoroutine("RegenCountCo");
        }

        else if (aminos.selectedAmino == 10)
        {
            int letterChainLength = target.letterChain.Count;

            for (int i = 0; i < target.letterChain.Count; i++)
            { 
                Destroy(target.insSubLet[i]);
            }

            target.letterChain.Clear();
            target.insSubLet.Clear();

            target.RandomLetterGenerator(letterChainLength);
        }

        else if (aminos.selectedAmino == 12)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == target)
                {
                    numberInEnemies = i;
                }
            }

            if (numberInEnemies + 1 < enemies.Count)
            {
                SetTarget(enemies[numberInEnemies + 1]);
            }

            else
            {
                SetTarget(enemies[0]);
            }
        }
    }

    public IEnumerator ScoreCo()
    {
        while(true)
        {
            yield return scoreWait;

            if(aminos.selectedAmino == 2)
            {
                firstScore += 1 * multiplier;
                score = Mathf.RoundToInt(firstScore);

                if(multiplier - 0.0025f >= 1)
                {
                    multiplier -= 0.0025f;
                } 
            }

            else
            {
                score += 1;
            }
            
            scoreText.text = "" + score;
        }
    }

    public IEnumerator EnemySpawnCo()
    {
        yield return spawnWait1;

        while (true)
        {
            if(score - oldScore >= 400)
            {
                if(maxNumber < 2)
                {
                    maxNumber += 1;
                }

                else if(maxNumber < 8)
                {
                    maxNumber += 6;
                }

                if (maxEnemies < 4)
                {
                    maxEnemies += 1;
                }

                for(int i = 0; i < enemyTimes.Count; i++)
                {
                    if(enemyTimes[i] - 0.25f > maxEnemyTimes[i])
                    {
                        enemyTimes[i] -= 0.25f;
                    }

                    else
                    {
                        enemyTimes[i] = maxEnemyTimes[i];
                    }
                }

                oldScore = score;
            }

            if(enemies.Count + shieldedEnemies.Count < maxEnemies)
            {
                randomEnemy = Random.Range(0, maxNumber);

                switch(randomEnemy)
                {
                    case 0:
                        insEnemy = Instantiate(enemiesObjects[0], new Vector3(Random.Range(-2.469f, 2.469f), 6.3f, -spawnIncrement), enemiesObjects[0].transform.rotation);
                        enemies.Add(insEnemy.GetComponent<Enemy>());
                        insEnemy.GetComponent<Enemy>().zOffset = -spawnIncrement;
                        randomEnemy = 0;
                        break;
               
                    case 1:
                        insEnemy = Instantiate(enemiesObjects[1], new Vector3(Random.Range(-2.4f, 2.4f), 5.554f, -spawnIncrement), enemiesObjects[1].transform.rotation);
                        enemies.Add(insEnemy.GetComponent<Enemy>());
                        randomEnemy = 1;
                        break;

                    case 2:
                        if (alreadyGennon)
                        {
                            insEnemy = Instantiate(enemiesObjects[1], new Vector3(Random.Range(-2.4f, 2.4f), 5.554f, -spawnIncrement), enemiesObjects[1].transform.rotation);
                            enemies.Add(insEnemy.GetComponent<Enemy>());
                            randomEnemy = 1;
                        }

                        else
                        {
                            insEnemy = Instantiate(enemiesObjects[2], new Vector3(Random.Range(-1.56f, 1.56f), 5.9f, -spawnIncrement), enemiesObjects[2].transform.rotation);
                            randomEnemy = 2;
                        }

                        break;

                    case 3:
                        insEnemy = Instantiate(enemiesObjects[3], new Vector3(Random.Range(-2.469f, 2.469f), 6.3f, -spawnIncrement), enemiesObjects[3].transform.rotation);
                        enemies.Add(insEnemy.GetComponent<Enemy>());
                        insEnemy.GetComponent<Enemy>().zOffset = -spawnIncrement;
                        randomEnemy = 3;
                        break;

                    case 4:
                        if(alreadyHealing)
                        {
                            insEnemy = Instantiate(enemiesObjects[3], new Vector3(Random.Range(-2.469f, 2.469f), 6.3f, -spawnIncrement), enemiesObjects[3].transform.rotation);
                            enemies.Add(insEnemy.GetComponent<Enemy>());
                            insEnemy.GetComponent<Enemy>().zOffset = -spawnIncrement;
                            randomEnemy = 3;
                        }

                        else
                        {
                            insEnemy = Instantiate(enemiesObjects[4], new Vector3(Random.Range(-2.469f, 2.469f), 6.2f, -spawnIncrement), enemiesObjects[4].transform.rotation);
                            insEnemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -250f));
                            randomEnemy = 4;
                        }
                        
                        break;

                    case 5:
                        if(alreadyCorona || alreadyVirus)
                        {
                            insEnemy = Instantiate(enemiesObjects[0], new Vector3(Random.Range(-2.469f, 2.469f), 6.3f, -spawnIncrement), enemiesObjects[0].transform.rotation);
                            enemies.Add(insEnemy.GetComponent<Enemy>());
                            insEnemy.GetComponent<Enemy>().zOffset = -spawnIncrement;
                            randomEnemy = 0;
                        }

                        else
                        {
                            insEnemy = Instantiate(enemiesObjects[5], new Vector3(Random.Range(-2.22f, 2.22f), 4, -spawnIncrement), enemiesObjects[5].transform.rotation);
                            enemies.Add(insEnemy.GetComponentInChildren<Enemy>());
                            randomEnemy = 5;
                        }

                        break;

                    case 6:
                        if (alreadyLateral)
                        {
                            insEnemy = Instantiate(enemiesObjects[1], new Vector3(Random.Range(-2.4f, 2.4f), 5.554f, -spawnIncrement), enemiesObjects[1].transform.rotation);
                            enemies.Add(insEnemy.GetComponent<Enemy>());
                            randomEnemy = 1;
                        }

                        else
                        {
                            insEnemy = Instantiate(enemiesObjects[6], new Vector3(0, 6.3f, -spawnIncrement), enemiesObjects[6].transform.rotation);
                            enemies.Add(insEnemy.GetComponent<Enemy>());
                            randomEnemy = 6;
                        }

                        break;

                    case 7:
                        if (alreadyCorona || alreadyVirus)
                        {
                            insEnemy = Instantiate(enemiesObjects[1], new Vector3(Random.Range(-2.4f, 2.4f), 5.554f, -spawnIncrement), enemiesObjects[1].transform.rotation);
                            enemies.Add(insEnemy.GetComponent<Enemy>());
                            randomEnemy = 1;
                        }

                        else
                        {
                            insEnemy = Instantiate(enemiesObjects[7], new Vector3(Random.Range(-2.22f, 2.22f), 4, -spawnIncrement), enemiesObjects[7].transform.rotation);
                            enemies.Add(insEnemy.GetComponentInChildren<Enemy>());
                            randomEnemy = 7;
                        }

                        break;
                }

                insEnemy.GetComponentInChildren<Enemy>().zOffset = -spawnIncrement;

                spawnIncrement += 0.2f;

                if (spawnIncrement > 2)
                {
                    spawnIncrement = 0;
                }

                yield return new WaitForSeconds(enemyTimes[randomEnemy]);
            }

            yield return scoreWait;
        }
    }

    public IEnumerator ObsSpawnCo()
    {
        yield return obsWait;

        while(true)
        {
            randomNumber = Random.Range(0, 30);

            if(randomNumber < 14)
            {
                randomPos = Random.Range(-2.25f, 2.25f);
                insObs = Instantiate(DmgObsWarn, new Vector3(randomPos, 0, -3), DmgObsWarn.transform.rotation);
                yield return warnWait;
                Destroy(insObs);
                Instantiate(DmgObs, new Vector2(randomPos, 5.45f), Quaternion.Euler(0, 0, Random.Range(0, 360)));
            }

            else if (randomNumber >= 14 && randomNumber < 29)
            {
                randomPos = Random.Range(-2.25f, 2.25f);
                insObs = Instantiate(HealObsWarn, new Vector3(randomPos, 0, -3), HealObsWarn.transform.rotation);
                yield return warnWait;
                Destroy(insObs);
                Instantiate(HealObs, new Vector2(randomPos, 5.45f), Quaternion.Euler(0, 0, Random.Range(0, 360)));
            }

            else if (randomNumber == 29)
            {
                randomPos = Random.Range(-2.25f, 2.25f);
                insObs = Instantiate(LootObsWarn, new Vector3(randomPos, 0, -3), LootObsWarn.transform.rotation);
                yield return warnWait;
                Destroy(insObs);
                Instantiate(LootObs, new Vector2(randomPos, 5.45f), Quaternion.Euler(0, 0, Random.Range(0, 360)));
            }

            yield return obsWait;
        }
    }

    public IEnumerator CoronaCo()
    {
        while(true)
        {
            yield return coronaWait;
            HPManager(1, null);
        }
    }

    IEnumerator RecoveryCo()
    {
        errorSound.Play();
        recoveryActive = true;

        for(int i = 0; i < letters.transform.childCount; i++)
        {
            letterChild = letters.transform.GetChild(i);
            color = letterChild.GetComponentInChildren<Text>().color;
            color.a = 0.4f;
            letterChild.GetComponentInChildren<Text>().color = color;
            letterChild.GetComponent<Button>().enabled = false;
        }

        yield return new WaitForSeconds(recovery);

        recoveryActive = false;

        for (int i = 0; i < letters.transform.childCount; i++)
        {
            letterChild = letters.transform.GetChild(i);
            color = letterChild.GetComponentInChildren<Text>().color;
            color.a = 1;
            letterChild.GetComponentInChildren<Text>().color = color;
            letterChild.GetComponent<Button>().enabled = true;
        }
    }

    public IEnumerator DeathCo()
    {
        yield return warnWait;

        for(int i = 0; i < toDestroy.Count; i++)
        {
            Destroy(toDestroy[i]);
        }

        Destroy(insDeath);
        insDeath = Instantiate(deathPar, ship.position, deathPar.transform.rotation);
        insDeath.transform.localScale = new Vector3(4, 4, 1);
        ship.position = new Vector2(0, -100);

        yield return new WaitForSeconds(0.3f);

        menu.SetActive(true);
        Time.timeScale = 0;
    }

    IEnumerator AnimCo()
    {
        yield return scoreWait;
        cam.SetBool("Damage", false);
    }

    public IEnumerator ImmortalCo()
    {
        ship.GetChild(4).gameObject.SetActive(true);
        immortal = true;

        for (int i = 0; i < letters.transform.childCount; i++)
        {
            letterChild = letters.transform.GetChild(i);
            color = letterChild.GetComponentInChildren<Text>().color;
            color.a = 0.4f;
            letterChild.GetComponentInChildren<Text>().color = color;
            letterChild.GetComponent<Button>().enabled = false;
        }

        yield return new WaitForSeconds(1);

        Destroy(ship.GetChild(4).gameObject);
        immortal = false;

        for (int i = 0; i < letters.transform.childCount; i++)
        {
            letterChild = letters.transform.GetChild(i);
            color = letterChild.GetComponentInChildren<Text>().color;
            color.a = 1;
            letterChild.GetComponentInChildren<Text>().color = color;
            letterChild.GetComponent<Button>().enabled = true;
        }
    }
}
