using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButton : MonoBehaviour
{
    public int price;
    [SerializeField]
    int transactionType;
    [SerializeField]
    int skinNumber;
    Store store;

    void Start()
    {
        store = FindObjectOfType<Store>();
    }

    public void Action()
    {
        store.Buy(transactionType, skinNumber, price);
    }
}
