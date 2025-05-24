using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AAManager : MonoBehaviour
{
    public int selectedAmino;
    public bool usedImm;
    [SerializeField]
    GameObject immortalBar;
    bool regen;
    int randomInt;
    GameManager manager;
    GameObject insProj;
    WaitForSeconds w3 = new WaitForSeconds(3);
    WaitForSeconds quarter = new WaitForSeconds(0.25f);
    List<Item> aminos = new List<Item>();

    void Start()
    {
        selectedAmino = 0;
        manager = FindObjectOfType<GameManager>();

        if (ES3.KeyExists("unlockedLists"))
        {
            aminos = ES3.Load<List<Item>[]>("unlockedLists")[0];

            for (int i = 0; i < aminos.Count; i++)
            {
                if (aminos[i].selected)
                {
                    selectedAmino = aminos[i].aminoNumber;
                }
            }
        }

        switch(selectedAmino)
        {
            case 1:
                manager.totalHP *= 0.85f;
                manager.currentHP = manager.totalHP;
                manager.totalEnergy *= 2;
                manager.energyText.text = "4";
                manager.currentEnergy = manager.totalEnergy;
                break;

            case 4:
                manager.totalEnergy *= 1.5f;
                manager.energyText.text = "3";
                manager.currentEnergy = manager.totalEnergy;
                break;

            case 6:
                manager.totalHP *= 1.2f;
                manager.currentHP = manager.totalHP;
                break;

            case 10:
                manager.totalHP *= 1.25f;
                manager.currentHP = manager.totalHP;
                manager.totalEnergy *= 1.5f;
                manager.energyText.text = "3";
                manager.currentEnergy = manager.totalEnergy;
                break;

            case 12:
                StartCoroutine(SpinnerCo());
                break;

            case 13:
                manager.totalHP *= 1.4f;
                manager.currentHP = manager.totalHP;
                manager.totalEnergy *= 0.5f;
                manager.energyText.text = "1";
                manager.currentEnergy = manager.totalEnergy;
                break;
        }
    }

    void FixedUpdate()
    {
        if(regen && selectedAmino == 9)
        {
            manager.HPManager(-0.33333333333333333333f, null);
        }
    }

    public void Lucky()
    {
        randomInt = Random.Range(0, 4);

        if(randomInt == 0)
        {
            StartCoroutine(LuckyCountCo());
        }
    }
    
    public void Immortal()
    {
        Instantiate(immortalBar, new Vector2(0, -5.19f), immortalBar.transform.rotation);
        usedImm = true;
    }

    public IEnumerator RegenCountCo()
    {
        regen = false;

        yield return w3;

        regen = true;
    }

    IEnumerator LuckyCountCo()
    {
        yield return quarter;

        manager.gunSound.Play();
        insProj = Instantiate(manager.proj, manager.spawn.position, manager.gun.rotation);
        insProj.GetComponent<Projectile>().target = manager.target;
        insProj.transform.SetParent(manager.spawn);
        insProj.transform.localPosition = new Vector2(0, insProj.GetComponent<Projectile>().offset);
        insProj.transform.SetParent(null);
    }

    IEnumerator SpinnerCo()
    {
        yield return new WaitForEndOfFrame();
        manager.recovery /= 4;
    }
}
