using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceScritp : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject population;
    public GameObject procentInfected;
    public GameObject populationImmunity;
    public GameObject virusSpreadFactor;
    public GameObject distanceToExpose;
    public GameObject timeToExpose;
    public GameObject simDuration;
    public GameObject simSpeed;
    public GameObject dayCounter;
    public GameObject hourCounter;
    private TMP_Text hourLabel;
    private TMP_Text dayLabel;
    public Clock clock;
    public humanSpawner spawner;
    private int exposed_counter = 0;
    private int infected_counter = 0;

    private bool simRunning = false;

    public Dictionary<String, float> simParameters;

        public float Population
    {
        get
        {
            if (simParameters.ContainsKey("population"))
            {
                return simParameters["population"];
            }
            return 1.0f;
        }
    }

    public float ProcentInfected
    {
        get
        {
            if (simParameters.ContainsKey("procentInfected"))
            {
                return simParameters["procentInfected"];
            }
            return 1.0f;
        }
    }

    public float PopulationImmunity
    {
        get
        {
            if (simParameters.ContainsKey("populationImmunity"))
            {
                return simParameters["populationImmunity"];
            }
            return 1.0f;
        }
    }

    public float VirusSpreadFactor
    {
        get
        {
            if (simParameters.ContainsKey("virusSpreadFactor"))
            {
                return simParameters["virusSpreadFactor"];
            }
            return 1.0f;
        }
    }

    public float DistanceToExpose
    {
        get
        {
            if (simParameters.ContainsKey("distanceToExpose"))
            {
                return simParameters["distanceToExpose"];
            }
            return 1.0f;
        }
    }

    public float TimeToExpose
    {
        get
        {
            if (simParameters.ContainsKey("timeToExpose"))
            {
                return simParameters["timeToExpose"];
            }
            return 1.0f;
        }
    }

    public float SimulationDuration
    {
        get
        {
            if (simParameters.ContainsKey("simulationDuration"))
            {
                return simParameters["simulationDuration"];
            }
            return 1.0f;
        }
    }
    void Start()
    {
        UpdateValueHints (procentInfected);
        UpdateValueHints (population);
        UpdateValueHints (populationImmunity);
        UpdateValueHints (virusSpreadFactor);
        UpdateValueHints (distanceToExpose);
        UpdateValueHints (timeToExpose);
        UpdateValueHints (simDuration);
        UpdateValueHints (simSpeed);

        hourLabel = hourCounter.GetComponentInChildren<TMP_Text>();
        dayLabel = dayCounter.GetComponentInChildren<TMP_Text>();

        population.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (population);});
        procentInfected.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (procentInfected);});
        populationImmunity.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (populationImmunity);});
        virusSpreadFactor.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (virusSpreadFactor);});
        distanceToExpose.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (distanceToExpose);});
        timeToExpose.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (timeToExpose);});
        simDuration.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (simDuration);});
        simSpeed.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {setSimSpeed (simSpeed);}); // TODO dać tak żeby dało się zmieniać w trakcie symulacji
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDays();
        UpdateHours();
    }

    public Dictionary<String, float> GetSimulationParameters()
    {
        var simParameters = new Dictionary<String, float>();
        simParameters["population"] = population.GetComponentInChildren<Slider>().value;
        simParameters["procentInfected"] = procentInfected.GetComponentInChildren<Slider>().value;
        simParameters["populationImmunity"] = populationImmunity.GetComponentInChildren<Slider>().value;
        simParameters["virusSpreadFactor"] = virusSpreadFactor.GetComponentInChildren<Slider>().value;
        simParameters["distanceToExpose"] = distanceToExpose.GetComponentInChildren<Slider>().value;
        simParameters["timeToExpose"] = timeToExpose.GetComponentInChildren<Slider>().value;
        simParameters["simulationDuration"] = simDuration.GetComponentInChildren<Slider>().value;
        return simParameters;
    }

    private void UpdateValueHints(GameObject sliderObject)
    {
        var slider = sliderObject.GetComponentInChildren<Slider>();
        var textBox = sliderObject.GetComponentInChildren<TMP_Text>();
        textBox.text = slider.value.ToString("F0");
    }

    public void setSimSpeed(GameObject sliderObject)
    {
        var slider = sliderObject.GetComponentInChildren<Slider>();
        var textBox = sliderObject.GetComponentInChildren<TMP_Text>();
        textBox.text = slider.value.ToString("F0");

    }

    private void UpdateHours()
    {
        int minutes = clock.GetMinutes();
        if(minutes >= 10)
        {
            hourLabel.text = clock.GetHour().ToString() +":"+minutes.ToString();
        }
        else
        {
            hourLabel.text = clock.GetHour().ToString() +":0"+minutes.ToString();
        }
    }

    private void UpdateDays()
    {
        dayLabel.text = clock.GetDay().ToString();
    }

    public void SimulationStart()
    {
        if(!simRunning)
        {
            simParameters = GetSimulationParameters();
            infected_counter = 0;
            exposed_counter = 0;
            clock.StartClock();
            spawner.populationSize = (int)simParameters["population"];
            int infectedpop = (int)(simParameters["population"]*(simParameters["procentInfected"]/100.0f));
            if (infectedpop > 1)
            {
                spawner.infectedPopulationSize = infectedpop;
            }
            else
            {
                spawner.infectedPopulationSize = 1;
            }
            spawner.SimulationStart();
            simRunning = true;
        }
    }

    public void SimulationStop()
    {
        simRunning = false;
        // TODO
    }

    public void SimulationPause()
    {
        // TODO
    }

    public void IncreaseExposed()
    {
        exposed_counter+=1;
    }

    public void IncreaseInfected()
    {
        infected_counter+=1;
    }
}
