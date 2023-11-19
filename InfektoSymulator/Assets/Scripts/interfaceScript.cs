using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class interfaceScritp : MonoBehaviour
{
    // Start is called before the first frame update

    public Slider population;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<String, float> GetSimulationParameters()
    {
        var simParameters = new Dictionary<String, float>();
        simParameters["population"] = population.value;
        //TODO : read parameters and put them in dictionary.
        return simParameters;
    }
}
