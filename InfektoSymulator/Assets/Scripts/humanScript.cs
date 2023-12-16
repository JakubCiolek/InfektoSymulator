using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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
    private int timeExposed;
    private int infectionTime;
    private int quarantineTime;

    


    private float immunity = 0.0f;
    public float simSpeed;
    public Clock clock;
    public NavMeshAgent agent;
    public Bounds floorBounds;
    public SpriteRenderer body;
    public SpriteRenderer range;
    public CircleCollider2D infectionTrigger;
    public InterfaceScritp simInterface;
    void Start()
    {
        checkStatus();
        simInterface = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceScritp>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(RandomFloorLocation());
        ScaleSpriteToColliderRadius();
        HideRange();
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

    public void SetMoveSpeed(float newMoveSpeed)
    {
        agent.speed = newMoveSpeed;
    }
    public void Initialize(Status initialStatus, Bounds floor, Clock globalClock, Dictionary<string, float> simParameters)
    {
        status = initialStatus;
        timeToInfection = simParameters["timeToExpose"];
        floorBounds = floor;
        clock = globalClock;
        infectionTrigger.radius = simParameters["distanceToExpose"];
        immunity =  UnityEngine.Random.Range((int)simParameters["populationImmunity"]-20, (int)simParameters["populationImmunity"]+20)/100.0f;
        if(immunity > 1.0f)
        {
            immunity = 1.0f;
        }
        else if(immunity < 0.0f)
        {
            immunity = 0.0f;
        }
        SetSimSpeed(simParameters["simulationSpeed"]);
        Debug.Log("timeToInfection "+timeToInfection);
    }

    private Vector3 RandomFloorLocation()
    {
        float randomX = UnityEngine.Random.Range(floorBounds.min.x, floorBounds.max.x);
        float randomY = UnityEngine.Random.Range(floorBounds.min.y, floorBounds.max.y);

        float randomZ = 0f;

        Vector3 randomPoint = new Vector3(randomX, randomY, randomZ);
        return randomPoint;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        humanScript otherHuman = collider.GetComponent<humanScript>();
        if(otherHuman.GetStatus() == Status.INFECTED && status==Status.HEALTHY)
        {
            ShowRange();
            timeEnter = Time.time; // Włącz pomiar czasu wejścia
            //Debug.Log("Zetknięto się z zarażonym. timeEnter: " + timeEnter);
        }
        if(status == Status.INFECTED && otherHuman.GetStatus() == Status.HEALTHY)
        {
            ShowRange();
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

            float contactDuration = timeExit - timeEnter;

            if (contactDuration > timeToInfection)
            {
                //Debug.Log("Status zmieniony na EXPOSED");
                status = Status.EXPOSED;
                body.color = Color.yellow;
                timeExposed = clock.GetHoursPassed();
                simInterface.IncreaseExposed();
            }
        }
        HideRange();
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

    void ScaleSpriteToColliderRadius()
    {
        // Pobierz aktualny rozmiar Collidera
        float colliderRadius = infectionTrigger.radius;

        // Ustaw nowy rozmiar sprite'a proporcjonalnie do rozmiaru Collidera
        Vector3 newScale = new Vector3(2*colliderRadius, 2*colliderRadius, 1f);
        range.transform.localScale = newScale;
    }

    void ShowRange()
    {
        range.color = new Color(range.color.r, range.color.g, range.color.b, 0.4f);
    }

    void HideRange()
    {
        range.color = new Color(range.color.r, range.color.g, range.color.b, 0.0f);
    }

    public void SetSimSpeed(float newSpeed)
    {
        simSpeed = newSpeed;
        agent.speed = 1.5f * simSpeed;
    }
}