using UnityEngine;

public class Clock : MonoBehaviour
{
    private int day = 1;
    private int hour = 8;
    private int hoursPassed = 0;
    private float minutesfloat = 0f;
    private int realMinutes = 0;
    private float realSeconds = 0;
    private bool isPaused = true;

    void Update()
    {
        if (!isPaused)
        {
            float deltaTime = Time.deltaTime;
            UpdateSimulationTime(deltaTime);
        }
    }

    private void UpdateSimulationTime(float deltaTime)
    {
        minutesfloat += (deltaTime);
        realSeconds += Time.unscaledDeltaTime;

        if (minutesfloat >= 60)
        {
            minutesfloat -= 60;
            hour += 1;
            hoursPassed+= 1;

            if (hour >= 24)
            {
                hour = 0;
                day += 1;
            }
        }

        if (realSeconds >= 60)
        {
            realSeconds -= 60;
            realMinutes += 1;
        }
    }

    public void ResetSimulationTime()
    {
        day = 1;
        hour = 8;
        minutesfloat = 0f;
        hoursPassed = 0;
        realMinutes = 0;
        realSeconds=0;
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

    public int GetRealSeconds()
    {
        return (int)realSeconds;
    }

    public int GetRealMinutes()
    {
        return realMinutes;
    }
    public void Pause()
    {
        isPaused = true;
    }
    public void StartClock()
    {
        isPaused = false;
    }
}