using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu()]
public class InventoryData : ScriptableObject  //ScriptableObject表示游戏关闭后会保存数据，下次游戏开始会继续
{
    public List<SlotData> slotList;//slotList是一个数组，每个物品是一个元素
}
