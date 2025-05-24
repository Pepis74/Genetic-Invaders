using UnityEngine;

[CreateAssetMenu(fileName = "New Assignment", menuName = "My Stuff/Assignment")]
[System.Serializable]
public class Assignment : ScriptableObject
{
    public Sprite icon;
    public int scoreObjective;
    public int aminoObjective;
    public int gunObjective;
    public int enemyCountObjective;
    public int enemyTypeObjective;
    public float healthObjective;
    public float staminaObjective;
    public bool noHealingObjective;
    public int reward;
    public bool singleGame;
    public int partsNeeded;
    public int assignmentNumber;
}
