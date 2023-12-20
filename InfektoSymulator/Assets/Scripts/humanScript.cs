using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class humanScript : MonoBehaviour
{

    public enum Status
    {
        HEALTHY,
        EXPOSED,
        INFECTED
    }
    public enum Activity
    {
        RANDOM_WALK = 0,
        WORK = 1,
        BREAK = 2,
        LUNCH = 3,
        CONVERSATION = 4,
        IDLE = 5
    }
    private Status status;
    private Activity activity;
    private float timeEnter;  // Czas wejścia do kolizji
    private float timeExit;   // Czas wyjścia z kolizji
    private float timeToInfection = 1f;
    private float timer = 0f;
    private float interval = 2f;
    private float virusSpreadFactor;
    private float currentActivityStartTime;
    private float currentActivityEndTime;
    private int incubationPeriod;
    private int infectionTime;
    private int quarantineTime;
    private bool maskOn;
    private bool willBeInfected = false;
    private float immunity = 0.0f;
    public float simSpeed;
    public Clock clock;
    public NavMeshAgent agent;
    public Bounds floorBounds;
    public SpriteRenderer body;
    public SpriteRenderer range;
    public SpriteRenderer mask;
    public SpriteRenderer speechBubble;
    public CircleCollider2D infectionTrigger;
    public InterfaceScritp simInterface;
    private bool isConversing = false;
    private humanScript currentConvoHuman = null;
    private GameObject[] desks;

    private GameObject[] sofas;
    private GameObject[] kitchenSeats;
    private SeatScript currentSeat;
    void Start()
    {
    }

    void Update()
    {
        
        if(status == Status.EXPOSED)
        {
            CheckInfection();
        }
        else if (status == Status.INFECTED)
        {
            CheckQuarantine();
        }
        WorkNightCycle();
        if(activity == Activity.RANDOM_WALK)
        {
            Move();
        }
        else if(activity == Activity.CONVERSATION)
        {
            EndConversation();
        }
        else
        {
            EndActivity();
        }
    }

    void Move()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            agent.SetDestination(RandomFloorLocation());
            timer = 0f;
        }
    }

    private void WorkNightCycle()
    {
        if(simInterface.IsNight)
        {
            agent.ResetPath();
            activity = Activity.IDLE;
            body.color = new Color(body.color.r, body.color.g, body.color.b, 0.0f);
            HideMask();
            HideRange();
            HideSpeech();
        }
        else if(activity == Activity.IDLE)
        {
            activity = (humanScript.Activity)UnityEngine.Random.Range(0,4);
            body.color = new Color(body.color.r, body.color.g, body.color.b, 1.0f);
        }
    }

    private bool SearchSeat(GameObject[] seats)
    {
        foreach(GameObject obj in seats)
        {
            currentSeat = obj.GetComponent<SeatScript>();
            if(!currentSeat.GetOccupation())
            {
                agent.SetDestination(currentSeat.GetSeatPosition());
                currentSeat.SetOccupation(true);
                currentActivityEndTime = clock.GetHour() + UnityEngine.Random.Range(1,3);
                return true;
            }
        }
        return false;
    }

    private void EndActivity()
    {
        if(currentActivityEndTime < clock.GetHour())
        {
            currentSeat.SetOccupation(false);
            StartActivity();
        }
    }
    void StartActivity()
    {
        activity = (Activity)UnityEngine.Random.Range(0,4);
        bool seatFound = false;
        switch(activity)
        {
            case Activity.WORK:
                seatFound = SearchSeat(desks);
                break;
            case Activity.BREAK:
                seatFound = SearchSeat(sofas);
                break;
            case Activity.LUNCH:
                seatFound = SearchSeat(kitchenSeats);
                break;
            default:
                break;
        }
        if(!seatFound)
        {
            activity = Activity.RANDOM_WALK;
        }
    }

    private void Conversation()
    {
        if(Time.time > 4.0f)
        {
            ShowSpeech();
            activity = Activity.CONVERSATION;
            agent.ResetPath();
            currentActivityEndTime = Time.time + UnityEngine.Random.Range(1,4);
        }
        
    }

    private void EndConversation()
    {
        if(currentActivityEndTime < Time.time)
        {
            HideSpeech();
            StartActivity();
        }
        currentActivityStartTime = Time.time;
    }
    public void Initialize(Status initialStatus, Bounds floor, Clock globalClock, Dictionary<string, float> simParameters)
    {
        status = initialStatus;
        timeToInfection = simParameters["timeToExpose"];
        floorBounds = floor;
        clock = globalClock;
        infectionTrigger.radius = simParameters["distanceToExpose"];
        virusSpreadFactor = simParameters["virusSpreadFactor"];
        incubationPeriod = (int)simParameters["incubationPeriod"] * 24;
        immunity =  UnityEngine.Random.Range((int)simParameters["populationImmunity"]-20, (int)simParameters["populationImmunity"]+20)/100.0f;
        if(immunity > 1.0f)
        {
            immunity = 1.0f;
        }
        else if(immunity < 0.0f)
        {
            immunity = 0.0f;
        };
        int rand = UnityEngine.Random.Range(0, 25);
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
        CheckStatus();
        desks = GameObject.FindGameObjectsWithTag("desk");
        sofas = GameObject.FindGameObjectsWithTag("sofa");
        kitchenSeats = GameObject.FindGameObjectsWithTag("kitchenTop");
        simInterface = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceScritp>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(RandomFloorLocation());
        HideSpeech();
        HideRange();
        StartActivity();
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
        int conversation = UnityEngine.Random.Range(0,100);
        if (conversation == 1)
        {
            otherHuman.Conversation();
            Conversation();
        }
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

    void OnTriggerExit2D(Collider2D collider)
    {
        humanScript otherHuman = collider.GetComponent<humanScript>();
        if(otherHuman.GetStatus() == Status.INFECTED && status==Status.HEALTHY)
        {
            timeExit = Time.time;

            float contactDuration = timeExit - timeEnter;

            if (contactDuration > timeToInfection)
            {
                int exposed = UnityEngine.Random.Range(0,100);
                if(exposed < virusSpreadFactor)
                {
                    status = Status.EXPOSED;
                    body.color = Color.yellow;
                    simInterface.IncreaseExposed();
                    CalculateInfection();
                }
            }
        }
        HideRange();
    }
    public Status GetStatus()
    {
        return status;
    }

    private void CheckStatus()
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
                quarantineTime = 24;
                break;
            default:
                break;
        }
    }

    void ScaleSpriteToColliderRadius()
    {
        float colliderRadius = infectionTrigger.radius;
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

    void ShowSpeech()
    {
        speechBubble.color = new Color(speechBubble.color.r, speechBubble.color.g, speechBubble.color.b, 1.0f);
    }

    void HideSpeech()
    {
        speechBubble.color = new Color(speechBubble.color.r, speechBubble.color.g, speechBubble.color.b, 0.0f);
    }
    void CalculateInfection()
    {
        int infectionProbabilty = (int)((virusSpreadFactor/100.0f)*(1 - immunity)*100.0f);
        int attempt = UnityEngine.Random.Range(1,100);
        if(attempt < infectionProbabilty)
        {
            infectionTime = clock.GetHoursPassed() + UnityEngine.Random.Range(incubationPeriod-24, incubationPeriod+24);
            willBeInfected = true;
        }
        else
        {
            infectionTime = clock.GetHoursPassed() + 24;
            willBeInfected = false;
        }
    }
    void CheckInfection()
    {
        if(clock.GetHoursPassed() == infectionTime)
        {
            if(willBeInfected)
            {
                status = Status.INFECTED;
                body.color = Color.red;
                simInterface.IncreaseInfected();
                quarantineTime = clock.GetHoursPassed() + 24;
            }
            else
            {
                status = Status.HEALTHY;
                body.color = Color.black;
            }
        }
    }

    void CheckQuarantine()
    {
        if(clock.GetHoursPassed() == quarantineTime)
        {
            Destroy(gameObject);
        }
    }
}