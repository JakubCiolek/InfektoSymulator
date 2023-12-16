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
    public GameObject maskProcentage;
    public GameObject incubationPeriod;
    public TMP_Text hourLabel;
    public TMP_Text dayLabel;
    public TMP_Text realTimeLabel;
    public TMP_Text ExposedLabel;
    public TMP_Text InfectedLabel;
    public Clock clock;
    public humanSpawner spawner;
    private int exposed_counter = 0;
    private int infected_counter = 0;
    private bool simRunning = false;

    private bool pause = true;

    public Dictionary<string, float> simParameters;

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
        UpdateValueHints (maskProcentage);
        UpdateValueHints (incubationPeriod);

        population.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (population);});
        procentInfected.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (procentInfected);});
        populationImmunity.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (populationImmunity);});
        virusSpreadFactor.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (virusSpreadFactor);});
        distanceToExpose.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (distanceToExpose);});
        timeToExpose.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (timeToExpose);});
        simDuration.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (simDuration);});
        maskProcentage.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (maskProcentage);});
        incubationPeriod.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {UpdateValueHints (incubationPeriod);});
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
        simParameters["simulationSpeed"] = simSpeed.GetComponentInChildren<Slider>().value;
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
        if(!pause)
        {
            Time.timeScale = slider.value;
        }
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

        int realSeconds = clock.GetRealSeconds();
        if(realSeconds >= 10)
        {
            realTimeLabel.text = clock.GetRealMinutes().ToString() +":"+ realSeconds.ToString();
        }
        else
        {
            realTimeLabel.text = clock.GetRealMinutes().ToString() +":0"+ realSeconds.ToString();
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
                InfectedLabel.text = infectedpop.ToString();
            }
            else
            {
                spawner.infectedPopulationSize = 1;
                InfectedLabel.text = "1";
            }
            spawner.SimulationStart();
            simRunning = true;
        }
        pause = false;
        setSimSpeed(simSpeed);
        clock.StartClock();
    }
    public void SimulationPause()
    {
        Time.timeScale = 0f;
        clock.Pause();
        pause = true;
    }

    public void SimulationRestart()
    {
        clock.ResetSimulationTime();
        clock.Pause();
        spawner.deleteHumans();
        simRunning = false;
    }

    public void IncreaseExposed()
    {
        exposed_counter+=1;
        ExposedLabel.text = exposed_counter.ToString();
    }

    public void IncreaseInfected()
    {
        infected_counter+=1;
        InfectedLabel.text = infected_counter.ToString();
    }
}
