using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarSlotUI : SlotUI  //它是SlotUI的子类，SlotUI有的，它都有
{
    public Sprite slotLight;//路径还没指定啊
    public Sprite slotDark;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Highlight()
    {
        image.sprite = slotDark;
    }

    public void UnHighlight()
    {
        image.sprite = slotLight;
    }

}
