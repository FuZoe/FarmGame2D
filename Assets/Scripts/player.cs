using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;

    private Animator anim;

    private Vector2 direction = Vector2.zero;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        


        if(direction.magnitude > 0) 
        {
            anim.SetBool("IsWalking", true);
            anim.SetFloat("horizontal", direction.x);
            anim.SetFloat("vertical", direction.y);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }

        
    }

    private void FixedUpdate()
    {
        Debug.Log("FixedUpdate������������");
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(x, y);
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)//ʰȡ��Ʒ�ĺ������������Ҫ������ע�͵�
    {//�������û�е��ã�������ô���еģ�
        Debug.Log("OnTriggerEnter2D������������");
        if (collision.tag== "Pickable")
        {
            Debug.Log("������Ʒ");
            InventoryManager.Instance.AddToSeedBackpack(collision.GetComponent<Pickable>().type);
            Destroy(collision.gameObject);
        }
    }

}
