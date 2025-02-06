using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackPackUI : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject ParentUI;

    public List<SlotUI> slotuiList;

    private void Awake()
    {
        ParentUI = transform.Find("ParentUI").gameObject;
    }

    private void Start()
    {
        InitUI();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))//玩家按下Tab键显示/隐藏背包
        {
            OnCloseClick();
            //ToggleUI();
        }
    }

    void InitUI()
    {
        slotuiList = new List<SlotUI>(new SlotUI[40]);

        //UnityEditor.EditorUtility.DisplayDialog("标题", "原神启动？", "确认", "取消");
        SlotUI[] slotuiArray = transform.GetComponentsInChildren<SlotUI>();//一般这个其实已经就是排好的顺序了
        foreach (SlotUI slotui in slotuiArray)//保险起见，用index再来排序一遍（index是SlotUI.cs里面的变量）
        {
            slotuiList[slotui.index]=slotui; 
        }
        //论BUG是如何修复的：点击了一下U3D引擎上面的启动键，运行了一下游戏
    }

    public void UpdateUI()//把inventorymanager里面的数据显示在游戏背包UI界面中
    {
        //Debug.Log("3");
        List<SlotData> slotdataList = InventoryManager.Instance.Seedbackpack.slotList;//完蛋了，这行代码报错了
        //哈哈！我修复了！原来问题不在Instance，而在Seedbackpack上，我指定错了路径
        //如今我修改为了Seedbackpack="Data/YourBags/YourSeeds"，就不报错了！
        
        /*反思一下自己为什么不会修：
         * Instance是什么，类对象？我有点没看懂Instance的声明*/
        /*在进行unity游戏制作的C#代码编写时，会遇到“NullReferenceException: Object reference not set to an instance of an object”
         * 这样的错误提示。
         * 错误的意思翻译过来是“未将对象引用到对象的实例”，
         * 意思就是声明了对象，却没有给它赋值，导致运行的时候依旧是null值。
         * 一定要核对控件名的拼写，确保正确。这类错误百分五十都是因为拼写控件名错误导致的。*/


        for (int i=0;i< slotdataList.Count; i++)
        {
            slotuiList[i].SetData(slotdataList[i]);
        }
    }




    private void ToggleUI()//这个函数用于显示/隐藏背包
    {
        ParentUI.SetActive(!ParentUI.activeSelf);
    }

    public void OnCloseClick()//关闭按钮索引到此。按下关闭按钮后，关闭背包
    {//问题是不是toggleUI函数的问题？可能需要修改函数参数？
        //我他爸的升级了一下Unity的Editor,然后bug自己修复了
        ToggleUI();
        Debug.Log("打开/关闭背包");
    }

    public void BBD()
    {
        //这个函数只是用于记录修bug的数次崩溃
    }
}
