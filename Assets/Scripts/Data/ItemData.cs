using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType //枚举所有物品名称
{
    None,//默认为空
    /*money_RMB,
    money_goldcoin,
    money_diamond,*/
    seed_carrot,
    seed_tomato
    //数组的最后一个元素不加逗号
}


[CreateAssetMenu()]
public class ItemData :ScriptableObject
{
    public ItemType type = ItemType.None;
    public Sprite sprite;
    public GameObject prefab;
    public int maxCount = 999;//任意物品最大数量默认为999，特别的可以调

}
