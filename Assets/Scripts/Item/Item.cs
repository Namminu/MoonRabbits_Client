using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private int itemCode;
    private Image itemImage;
    private ItemData itemData;


    public Image GetItemImage() { return itemImage; } 

    public ItemData GetItemData() { return itemData; }
}
