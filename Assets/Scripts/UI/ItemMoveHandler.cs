using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemMoveHandler : MonoBehaviour

{

    //单例模式
    public static ItemMoveHandler Instance { get; private set; }

    private Image icon;
    //private SlotUI selectedSlotUI;
    private SlotData selectedSlotData;
    private Player player;

    public void Awake()
    {
        Instance = this;
        icon = GetComponentInChildren<Image>();
        HideIcon();
        player = GameObject.FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        if (icon.enabled)
        {
            Vector2 Position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(),
                Input.mousePosition,
                null,
                out Position);
            icon.GetComponent<RectTransform>().anchoredPosition = Position;

        }

        if (Input.GetMouseButtonDown(0))//丢弃物品
        {

            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                ThrowItem();
            }
        }
    }

    public void OnSlotClick(SlotUI slotui)
    {
        Debug.Log("Lauch OnSlotClick");
        print("OnClick" + slotui.name);
        //selectedSlotUI = slotui;
        selectedSlotData = slotui.GetData();
        ShowIcon(selectedSlotData.item.sprite);
    }

    void HideIcon()
    {
        if (icon != null)
        {
            icon.enabled = false;
        }
    }

    void ShowIcon(Sprite sprite)
    {
        if (icon != null)
        {
            icon.enabled = true;
            icon.sprite = sprite;
        }
    }
    void Clear()
    {
        HideIcon();
        selectedSlotData = null;
    }    
    private void ThrowItem()
    {
        Debug.Log("Throw Item");
        GameObject prefab = selectedSlotData.item.prefab;
        int count = selectedSlotData.count;
        player.ThrowItem(prefab, count);
        selectedSlotData.Clear();
        Clear();
        
    }
}
