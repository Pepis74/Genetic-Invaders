using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignmentManager : MonoBehaviour
{
    public Assignment dailyAssignment;
    AAManager aminos;
    GameManager manager;
    Tutorial tutorial;
    Warning warn;
    public bool completed;
    bool lostHealth;
    bool lostHealing;
    bool lostStamina;
    public int kills;
    int partsCompleted;

    void Start()
    {
        tutorial = FindObjectOfType<Tutorial>();
        aminos = FindObjectOfType<AAManager>();
        manager = FindObjectOfType<GameManager>();
        warn = FindObjectOfType<Warning>();

        if(ES3.KeyExists("dailyAssignment"))
        {
            dailyAssignment = ES3.Load<Assignment>("dailyAssignment");

            if(ES3.KeyExists("completed"))
            {
                completed = ES3.Load<bool>("completed");
            }

            if (!completed)
            {
                if (ES3.KeyExists("kills"))
                {
                    kills = ES3.Load<int>("kills");
                }

                if (dailyAssignment.aminoObjective != 0 && dailyAssignment.aminoObjective == aminos.selectedAmino)
                {
                    partsCompleted += 1;
                }

                if (dailyAssignment.gunObjective != 0 && ES3.KeyExists("unlockedLists"))
                {
                    for(int i = 0; i < manager.items[0].Count; i++)
                    {
                        if(manager.items[0][i].selected && manager.items[0][i].aminoNumber == dailyAssignment.gunObjective)
                        {
                            partsCompleted += 1;
                        }
                    }
                }

                if (dailyAssignment.healthObjective != 0)
                {
                    partsCompleted += 1;
                }

                if (dailyAssignment.staminaObjective != 0)
                {
                    partsCompleted += 1;
                }

                if (dailyAssignment.noHealingObjective)
                {
                    partsCompleted += 1;
                }
            }
        }
    }

    void Update()
    {
        if(dailyAssignment != null && !completed)
        {
            if(dailyAssignment.scoreObjective != 0 && dailyAssignment.scoreObjective == manager.score)
            {
                partsCompleted += 1;
            }

            if (dailyAssignment.staminaObjective != 0 && manager.currentEnergy / manager.totalEnergy < dailyAssignment.staminaObjective && !lostStamina)
            {
                partsCompleted -= 1;
                lostStamina = true;
            }

            if(partsCompleted == dailyAssignment.partsNeeded && !completed)
            {
                completed = true;
                ES3.Save<int>("kills", 0);
                ES3.Save<bool>("newAssignments", true);
                StartCoroutine(WarnTextCo());
            }
        }
    }

    public void AddKills(int type)
    {
        if(dailyAssignment != null && dailyAssignment.enemyCountObjective != 0 && dailyAssignment.enemyCountObjective != kills && !completed)
        {
            if (dailyAssignment.enemyTypeObjective == 0)
            {
                kills += 1;
            }

            else if (type == dailyAssignment.enemyTypeObjective)
            {
                kills += 1;
            }

            if(kills == dailyAssignment.enemyCountObjective)
            {
                partsCompleted += 1;
            }
        }
    }

    public void CheckHealth(float healthPercentage, float damage)
    {
        if(dailyAssignment != null && !completed)
        {
            if (dailyAssignment.healthObjective != 0 && healthPercentage < dailyAssignment.healthObjective && !lostHealth)
            {
                partsCompleted -= 1;
                lostHealth = true;
            }

            if (dailyAssignment.noHealingObjective && damage < 0 && !lostHealing)
            {
                partsCompleted -= 1;
                lostHealing = true;
            }
        }
    }

    public void DecompleteItems()
    {
        if (dailyAssignment.aminoObjective != 0 && !completed)
        {
            partsCompleted -= 1;
        }

        if (dailyAssignment.gunObjective != 0 && !completed)
        {
            partsCompleted -= 1;
        }
    }

    IEnumerator WarnTextCo()
    {
        yield return new WaitUntil(() => !warn.running);
        warn.WarnText(2);
    }
}
