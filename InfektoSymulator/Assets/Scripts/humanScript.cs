using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class humanScript : MonoBehaviour
{

    public enum Status
    {
        HEALTHY,
        EXPOSED,
        INFECTED
    }
    private Status status;
    private float timeEnter;  // Czas wejścia do kolizji
    private float timeExit;   // Czas wyjścia z kolizji
    private float timeToInfection = 1f;
    private float timer = 0f;
    private float interval = 2f;
    public NavMeshAgent agent;
    Renderer floor;

    public SpriteRenderer body;
    void Start()
    {
        checkStatus();
        floor =  GameObject.FindGameObjectWithTag("floor").GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(RandomFloorLocation());
    }

    void Update()
    {
        move();
    }

    void move()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            agent.SetDestination(RandomFloorLocation());
            timer = 0f;
        }
    }

    public void Initialize(Status initialStatus, float choosenTimeToInfection)
    {
        status = initialStatus;
        timeToInfection = choosenTimeToInfection;
    }

    private Vector3 RandomFloorLocation()
    {
        Bounds floorBounds = floor.bounds;

        float randomX = Random.Range(floorBounds.min.x, floorBounds.max.x);
        float randomY = Random.Range(floorBounds.min.y, floorBounds.max.y);

        float randomZ = 0f;

        Vector3 randomPoint = new Vector3(randomX, randomY, randomZ);
        return randomPoint;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        humanScript otherHuman = collider.GetComponent<humanScript>();
        if(otherHuman.GetStatus() == Status.INFECTED && status==Status.HEALTHY)
        {
            timeEnter = Time.time; // Włącz pomiar czasu wejścia
            //Debug.Log("Zetknięto się z zarażonym. timeEnter: " + timeEnter);
        }
    }

    // Gets called during the collision
    void OnTriggerStay2D(Collider2D collider)
    {
        //Debug.Log("stay");
    }

    // Gets called when the object exits the collision
    void OnTriggerExit2D(Collider2D collider)
    {
        humanScript otherHuman = collider.GetComponent<humanScript>();
        if(otherHuman.GetStatus() == Status.INFECTED && status==Status.HEALTHY)
        {
            timeExit = Time.time; // Włącz pomiar czasu wyjścia
            //Debug.Log("Zakończono kontakt. timeExit: " + timeExit);
            float contactDuration = timeExit - timeEnter;
           //Debug.Log("Czas trwania kontaktu: " + contactDuration);
            if (contactDuration > timeToInfection)
            {
                //Debug.Log("Status zmieniony na EXPOSED");
                status = Status.EXPOSED;
                body.color = Color.yellow;
            }
        }
    }

    public Status GetStatus()
    {
        return status;
    }

    private void checkStatus()
    {
        switch(status){
            case Status.HEALTHY:
                body.color = Color.black;
                break;
            case Status.EXPOSED:
                body.color = Color.yellow;
                break;
            case Status.INFECTED:
                body.color = Color.red;
                break;
            default:
                break;
        }
    }
}