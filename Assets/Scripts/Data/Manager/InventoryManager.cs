using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    //Debug.Log("1");

    //在这里不能用Debug.Log，这个函数只能在Awake或者Start，Update中调用

    private Dictionary<ItemType,ItemData> ItemDataDict = new Dictionary<ItemType,ItemData>();//空的字典

    public InventoryData Seedbackpack;//主角背包
    public InventoryData toolbarData;//工具栏

    private void Awake()
    {
        //Debug.Log("2");
        Instance = this; //对Instance实例化了呀
        Init();//用于初始化字典和数组
    }

    public void Init() //用于初始化字典和数组
    {
        ItemData[] ItemDataArray = Resources.LoadAll<ItemData>("Data");
        foreach(ItemData data in ItemDataArray)
        {
            ItemDataDict.Add(data.type, data);
        }

        Seedbackpack = Resources.Load<InventoryData>("Data/YourBags/YourSeeds");
        toolbarData = Resources.Load<InventoryData>("Data/YourBags/Toolbar");

    }
    //Assets/Resources/Data/YourBags/YourSeeds.asset
    private ItemData GetItemData(ItemType type)  //用于获取物品信息
    {
        ItemData data;
        bool isSuccess = ItemDataDict.TryGetValue(type, out data);
        if (isSuccess)
        {
            return data;
        }
        else
        {
            Debug.LogWarning("(Bug)你传递的type:" + type + "不存在，无法得到物品信息");
            return null;
        }
            
    }

    public void AddToSeedBackpack(ItemType type)//捡到物品并把物品放入背包
    {
        ItemData item = GetItemData(type);
        //我服了，这个bug修了好久。结果是因为我把大小写打错了，Slotdata是一个类，而slotdata是一个示例。之前打成了Slotdata
        foreach(SlotData slotData in Seedbackpack.slotList)
        {
            if (slotData.item == item    && slotData.CanAddItem() )//如果背包里已经含有同类物品，并且该物品没有达到数量上限//我怒了，为什么编译错误
            {
                slotData.AddOne();
                return;//物品所对应的格子 下标加1就可以了
            }
        }

        foreach(SlotData slotData in Seedbackpack.slotList)
        {
            if(slotData.count==0)////如果背包里不含有同类物品 或者 物品达到数量上限
            {
                slotData.AddItem(item);return;
            }
        }

        //如果以上两种都没有return
        Debug.LogWarning("无法拾取物品！因为你种子背包的" + Seedbackpack + "栏位已满。");
    }    

}
