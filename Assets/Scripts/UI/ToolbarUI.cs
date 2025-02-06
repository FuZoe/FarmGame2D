using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarUI : MonoBehaviour
{

    public List<ToolBarSlotUI> slotuiList;
    private ToolBarSlotUI selectedSlotUI;


    // Start is called before the first frame update
    void Start()
    {
        InitUI();
    }

    private void Update()
    {
        for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha9; i++)
        {
            if(Input.GetKeyDown( (KeyCode)i )  )
            {
                if (selectedSlotUI != null)
                {
                    selectedSlotUI.UnHighlight();
                }
                int index = i - (int)KeyCode.Alpha1;
                selectedSlotUI = slotuiList[index];
                selectedSlotUI.Highlight();
            }
        }
    }

    void InitUI()
    {
        slotuiList = new List<ToolBarSlotUI>(new ToolBarSlotUI[9]);

        ToolBarSlotUI[] slotuiArray = transform.GetComponentsInChildren<ToolBarSlotUI>();
        
        foreach (ToolBarSlotUI slotui in slotuiArray)
        {
            slotuiList[slotui.index] = slotui;
        }

        UpdateUI();
    }
    
    public void UpdateUI()
    {

        List<SlotData> slotdataList = InventoryManager.Instance.toolbarData.slotList;
                                                                                      
                                                                                      

        for (int i = 0; i < slotdataList.Count; i++)
        {
            slotuiList[i].SetData(slotdataList[i]);
        }
    }
}
