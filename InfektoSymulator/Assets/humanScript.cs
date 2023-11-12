using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class humanScript : MonoBehaviour
{

    public Tilemap room;
    public Rigidbody2D human;

    private Vector2 speed = Vector2.up*5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        human.velocity = speed;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Do something here");
        changeDirection();
    }

    void changeDirection()
    {
        speed = -1*speed;
    }
}
