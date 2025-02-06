using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType //ö��������Ʒ����
{
    None,//Ĭ��Ϊ��
    /*money_RMB,
    money_goldcoin,
    money_diamond,*/
    seed_carrot,
    seed_tomato
    //��������һ��Ԫ�ز��Ӷ��ţ�Ҳ�Ǻܷ�����
}


[CreateAssetMenu()]
public class ItemData :ScriptableObject
{
    public ItemType type = ItemType.None;
    public Sprite sprite;
    public GameObject prefab;
    public int maxCount = 999;//������Ʒ�������Ĭ��Ϊ999���ر�Ŀ��Ե�

}
