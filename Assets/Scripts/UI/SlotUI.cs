using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour , IPointerClickHandler
{
    public int index = 0;
    private SlotData data;

    public Image iconImage;
    public TextMeshProUGUI countText;


    public void SetData(SlotData data)
    {
        this.data = data;
        data.AddListener(OnDataChange);

        UpdateUI();
    }
    public SlotData GetData()
    {
        return data;
    }
    private void OnDataChange()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (data.item == null)
        {
            iconImage.enabled = false;
            countText.enabled = false;
        }
        else
        {
            iconImage.enabled = true;
            countText.enabled = true;
            iconImage.sprite = data.item.sprite;//更新物品图标
            countText.text = data.count.ToString();//更新物品数量角标
        }
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("launch OnPointerClick");

        if (ItemMoveHandler.Instance != null && this != null)
        {
            ItemMoveHandler.Instance.OnSlotClick(this);
        }
        else
        {
            Debug.LogError($"NullReference: Instance={ItemMoveHandler.Instance}, SlotUI={this}");
        }
    }

    

}


