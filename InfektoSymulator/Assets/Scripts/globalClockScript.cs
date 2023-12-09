using UnityEngine;

public class Clock : MonoBehaviour
{
    private int day = 1;
    private int hour = 8;
    private int hoursPassed = 0;
    private float minutesfloat = 0f;
    private float timeScale = 1.0f;
    private bool isPaused = true;

    void Update()
    {
        if (!isPaused)
        {
            float deltaTime = Time.deltaTime * timeScale;
            UpdateSimulationTime(deltaTime);

            if (hoursPassed >= 24)
            {
                ResetSimulationTime();
            }
        }
    }

    private void UpdateSimulationTime(float deltaTime)
    {
        minutesfloat += (deltaTime);

        if (minutesfloat >= 60)
        {
            minutesfloat -= 60;
            hour += 1;

            if (hour >= 24)
            {
                hour = 0;
                day += 1;
            }
        }

        hoursPassed += Mathf.RoundToInt(deltaTime);
    }

    private void ResetSimulationTime()
    {
        day = 1;
        hour = 8;
        minutesfloat = 0f;
        hoursPassed = 0;
    }

    public int GetDay()
    {
        return day;
    }

    public int GetHour()
    {
        return hour;
    }

    public int GetMinutes()
    {
        return (int)minutesfloat;
    }

    public int GetHoursPassed()
    {
        return hoursPassed;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
    }
    public void StartClock()
    {
        isPaused = false;
    }

    public void SetTimeScale(float newTimeScale)
    {
        timeScale = Mathf.Max(0.1f, newTimeScale);
    }
}