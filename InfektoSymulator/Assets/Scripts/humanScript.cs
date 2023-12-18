using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;

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
    private float convDuration;
    private float convEnd;
    private int incubationPeriod;
    private int infectionTime;
    private int quarantineTime;
    private int beginWorkHour;
    private int endWorkHour;
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
    void Start()
    {
        CheckStatus();
        activity = Activity.RANDOM_WALK;
        simInterface = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceScritp>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(RandomFloorLocation());
        beginWorkHour = UnityEngine.Random.Range(8,11);
        endWorkHour = UnityEngine.Random.Range(16,18);
        HideSpeech();
        HideRange();
    }

    void Update()
    {
        HandleActivity();
        if(status == Status.EXPOSED)
        {
            CheckInfection();
        }
        else if (status == Status.INFECTED)
        {
            CheckQuarantine();
        }
        WorkNightCycle();
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

    private void HandleActivity()
    {
        switch(activity)
        {
            case Activity.RANDOM_WALK:
                    Move();
                break;
            case Activity.WORK:
                    Work();
                break;
            case Activity.BREAK:
                    Break();
                break;
            case Activity.LUNCH:
                    Lunch();
                break;
            case Activity.CONVERSATION:
                    EndConversation();
                break;
            default:
                break;
        }
    }

    private void WorkNightCycle()
    {
        if(simInterface.IsNight)
        {
            agent.ResetPath();
            activity = Activity.IDLE;
        }
        else if(activity == Activity.IDLE) 
        {
            //activity = (humanScript.Activity)UnityEngine.Random.Range(0,4);
            activity = Activity.RANDOM_WALK;
        }
    }

    private void Work()
    {

    }
    private void Break()
    {
        
    }

    private void Lunch()
    {
        
    }

    private void Conversation()
    {
        ShowSpeech();
        activity = Activity.CONVERSATION;
        agent.isStopped = true;
        convDuration = Time.time;
        convEnd = convDuration + UnityEngine.Random.Range(1,4);
    }

    private void EndConversation()
    {
        if(convEnd < convDuration)
        {
            agent.isStopped = false;
            HideSpeech();
            activity = Activity.RANDOM_WALK;
            convEnd = 0.0f;
            convDuration = 0.0f;
        }
        convDuration = Time.time;
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
        if (collider.CompareTag("human"))
        {
            HandleHumanTriggerEnter(collider);
        }
        else
        {
            //HandleOtherTriggerEnter(collider);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("human"))
        {
            HandleHumanTriggerExit(collider);
        }
    }

    void HandleHumanTriggerEnter(Collider2D collider)
    {
        // int conversation = UnityEngine.Random.Range(0,2);
        // if (conversation == 1 && isConversing == false)
        // {
        //     Conversation();
        //     humanScript convoHuman = collider.GetComponent<humanScript>();
        //     convoHuman.Conversation();
        //     Debug.Log("blablabla");

        //     isConversing = true;
        //     currentConvoHuman = convoHuman;
        // }
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

    void HandleHumanTriggerExit(Collider2D collider)
    {
        if (isConversing && currentConvoHuman == collider.GetComponent<humanScript>())
        {
            // Zakończ rozmowę lub wykonaj inne czynności
            isConversing = false;
            currentConvoHuman = null;
        }
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

    void ShowSpeech()
    {
        speechBubble.color = new Color(speechBubble.color.r, speechBubble.color.g, speechBubble.color.b, 1.0f);
        infectionTrigger.radius *= 0.3f;
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
                Debug.Log("Infection Time");
                status = Status.INFECTED;
                body.color = Color.red;
                simInterface.IncreaseInfected();
                quarantineTime = clock.GetHoursPassed() + 24;
            }
            else
            {
                Debug.Log("healthy Time");
                status = Status.HEALTHY;
                body.color = Color.black;
            }
        }
    }

    void CheckQuarantine()
    {
        if(clock.GetHoursPassed() == quarantineTime)
        {
            // first go to exit then destroy
            Destroy(gameObject);
        }
    }
}