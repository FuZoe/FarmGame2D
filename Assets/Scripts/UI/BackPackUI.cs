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
        if (Input.GetKeyDown(KeyCode.Tab))//��Ұ���Tab����ʾ/���ر���
        {
            OnCloseClick();
            //ToggleUI();
        }
    }

    void InitUI()
    {
        slotuiList = new List<SlotUI>(new SlotUI[40]);

        //UnityEditor.EditorUtility.DisplayDialog("����", "ԭ��������", "ȷ��", "ȡ��");
        SlotUI[] slotuiArray = transform.GetComponentsInChildren<SlotUI>();//һ�������ʵ�Ѿ������źõ�˳����
        foreach (SlotUI slotui in slotuiArray)//�����������index��������һ�飨index��SlotUI.cs����ı�����
        {
            slotuiList[slotui.index]=slotui; 
        }
        //��BUG������޸��ģ������һ��U3D�����������������������һ����Ϸ
    }

    public void UpdateUI()//��inventorymanager�����������ʾ����Ϸ����UI������
    {
        //Debug.Log("3");
        List<SlotData> slotdataList = InventoryManager.Instance.Seedbackpack.slotList;//�군�ˣ����д��뱨����
        //���������޸��ˣ�ԭ�����ⲻ��Instance������Seedbackpack�ϣ���ָ������·��
        //������޸�Ϊ��Seedbackpack="Data/YourBags/YourSeeds"���Ͳ������ˣ�
        
        /*��˼һ���Լ�Ϊʲô�����ޣ�
         * Instance��ʲô����������е�û����Instance������*/
        /*�ڽ���unity��Ϸ������C#�����дʱ����������NullReferenceException: Object reference not set to an instance of an object��
         * �����Ĵ�����ʾ��
         * �������˼��������ǡ�δ���������õ������ʵ������
         * ��˼���������˶���ȴû�и�����ֵ���������е�ʱ��������nullֵ��
         * һ��Ҫ�˶Կؼ�����ƴд��ȷ����ȷ���������ٷ���ʮ������Ϊƴд�ؼ��������µġ�*/


        for (int i=0;i< slotdataList.Count; i++)
        {
            slotuiList[i].SetData(slotdataList[i]);
        }
    }




    private void ToggleUI()//�������������ʾ/���ر���
    {
        ParentUI.SetActive(!ParentUI.activeSelf);
    }

    public void OnCloseClick()//�رհ�ť�������ˡ����¹رհ�ť�󣬹رձ���
    {//�����ǲ���toggleUI���������⣿������Ҫ�޸ĺ���������
        //�����ֵ�������һ��Unity��Editor,Ȼ��bug�Լ��޸���
        ToggleUI();
        Debug.Log("��/�رձ���");
    }

    public void BBD()
    {
        //�������ֻ�����ڼ�¼��bug�����α���
    }
}
