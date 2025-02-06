using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlotData //һ���ֿ����
{
    public ItemData item;//��Ʒ��Ϣ
    public int count = 0;//��Ʒ����������Ĭ��0

    private Action Onchange;

    public bool CanAddItem()//�ж���Ʒ�Ƿ��ѵ���������ޡ����米��������999�����ܲ�������Ϊһ�����ӣ��ٻ�ú��ܲ�����ʱ������ռ��һ���¸���
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
    public void AddItem(ItemData item)//��������
    {
        this.item = item;
        count = 1;
        Onchange?.Invoke();
    }

    public void AddListener(Action Onchange)//һ������������������ʰȡ��Ʒ���߶�����Ʒʱ���ñ�������ˢ��
    {
        this.Onchange = Onchange; 
    }
}
