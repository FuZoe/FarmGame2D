using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMoveHandler : MonoBehaviour

{
    //����ģʽ
    public static ItemMoveHandler Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public void OnSlotClick(SlotUI slotui)
    {
        Debug.Log("զ��զ�ľ��ǲ����2");
        print("OnClick" + slotui.name);//զ��զ�ľ��ǲ����
    }


}
