using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public float countdownLeft;
    public bool timerOn;

    public Text TimerText;
    public bool timerCompleted;
    
    void Start()
    {
        timerOn = true;
    }

   
    void Update()
    {
        if(timerOn)
        {
            
            if (countdownLeft > 0)
            {
                countdownLeft -= Time.deltaTime;
                updateTimer(countdownLeft);
                //Debug.Log("timer started");

            }
            else
            {
                Debug.Log("time is up");
                countdownLeft = 0;
                timerOn = false;
                //timerCompleted = true;
            }
        }

    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        float milliseconds = seconds / 100;

        TimerText.text = string.Format("{0:00} : {1:00}", seconds, milliseconds);

    }


}
