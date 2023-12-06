using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class interfaceScritp : MonoBehaviour
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
    void Start()
    {
        population.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {updateValueHints (population);});
        procentInfected.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {updateValueHints (procentInfected);});
        populationImmunity.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {updateValueHints (populationImmunity);});
        virusSpreadFactor.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {updateValueHints (virusSpreadFactor);});
        distanceToExpose.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {updateValueHints (distanceToExpose);});
        timeToExpose.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {updateValueHints (timeToExpose);});
        simDuration.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {updateValueHints (simDuration);});
        simSpeed.GetComponentInChildren<Slider>().onValueChanged.AddListener (delegate {updateValueHints (simSpeed);});
    }

    // Update is called once per frame
    void Update()
    {
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
        //TODO : read parameters and put them in dictionary.
        return simParameters;
    }

    private void updateValueHints(GameObject sliderObject)
    {
        var slider = sliderObject.GetComponentInChildren<Slider>();
        var textBox = sliderObject.GetComponentInChildren<TMP_Text>();
        textBox.text = slider.value.ToString("F" + 2);
    }

}
