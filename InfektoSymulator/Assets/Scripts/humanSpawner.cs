using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class humanSpawner : MonoBehaviour
{

    public GameObject human;
    public Renderer map;
    // Start is called before the first frame update
    public float x;
    public float y;

    public int populationSize;
    void Start()
    {
        Bounds objectBounds = map.bounds;

            // Pobierz lewy dolny róg granic
        Vector3 bottomLeftCorner = new Vector3(objectBounds.min.x + x, objectBounds.min.y + y, 0f);
        for(int i =0 ; i<=populationSize;i++)
        {
            Instantiate(human, bottomLeftCorner, human.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
