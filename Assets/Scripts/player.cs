using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
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
        Debug.Log("FixedUpdate is running");
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(x, y);
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)//拾取物品的函数，如果不需要，可以注释掉
    {
        Debug.Log("OnTriggerEnter2D is running");
        if (collision.tag== "Pickable")
        {
            Debug.Log("捡起物品");
            InventoryManager.Instance.AddToSeedBackpack(collision.GetComponent<Pickable>().type);
            Destroy(collision.gameObject);
        }
    }

    public void ThrowItem(GameObject itemPrefab,int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject abdItem = GameObject.Instantiate(itemPrefab);
            Vector2 direction = Random.insideUnitCircle * 1.2f;
            abdItem.transform.position = transform.position + new Vector3(direction.x, direction.y, 0f);
        }
            
    }

}
