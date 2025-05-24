using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "My Stuff/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public Sprite icon;
    public bool selected;
    public float unlockNumber;
    public float currentUnlockNumber;
    public int type;
    public int aminoNumber;
    public GameObject collider;
    public GameObject propellers;
    public GameObject projectile;
    public Color color1;
    public Color color2;
}
