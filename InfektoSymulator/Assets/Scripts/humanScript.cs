using System.Collections.Generic;
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
    private int timeExposed;
    private int infectionTime;
    private int quarantineTime;
    private bool maskOn;

    private float immunity = 0.0f;
    public float simSpeed;
    public Clock clock;
    public NavMeshAgent agent;
    public Bounds floorBounds;
    public SpriteRenderer body;
    public SpriteRenderer range;
    public SpriteRenderer mask;
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
        };
        int rand = UnityEngine.Random.Range(0, 100);
        maskOn = rand < (int)simParameters["maskProcentage"];
        if(maskOn)
        {
            ShowMask();
        }
        else
        {
            HideMask();
        }
        ScaleSpriteToColliderRadius();
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
            timeEnter = Time.time;
        }
        if(status == Status.INFECTED && otherHuman.GetStatus() == Status.HEALTHY)
        {
            ShowRange();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        humanScript otherHuman = collider.GetComponent<humanScript>();
        if(otherHuman.GetStatus() == Status.INFECTED && status==Status.HEALTHY)
        {
            timeExit = Time.time;

            float contactDuration = timeExit - timeEnter;

            if (contactDuration > timeToInfection)
            {
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

    void ShowMask()
    {
        mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 1.0f);
        infectionTrigger.radius *= 0.3f;
    }

    void HideMask()
    {
        mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, 0.0f);
    }
}