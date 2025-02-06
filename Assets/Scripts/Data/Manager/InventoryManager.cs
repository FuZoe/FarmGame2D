using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    //Debug.Log("1");

    //�����ﲻ����Debug.Log���������ֻ����Awake����Start��Update�е���

    private Dictionary<ItemType,ItemData> ItemDataDict = new Dictionary<ItemType,ItemData>();//�յ��ֵ�

    public InventoryData Seedbackpack;//���Ǳ���
    public InventoryData toolbarData;//������

    private void Awake()
    {
        //Debug.Log("2");
        Instance = this; //��Instanceʵ������ѽ
        Init();//���ڳ�ʼ���ֵ������
    }

    public void Init() //���ڳ�ʼ���ֵ������
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
    private ItemData GetItemData(ItemType type)  //���ڻ�ȡ��Ʒ��Ϣ
    {
        ItemData data;
        bool isSuccess = ItemDataDict.TryGetValue(type, out data);
        if (isSuccess)
        {
            return data;
        }
        else
        {
            Debug.LogWarning("(Bug)�㴫�ݵ�type:" + type + "�����ڣ��޷��õ���Ʒ��Ϣ");
            return null;
        }
            
    }

    public void AddToSeedBackpack(ItemType type)//����Ʒ������Ʒ���뱳��
    {
        ItemData item = GetItemData(type);
        //�ҷ��ˣ����bug���˺þá��������Ϊ�ҰѴ�Сд����ˣ�Slotdata��һ���࣬��slotdata��һ��ʾ����֮ǰ�����Slotdata
        foreach(SlotData slotData in Seedbackpack.slotList)
        {
            if (slotData.item == item    && slotData.CanAddItem() )//����������Ѿ�����ͬ����Ʒ�����Ҹ���Ʒû�дﵽ��������//��ŭ�ˣ�Ϊʲô�������
            {
                slotData.AddOne();
                return;//��Ʒ����Ӧ�ĸ��� �±��1�Ϳ�����
            }
        }

        foreach(SlotData slotData in Seedbackpack.slotList)
        {
            if(slotData.count==0)////��������ﲻ����ͬ����Ʒ ���� ��Ʒ�ﵽ��������
            {
                slotData.AddItem(item);return;
            }
        }

        //����������ֶ�û��return
        Debug.LogWarning("�޷�ʰȡ��Ʒ����Ϊ�����ӱ�����" + Seedbackpack + "��λ������");
    }    

}
