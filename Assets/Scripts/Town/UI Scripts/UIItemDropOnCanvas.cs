using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UIItemDropOnCanvas : MonoBehaviour
{
    private Rigidbody2D rd;
    private float xdir;
    private float ydir;
    private const float speed = 50000;
    private float totalTime = 0;
    private const float endTime = 1;
    private Vector3 itemButtonPosition;
    public GameObject ItemImage;

    public void Initialize(int itemId, Vector3 startPosition)
    {
        ItemImage.GetComponent<Image>().sprite = ItemDataLoader.GetSpriteByItemId(itemId);
        transform.position = startPosition;
    }
    // Start is called before the first frame update
    void Start()
    {
        itemButtonPosition = GameObject.Find("Button_Inventory").transform.position;
        xdir = UnityEngine.Random.value * 2 - 1;
        ydir = UnityEngine.Random.value * 2 - 1;
        rd = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        Vector2  force;
        if(totalTime < endTime)
        {
            if (totalTime < endTime * 0.25)
            {
                force = new Vector2(xdir * speed * Time.deltaTime , ydir * speed * Time.deltaTime );
            }
            else if (totalTime < endTime * 0.75)
            {
                force = new Vector2(-xdir * speed * Time.deltaTime * 0.3f, -ydir * speed * Time.deltaTime * 0.3f);
            }
            else
            {
                force = new Vector2(0,0);
                rd.totalForce = force;
            }
        }
        else
        {
            force = Vector2.MoveTowards(transform.position, itemButtonPosition, speed);
            if (transform.position.x > itemButtonPosition.x)
            {
                Destroy(gameObject);
            }
        }
        rd.AddForce(force);
    }
}
