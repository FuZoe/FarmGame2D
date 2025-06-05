using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMoveHandler : MonoBehaviour

{
    //单例模式
    public static ItemMoveHandler Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public void OnSlotClick(SlotUI slotui)
    {
        Debug.Log("Lauch OnSlotClick");
        print("OnClick" + slotui.name);
    }


}
