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
        population.text = "Populacja: "+ paramatersDict["population"].ToString("0.00");
        procentInfected.text =  "Procent zarażonych: "+paramatersDict["procentInfected"].ToString("0.00") + " %";
        populationImmunity.text = "Odporność populacji: "+paramatersDict["populationImmunity"].ToString("0.00") + " %";
        virusSpreadFactor.text = "Zarażalność patogenu: "+paramatersDict["virusSpreadFactor"].ToString("0.00");
        distanceToExpose.text = "Dystans do zarażenia: "+paramatersDict["distanceToExpose"].ToString("0.00")+ " m";
        timeToExpose.text = "Czas do zarażenia: "+paramatersDict["timeToExpose"].ToString("0.00")+ " min";
        simulationDuration.text = "Długośc symulacji: "+paramatersDict["simulationDuration"].ToString("0.00") + " dni";
        maskProcentage.text = "Procent w maseczkach: "+paramatersDict["maskProcentage"].ToString("0.00")+ " %";
        incubationPeriod.text = "Średni okres inkubacji: "+paramatersDict["incubationPeriod"].ToString("0.00") + " dni";
        quarantineTime.text = "Średni czas wykrycia: "+paramatersDict["quarantineTime"].ToString("0.00") + " dni";
        maskEffectivness.text = "Skuteczność maseczek: "+paramatersDict["maskEffectivness"].ToString("0.00") + " %";
    }

    public void HideGameOverScreen()
    {
        gameObject.SetActive(false);
    }
}
