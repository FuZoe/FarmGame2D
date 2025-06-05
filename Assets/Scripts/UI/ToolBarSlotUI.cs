using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarSlotUI : SlotUI  
{
    public Sprite slotLight;//路径未指定
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
