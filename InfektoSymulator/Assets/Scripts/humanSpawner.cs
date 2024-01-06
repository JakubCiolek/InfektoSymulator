using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class humanSpawner : MonoBehaviour
{

    public humanScript humanPrefab;
    public Renderer map;
    public float x = 0.5f;
    public float y = 0.5f;

    public bool restart = false;
    public int populationSize;
    public int infectedPopulationSize;
    public List<humanScript> humans;
    public Clock globalClock;
    private Bounds objectBounds;
    private Vector3 bottomLeftCorner;
    public InterfaceScritp simInterface;
    private Dictionary<string, float> paramatersDict;

    void Start()
    {
        objectBounds = map.bounds;
        bottomLeftCorner = new Vector3(objectBounds.min.x + x, objectBounds.min.y + y, 0f);
    }
    public void SimulationStart()
    {
        paramatersDict = simInterface.GetSimulationParameters();
        StartCoroutine(SpawnHumansWithDelay());
    }
    IEnumerator SpawnHumansWithDelay()
    {
        for (int i = 0; i < populationSize-infectedPopulationSize; i++)
        {
            if(restart)
            {
                break;
            }
            humanScript newHuman = Instantiate(humanPrefab, bottomLeftCorner, humanPrefab.transform.rotation).GetComponent<humanScript>();
            newHuman.Initialize(humanScript.Status.HEALTHY, map.bounds, globalClock, paramatersDict);
            humans.Add(newHuman);
            yield return new WaitForSeconds(0.2f);
        }

        for (int i = 0; i < infectedPopulationSize; i++)
        {
            if(restart)
            {
                break;
            }
            humanScript infectedHuman = Instantiate(humanPrefab, bottomLeftCorner, humanPrefab.transform.rotation).GetComponent<humanScript>();
            infectedHuman.Initialize(humanScript.Status.INFECTED, map.bounds, globalClock, paramatersDict);
            humans.Add(infectedHuman);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void deleteHumans()
    {
        GameObject[] humansToDelete = GameObject.FindGameObjectsWithTag("human");
        foreach(GameObject human in humansToDelete)
        {
            Destroy(human);
        }
    }
}
