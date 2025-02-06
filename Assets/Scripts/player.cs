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
        Debug.Log("FixedUpdate函数正在运行");
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(x, y);
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)//拾取物品的函数，如果不需要，可以注释掉
    {//这个函数没有调用，它是怎么运行的？
        Debug.Log("OnTriggerEnter2D函数正在运行");
        if (collision.tag== "Pickable")
        {
            Debug.Log("捡起物品");
            InventoryManager.Instance.AddToSeedBackpack(collision.GetComponent<Pickable>().type);
            Destroy(collision.gameObject);
        }
    }

}
