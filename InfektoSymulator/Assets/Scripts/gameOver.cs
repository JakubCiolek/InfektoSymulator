using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{

    public TMP_Text realTimeLabel;
    public TMP_Text ExposedLabel;
    public TMP_Text InfectedLabel;
    public void ShowGameOverScreen(string exposed, string infected, string time)
    {
        realTimeLabel.text = time;
        ExposedLabel.text = exposed;
        InfectedLabel.text = infected;
        gameObject.SetActive(true);
    }

    public void HideGameOverScreen()
    {
        gameObject.SetActive(false);
    }
}
