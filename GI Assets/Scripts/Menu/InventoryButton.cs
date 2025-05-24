using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    public Item item;
    public bool description;
    [SerializeField]
    int claimNumber;
    InventoryManager manager;
    AssignmentMenu assignment;
    
    void Start()
    {
        manager = FindObjectOfType<InventoryManager>();
        assignment = FindObjectOfType<AssignmentMenu>();
    }

    public void ButtonPress()
    {
        manager.IconPress(item, gameObject);
    }

    public void LockedPress()
    {
        manager.LockedPress(item, description);
    }

    public void AssignmentPress()
    {
        assignment.InventoryPress(item, description, gameObject);
    }
    
    public void ProgressPress()
    {
        assignment.ProgressClaim(item, claimNumber);
    }
}
