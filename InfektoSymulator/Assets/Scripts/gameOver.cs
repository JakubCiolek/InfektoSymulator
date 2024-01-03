using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{

    public TMP_Text realTimeLabel;
    public TMP_Text ExposedLabel;
    public TMP_Text InfectedLabel;
    public TMP_Text population;
    public TMP_Text procentInfected;
    public TMP_Text populationImmunity;
    public TMP_Text virusSpreadFactor;
    public TMP_Text distanceToExpose;
    public TMP_Text timeToExpose;
    public TMP_Text simulationDuration;
    public TMP_Text maskProcentage;
    public TMP_Text incubationPeriod;
    public TMP_Text quarantineTime;
    public TMP_Text maskEffectivness;
    public void ShowGameOverScreen(string exposed, string infected, string time, Dictionary<string, float> paramatersDict)
    {
        realTimeLabel.text = time;
        ExposedLabel.text = exposed;
        InfectedLabel.text = infected;
        gameObject.SetActive(true);
        population.text = population.text +" "+paramatersDict["population"].ToString("0.00");
        procentInfected.text = procentInfected.text +" "+paramatersDict["procentInfected"].ToString("0.00") + " %";
        populationImmunity.text = populationImmunity.text +" "+paramatersDict["populationImmunity"].ToString("0.00") + " %";
        virusSpreadFactor.text = virusSpreadFactor.text +" "+paramatersDict["virusSpreadFactor"].ToString("0.00");
        distanceToExpose.text = distanceToExpose.text +" "+paramatersDict["distanceToExpose"].ToString("0.00")+ " m";
        timeToExpose.text = timeToExpose.text +" "+paramatersDict["timeToExpose"].ToString("0.00")+ " min";
        simulationDuration.text = simulationDuration.text +" "+paramatersDict["simulationDuration"].ToString("0.00") + " dni";
        maskProcentage.text = maskProcentage.text +" "+paramatersDict["maskProcentage"].ToString("0.00")+ " %";
        incubationPeriod.text = incubationPeriod.text +" "+paramatersDict["incubationPeriod"].ToString("0.00") + " dni";
        quarantineTime.text = quarantineTime.text +" "+paramatersDict["quarantineTime"].ToString("0.00") + " dni";
        maskEffectivness.text = maskEffectivness.text +" "+paramatersDict["maskEffectivness"].ToString("0.00") + " %";
    }

    public void HideGameOverScreen()
    {
        gameObject.SetActive(false);
    }
}
