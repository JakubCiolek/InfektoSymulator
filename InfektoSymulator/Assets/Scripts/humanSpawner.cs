using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class humanSpawner : MonoBehaviour
{

    public humanScript humanPrefab;
    public Renderer map;
    public float x;
    public float y;
    public int populationSize;
    public int infectedPopulationSize;
    public List<humanScript> humans;

    public Clock globalClock;

    private Bounds objectBounds;
    private Vector3 bottomLeftCorner;
    private float choosenTimeToInfection = 1f; //TODO : get from interface
    void Start()
    {
        objectBounds = map.bounds;

        // Pobierz lewy dolny róg granic
        bottomLeftCorner = new Vector3(objectBounds.min.x + x, objectBounds.min.y + y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SimulationStart()
    {
        StartCoroutine(SpawnHumansWithDelay());
    }
    IEnumerator SpawnHumansWithDelay()
    {
        for (int i = 0; i < populationSize-infectedPopulationSize; i++)
        {
            humanScript newHuman = Instantiate(humanPrefab, bottomLeftCorner, humanPrefab.transform.rotation).GetComponent<humanScript>();
            newHuman.Initialize(humanScript.Status.HEALTHY, choosenTimeToInfection, map.bounds, globalClock);
            humans.Add(newHuman);
            yield return new WaitForSeconds(0.2f);
        }

        for (int i = 0; i < infectedPopulationSize; i++)
        {
            humanScript infectedHuman = Instantiate(humanPrefab, bottomLeftCorner, humanPrefab.transform.rotation).GetComponent<humanScript>();
            infectedHuman.Initialize(humanScript.Status.INFECTED, choosenTimeToInfection, map.bounds, globalClock);
            humans.Add(infectedHuman);
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("humans array count " + humans.Count);
    }
}
