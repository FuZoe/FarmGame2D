using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu()]
public class InventoryData : ScriptableObject  //ScriptableObject��ʾ��Ϸ�رպ�ᱣ�����ݣ��´���Ϸ��ʼ�����
{
    public List<SlotData> slotList;//slotList��һ�����飬ÿ����Ʒ��һ��Ԫ��
}
