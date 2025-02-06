using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlotData //一个仓库格子
{
    public ItemData item;//物品信息
    public int count = 0;//物品持有数量，默认0

    private Action Onchange;

    public bool CanAddItem()//判断物品是否已到达叠加上限。例如背包里有了999个胡萝卜种子作为一个格子，再获得胡萝卜种子时会另外占用一个新格子
    {
        if (count < item.maxCount)
            return true;
        else return false;
    }
    public void AddOne()
    {
        count++;
        Onchange?.Invoke();
    }
    public void AddItem(ItemData item)//新增格子
    {
        this.item = item;
        count = 1;
        Onchange?.Invoke();
    }

    public void AddListener(Action Onchange)//一个监听函数，在主角拾取物品或者丢弃物品时，让背包进行刷新
    {
        this.Onchange = Onchange; 
    }
}
